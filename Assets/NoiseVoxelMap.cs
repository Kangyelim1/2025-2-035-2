using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NoiseVoxelMap : MonoBehaviour
{
    public GameObject dirtPrefab;
    public GameObject grassPrefab;
    public GameObject waterPrefab;
    public GameObject treePrefab;

    public int width = 20;
    public int depth = 20;
    public int maxHeight = 16;
    [SerializeField] float noiseScale = 20f;
    public int waterLevel = 4;

    //나무
    public float treeDensity = 0.01f;
    private const int GRASS_BLOCK_ID = 1;
    public const int CHUNK_SIZE = 16;

    void GenerateChunk()
    {
        for (int z = 0; z < CHUNK_SIZE; z++)
        {
            // 여기서 GetBlockType 사용
            if (GetBlockType(0, 0, z) == GRASS_BLOCK_ID)
            {
                // 나무 생성
            }
        }
    }

    // ✅ 여기에 추가하세요 (가장 아래가 제일 안전)
    int GetBlockType(int x, int y, int z)
    {
        return GRASS_BLOCK_ID; // 임시
    }

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

    void PlaceTrees(Vector3Int chunkStartPos)
    {
        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int z = 0; z < CHUNK_SIZE; z++)
            {
                Vector3Int worldPos = chunkStartPos + new Vector3Int(x, 0, z);
                int groundY = GetHighestBlockY(worldPos.x, worldPos.z);

                // 나무가 배치될 최종 위치 (잔디 블록 바로 위)
                Vector3Int treePos = new Vector3Int(worldPos.x, groundY + 1, worldPos.z);
                if (GetBlockType(worldPos.x, groundY, worldPos.z) == GRASS_BLOCK_ID)
                {
                    // 2. 설정된 확률로 나무를 심을지 결정합니다.
                    if (Random.value < treeDensity)
                    {
                        InstantiateTree(treePos);
                    }
                }
            }
        }

    }

    int GetHighestBlockY(int x, int z)
    {
        return 60;
    }



    void InstantiateTree(Vector3Int position)
    {
        Instantiate(treePrefab, position, Quaternion.identity); 
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
