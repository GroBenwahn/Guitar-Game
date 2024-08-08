using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // ���� ���� �̸� �Ǵ� �ε��� ����
    public string nextSceneName; // �� �̸����� ��ȯ�� ���
    public int nextSceneIndex; // �� �ε����� ��ȯ�� ���

    void Update()
    {
        // ��ġ �Է� ����
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SwitchScene();
        }

        // ���콺 Ŭ�� ���� (����� �� PC���� �׽�Ʈ �뵵)
        if (Input.GetMouseButtonDown(0))
        {
            SwitchScene();
        }
    }

    // �� ��ȯ �޼ҵ�
    void SwitchScene()
    {
        // �� �̸� �Ǵ� �� �ε��� �� �ϳ��� ����Ͽ� �� ��ȯ
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