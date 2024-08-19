using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class Get_List : MonoBehaviour
{
    private string baseURL = "http://localhost/public_4/get_url.php";

    // 데이터를 저장할 배열들
    public string[] names;
    public string[] musicians;
    public string[] urls;
    public string[] memos;

    public Transform content; // UI 요소를 담을 부모 객체
    public Font customFont; // 사용자 지정 폰트
    public Sprite customBackground; // 사용자 지정 배경 이미지
    public Image panel;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI musicianText;
    public TextMeshProUGUI urlText;

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
            PopulateData(response.data);
            PopulateUIWithData();
        }
        else
        {
            Debug.LogWarning($"데이터 조회 실패: {response.message}");
        }
    }

    private void PopulateData(Data[] data)
    {
        // 데이터 개수에 맞춰 배열 초기화
        names = new string[data.Length];
        musicians = new string[data.Length];
        urls = new string[data.Length];
        memos = new string[data.Length];

        // 배열에 데이터 저장
        for (int i = 0; i < data.Length; i++)
        {
            names[i] = data[i].name;
            musicians[i] = data[i].musician;
            urls[i] = data[i].url;
            memos[i] = data[i].memo;
        }
    }

    private void PopulateUIWithData()
    {
        // 기존 UI 요소들을 초기화
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 배열 데이터에 기반하여 토글 생성 및 설정
        for (int i = 0; i < names.Length; i++)
        {
            string nn = names[i];
            string mm = musicians[i];
            string uu = urls[i];
            // 새 GameObject 생성
            GameObject toggleObj = new GameObject($"Toggle_{i}");
            toggleObj.transform.SetParent(content, false);

            // Toggle 컴포넌트 추가
            Toggle newToggle = toggleObj.AddComponent<Toggle>();

            // Background 및 체크 표시를 위한 Image 추가
            GameObject background = new GameObject("Background");
            background.transform.SetParent(toggleObj.transform, false);
            Image backgroundImage = background.AddComponent<Image>();
            backgroundImage.sprite = customBackground;
            newToggle.targetGraphic = backgroundImage;

            // 이미지 크기 조절
            RectTransform bgRect = backgroundImage.GetComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(700, 100); // 예시로 이미지 크기를 설정

            // 이미지 비율 유지하면서 크기 조절
            bgRect.localScale = Vector3.one; // 기본 스케일 설정

            // Checkmark 생성 및 설정
            GameObject checkmark = new GameObject("Checkmark");
            checkmark.transform.SetParent(background.transform, false);
            Image checkmarkImage = checkmark.AddComponent<Image>();
            checkmarkImage.sprite = customBackground;
            RectTransform checkmarkRect = checkmarkImage.GetComponent<RectTransform>();
            checkmarkRect.sizeDelta = new Vector2(20, 20); // 체크박스 크기 조절
            newToggle.graphic = checkmarkImage;

            // Text 생성 및 설정
            GameObject label = new GameObject("Label");
            label.transform.SetParent(toggleObj.transform, false);
            Text toggleText = label.AddComponent<Text>();
            toggleText.text = $"{names[i]} - {musicians[i]} - {urls[i]}";
            toggleText.font = customFont;
            toggleText.color = Color.black;
            toggleText.alignment = TextAnchor.MiddleLeft;

            // Text 크기 조절
            RectTransform textRect = toggleText.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(250, 100); // 텍스트 영역 크기 설정
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = new Vector2(10, 0);

            // 토글 크기 설정
            RectTransform toggleRect = toggleObj.GetComponent<RectTransform>();
            toggleRect.sizeDelta = new Vector2(350, 100); // 토글 크기 설정
            toggleRect.anchorMin = new Vector2(0.5f, 1); // `Viewport`에 상대적인 위치 설정
            toggleRect.anchorMax = new Vector2(0.5f, 1);
            toggleRect.pivot = new Vector2(0.5f, 0.5f);
            toggleRect.anchoredPosition = new Vector2(0, -100 * i); // 위치 조정

            // Toggle의 OnValueChanged 이벤트에 리스너 추가
            newToggle.onValueChanged.AddListener(delegate {
                panel.gameObject.SetActive(newToggle.isOn);
                nameText.text = nn;
                musicianText.text = mm;
                urlText.text = uu;
            });
        }
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