using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
   public GameObject scene2DirtPrefab;
    public GameObject scene2GrassPrefab;

    private bool isLoading = false; // 로딩 중인지 체크하는 변수

    public void GoToNextScene()
    {
        // 이미 로딩 중이면 중복 실행 방지
        if (isLoading) return;

        // 현재 씬이 이미 2번이라면 더 이상 로드하지 않음
        if (SceneManager.GetActiveScene().name == "2") return;

        isLoading = true;
        SceneManager.LoadScene("2");
    }

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
        // 로딩 상태 해제
        isLoading = false;
        Debug.Log(scene.name + " 씬 로드 완료 및 로딩 상태 초기화");

        // 여기는 비워두거나 씬 이동 로그 정도만 남겨두세요.
        Debug.Log(scene.name + " 씬이 로드되었습니다.");
    }
}

