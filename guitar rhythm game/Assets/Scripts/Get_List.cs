using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Get_List : MonoBehaviour
{
    private string baseURL = "http://localhost/public_4/get_url.php";
    
    public Transform content; // UI 요소를 담을 부모 객체
    public GameObject togglePrefab; // 토글 프리팹

    void Start()
    {
        StartCoroutine(GetDataFromServer());
    }

    private IEnumerator GetDataFromServer()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(baseURL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // 서버에서 반환된 JSON 데이터 처리
                Debug.Log(www.downloadHandler.text);
                ProcessResponse(www.downloadHandler.text);
            }
        }
    }

    private void ProcessResponse(string jsonResponse)
    {
        // JSON 응답 파싱
        Response response = JsonUtility.FromJson<Response>(jsonResponse);

        if (response.success)
        {
            Debug.Log("데이터 조회 성공.");
            PopulateUI(response.data);
        }
        else
        {
            Debug.LogWarning($"데이터 조회 실패: {response.message}");
        }
    }

    private void PopulateUI(Data[] data)
    {
        // UI를 초기화
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 데이터에 기반하여 토글 생성
        foreach (var item in data)
        {
            GameObject toggleObj = Instantiate(togglePrefab, content);
            Toggle newToggle = toggleObj.GetComponent<Toggle>();

            // Toggle의 이름을 DB에서 가져온 name 필드로 설정합니다.
            newToggle.name = item.name;

            // 텍스트를 name 필드로 설정
            toggleObj.GetComponentInChildren<Text>().text = $"{item.name} - {item.musician}";

            // 필요에 따라 추가적인 설정 및 이벤트 핸들러 설정
        }
    }

    [System.Serializable]
    public class Response
    {
        public bool success;
        public string message;
        public Data[] data;
    }

    [System.Serializable]
    public class Data
    {
        public string name;
        public string musician;
        public string url;
        public string memo;
    }
}
