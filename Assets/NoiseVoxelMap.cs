using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class NoiseVoxelMap : MonoBehaviour
{
    public GameObject dirtPrefab;
    public GameObject grassPrefab;
    public GameObject waterPrefab;
    public GameObject treePrefab;
    public GameObject leafPrefab;
    public GameObject rockPrefab;
    public GameObject orePrefab;

    public int width = 50;
    public int depth = 50;
    public int maxHeight = 20;
    [SerializeField] float noiseScale = 20f;
    public int waterLevel = 4;

    [Range(0, 0.1f)] public float treeDensity = 0.05f; // 5% 확률로 생성

    void Start()
    {
        // 1. 현재 씬이 "2"인지 확인
        if (SceneManager.GetActiveScene().name == "2")
        {
            // 하이어라키에서 SceneSwitcher를 찾음
            SceneSwitcher switcher = FindObjectOfType<SceneSwitcher>();

            if (switcher != null)
            {
                // 프리팹 교체
                dirtPrefab = switcher.scene2DirtPrefab;
                grassPrefab = switcher.scene2GrassPrefab;
                treeDensity = 0f; // 씬 2는 나무 없음
                waterLevel = -1;  // 씬 2는 물 없음
                Debug.Log("씬 2 설정 적용 완료!");
            }
            else
            {
                Debug.LogError("SceneSwitcher를 찾을 수 없습니다! DontDestroyOnLoad를 확인하세요.");
            }
        }

        // 2. 맵 생성 함수 호출 (무조건 실행됨)
        GenerateMap();
    }

    public void GenerateMap()
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

                if (h >= waterLevel && Random.value < 0.03f)
                {
                    GenerateTree(x, h + 1, z); // 풀 바로 위에 생성
                }

                // 물 채우기
                if (h < waterLevel)
                {
                    for (int y = h + 1; y <= waterLevel; y++)
                    {
                        Place(waterPrefab, x, y, z);
                    }
                }

                // 나무 생성 로직 추가
                if (Random.value < treeDensity)
                {
                    GenerateTree(x, h + 1, z);
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
            b.maxHP = 1;
            b.dropCount = 1;
            b.mineable = true;
        }
        else if (prefab == waterPrefab)
        {
            var go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
            go.name = $"Water_{x}_{y}_{z}";

            var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
            b.type = ItemType.Water;
            b.maxHP = 1;
            b.dropCount = 1;
            b.mineable = true;
        }
        else if (prefab == treePrefab) // 4. 나무 Prefab인 경우
        {
            var go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
            go.name = $"Tree_{x}_{y}_{z}";

            var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
            b.type = ItemType.Tree; // 5. 아이템 타입 변경
            b.maxHP = 1; // 6. 나무에 맞는 HP 설정 (예시)
            b.dropCount = 1;
            b.mineable = true;
        }
        else if (prefab == rockPrefab) // 1. 돌 Prefab인 경우
        {
            var go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
            go.name = $"rock_{x}_{y}_{z}";

            var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
            b.type = ItemType.rock; // 2. 아이템 타입 변경
            b.maxHP = 1; // 3. 돌에 맞는 HP 설정 (예시)
            b.dropCount = 1;
            b.mineable = true;
        }
        else if (prefab == orePrefab) // 1. 돌 Prefab인 경우
        {
            var go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
            go.name = $"ore_{x}_{y}_{z}";

            var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
            b.type = ItemType.ore; // 2. 아이템 타입 변경
            b.maxHP = 1; // 3. 돌에 맞는 HP 설정 (예시)
            b.dropCount = 1;
            b.mineable = true;
        }

    }

    void GenerateTree(int x, int y, int z)
    {
        int th = Random.Range(4, 6); // 나무 높이

        // 기둥 생성
        for (int i = 0; i < th; i++) Place(treePrefab, x, y + i, z);

        // 잎 생성: 기둥 꼭대기에 3x3 평면 하나만 딱! (가장 간단한 버전)
        for (int lx = -1; lx <= 1; lx++)
        {
            for (int lz = -1; lz <= 1; lz++)
            {
                // 기둥 맨 위(y + th - 1) 바로 윗칸에 잎 배치
                Place(leafPrefab, x + lx, y + th, z + lz);
            }
        }
    }

    public void PlaceTile(Vector3Int pos, ItemType type)
{
    GameObject prefabToPlace = null;

    // 아이템 타입에 따라 어떤 프리팹을 쓸지 결정 (ItemType 정의에 따라 수정 필요)
    switch (type)
    {
        case ItemType.Dirt: prefabToPlace = dirtPrefab; break;
        case ItemType.Grass: prefabToPlace = grassPrefab; break;
        case ItemType.rock: prefabToPlace = rockPrefab; break;
        case ItemType.Tree: prefabToPlace = treePrefab; break; // 나무 기둥
        // ... 필요한 만큼 추가
    }

    if (prefabToPlace != null)
    {
        Place(prefabToPlace, pos.x, pos.y, pos.z);
    }
}


}
