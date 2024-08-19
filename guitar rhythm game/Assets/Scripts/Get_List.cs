using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Get_List : MonoBehaviour
{
    private string baseURL = "http://localhost/public_4/get_url.php"; // PHP 스크립트의 정확한 경로를 지정하세요.

    public Text resultText; // 결과를 표시할 UI 텍스트 컴포넌트

    void Start()
    {
        StartCoroutine(GetDataFromServer());
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

    IEnumerator GetDataFromServer()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(baseURL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
                resultText.text = "Failed to load data";
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                Response response = JsonUtility.FromJson<Response>(jsonResult);

                if (response.success)
                {
                    resultText.text = "Data Loaded Successfully:\n";
                    foreach (var item in response.data)
                    {
                        resultText.text += $"Name: {item.name}, Musician: {item.musician}, URL: {item.url}, Memo: {item.memo}\n";
                    }
                }
                else
                {
                    resultText.text = "Failed to load data: " + response.message;
                }
            }
        }
    }
}
