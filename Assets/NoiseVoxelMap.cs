using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NoiseVoxelMap : MonoBehaviour
{
    public GameObject dirtPrefab;
    public GameObject grassPrefab;
    public GameObject waterPrefab;

    public int width = 20;
    public int depth = 20;
    public int maxHeight = 16;
    [SerializeField] float noiseScale = 20f;
    public int waterLevel = 4;

    void Start()
    {
        float offsetX = Random.Range(-9999f, 9999f);
        float offsetZ = Random.Range(-9999f, 9999f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float nx = (x + offsetX) / noiseScale;
                float nz = (z + offsetZ) / noiseScale;
                float noise = Mathf.PerlinNoise(nx, nz);

                int h = Mathf.FloorToInt(noise * maxHeight);

                // 흙 + 풀 생성
                for (int y = 0; y <= h; y++)
                {
                    // 맨 위(y == h)는 풀, 나머지는 흙
                    GameObject prefab = (y == h) ? grassPrefab : dirtPrefab;
                    Place(prefab, x, y, z);
                }

                // 물 채우기
                if (h < waterLevel)
                {
                    for (int y = h + 1; y <= waterLevel; y++)
                    {
                        Place(waterPrefab, x, y, z);
                    }
                }
            }
        }
    }

    // 공통 블록 배치 함수
    private void Place(GameObject prefab, int x, int y, int z)
    {

        //var go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
        ///go.name = $"{prefab.name}{x}{y}_{z}";

        if (prefab == dirtPrefab)
        {
            var go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
            go.name = $"Dirt_{x}_{y}_{z}";

            var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
            b.type = ItemType.Dirt;
            b.maxHP = 3;
            b.dropCount = 1;
            b.mineable = true;
        }
        else if (prefab == grassPrefab)
        {
            var go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
            go.name = $"Grass_{x}_{y}_{z}";

            var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
            b.type = ItemType.Grass;
            b.maxHP = 3;
            b.dropCount = 1;
            b.mineable = true;
        }
        else if (prefab == waterPrefab)
        {
            var go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
            go.name = $"Water_{x}_{y}_{z}";

            var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
            b.type = ItemType.Water;
            b.maxHP = 3;
            b.dropCount = 1;
            b.mineable = true;
        }

    }

    public void PlaceTile(Vector3Int pos, ItemType type)
    {
        switch (type)
        {
            case ItemType.Dirt:
                Place(dirtPrefab, pos.x, pos.y, pos.z);
                break;
            case ItemType.Grass:
                Place(grassPrefab, pos.x, pos.y, pos.z);
                break;
            case ItemType.Water:
                Place(waterPrefab, pos.x, pos.y, pos.z);
                break;
        }
    }
}
