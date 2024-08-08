using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // 다음 씬의 이름 또는 인덱스 설정
    public string nextSceneName; // 씬 이름으로 전환할 경우
    public int nextSceneIndex; // 씬 인덱스로 전환할 경우

    void Update()
    {
        // 터치 입력 감지
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SwitchScene();
        }

        // 마우스 클릭 감지 (디버그 및 PC에서 테스트 용도)
        if (Input.GetMouseButtonDown(0))
        {
            SwitchScene();
        }
    }

    // 씬 전환 메소드
    void SwitchScene()
    {
        // 씬 이름 또는 씬 인덱스 중 하나만 사용하여 씬 전환
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}