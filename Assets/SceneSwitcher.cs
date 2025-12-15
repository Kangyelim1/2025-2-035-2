using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public GameObject scene2DirtPrefab;
    public GameObject scene2GrassPrefab;
    // ...

    void OnEnable()
    {
        // 씬 로드 이벤트를 구독 (SceneManager.LoadScene() 사용 시)
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "2") // 씬 2가 로드되었을 때만 실행
        {
            // 1. NoiseVoxelMap 컴포넌트를 찾습니다.
            NoiseVoxelMap mapScript = FindObjectOfType<NoiseVoxelMap>();

            if (mapScript != null)
            {
                // 2. Prefab을 씬 2 설정으로 교체합니다.
                mapScript.rockPrefab = scene2DirtPrefab;
                mapScript.orePrefab = scene2GrassPrefab;
                // ... 나머지 Prefab들도 교체

                // 3. (선택 사항) 맵 재생성 함수 호출
                // mapScript.GenerateMap();
            }
        }
    }
}
