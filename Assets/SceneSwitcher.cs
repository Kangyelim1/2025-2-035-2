using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
   public GameObject scene2DirtPrefab;
    public GameObject scene2GrassPrefab;

    void Awake()
    {
        // ⭐ 이 상자가 씬이 바뀌어도 안 사라지게 지켜줍니다.
        DontDestroyOnLoad(gameObject);
    }

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
        // 여기는 비워두거나 씬 이동 로그 정도만 남겨두세요.
        Debug.Log(scene.name + " 씬이 로드되었습니다.");
    }
}

