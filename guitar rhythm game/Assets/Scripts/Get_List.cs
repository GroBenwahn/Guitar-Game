using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Get_List : MonoBehaviour
{
    private string baseURL = "http://localhost/public_4/get_url.php";

    public Transform content;
    public Font customFont;
    public Sprite customBackground;
    public Image panel;

    public Toggle descendingToggle;  //  이름 올림차순
    public Toggle ascendingToggle;  //  이름  내림차순
    public Toggle groupByMusicianToggle;  // 가수 그룹화 토글

    void Start()
    {
        // 정렬 토글 리스너 추가
        descendingToggle.onValueChanged.AddListener(delegate { FetchAndDisplayData("DESC", groupByMusicianToggle.isOn); });
        ascendingToggle.onValueChanged.AddListener(delegate { FetchAndDisplayData("ASC", groupByMusicianToggle.isOn); });
        groupByMusicianToggle.onValueChanged.AddListener(delegate { FetchAndDisplayData(ascendingToggle.isOn ? "ASC" : "DESC", groupByMusicianToggle.isOn); });

        // 초기 데이터 가져오기 (기본 오름차순)
        FetchAndDisplayData("ASC", false);
    }

    private void FetchAndDisplayData(string sortOrder, bool groupByMusician)
    {
        StartCoroutine(GetDataFromServer(sortOrder, groupByMusician));
    }

    private IEnumerator GetDataFromServer(string sortOrder, bool groupByMusician)
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{baseURL}?sortOrder={sortOrder}&groupByMusician={groupByMusician.ToString().ToLower()}"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                ProcessResponse(www.downloadHandler.text);
            }
        }
    }

    private void ProcessResponse(string jsonResponse)
    {
        Response response = JsonUtility.FromJson<Response>(jsonResponse);

        if (response.success)
        {
            PopulateUIWithData(response.data);
        }
        else
        {
            Debug.LogWarning($"데이터 조회 실패: {response.message}");
        }
    }

    private void PopulateUIWithData(Data[] data)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < data.Length; i++)
        {
            GameObject toggleObj = new GameObject($"Toggle_{i}");
            toggleObj.transform.SetParent(content, false);

            Toggle newToggle = toggleObj.AddComponent<Toggle>();

            GameObject background = new GameObject("Background");
            background.transform.SetParent(toggleObj.transform, false);
            Image backgroundImage = background.AddComponent<Image>();
            backgroundImage.sprite = customBackground;
            newToggle.targetGraphic = backgroundImage;

            RectTransform bgRect = backgroundImage.GetComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(700, 100);

            bgRect.localScale = Vector3.one;

            GameObject checkmark = new GameObject("Checkmark");
            checkmark.transform.SetParent(background.transform, false);
            Image checkmarkImage = checkmark.AddComponent<Image>();
            checkmarkImage.sprite = customBackground;
            RectTransform checkmarkRect = checkmarkImage.GetComponent<RectTransform>();
            checkmarkRect.sizeDelta = new Vector2(20, 20);
            newToggle.graphic = checkmarkImage;

            GameObject label = new GameObject("Label");
            label.transform.SetParent(toggleObj.transform, false);
            Text toggleText = label.AddComponent<Text>();
            toggleText.text = $"{data[i].name} - {data[i].musician} - {data[i].url}";
            toggleText.font = customFont;
            toggleText.color = Color.black;
            toggleText.alignment = TextAnchor.MiddleLeft;

            RectTransform textRect = toggleText.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(250, 100);
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = new Vector2(10, 0);

            RectTransform toggleRect = toggleObj.GetComponent<RectTransform>();
            toggleRect.sizeDelta = new Vector2(350, 100);
            toggleRect.anchorMin = new Vector2(0.5f, 1);
            toggleRect.anchorMax = new Vector2(0.5f, 1);
            toggleRect.pivot = new Vector2(0.5f, 0.5f);
            toggleRect.anchoredPosition = new Vector2(0, -100 * i);

            newToggle.onValueChanged.AddListener(delegate {
                panel.gameObject.SetActive(newToggle.isOn);
            });
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
