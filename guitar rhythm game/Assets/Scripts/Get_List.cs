using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Get_List : MonoBehaviour
{
    // 서버의 PHP 스크립트 URL을 저장
    private string baseURL = "http://localhost/public_4/get_url.php";

    // 데이터 배열: 이름, 뮤지션, URL, 메모를 각각 저장
    public string[] names;
    public string[] musicians;
    public string[] urls;
    public string[] memos;

    // UI 요소들
    public Transform content;               // 생성된 버튼을 담을 부모 객체
    public Font customFont;                 // 사용자 지정 폰트
    public Sprite customBackground;  // 버튼 배경 이미지
    public Image panel;                        // 클릭된 버튼에 대한 세부 정보 패널

    // UI에서 사용할 버튼들
    public Button descendingButton;            // 내림차순 정렬 버튼
    public Button ascendingButton;             // 오름차순 정렬 버튼
    public Button groupByMusicianButton; // 뮤지션별 그룹화 버튼

    public TMP_InputField searchInputField;   // 검색 기능을 위한 InputField
    public TextMeshProUGUI nameText;          // 선택된 항목의 이름을 표시할 텍스트
    public TextMeshProUGUI musicianText;    // 선택된 항목의 뮤지션을 표시할 텍스트
    public TextMeshProUGUI urlText;             // 선택된 항목의 URL을 표시할 텍스트

    public Image albumImage;            // 앨범 이미지

    private bool isGroupedByMusician = false;  // 현재 데이터가 뮤지션별로 그룹화되어 있는지 추적

    public Button gamestart;

    private string videoID;  // videoID 변수 추가

    void Start()
    {
        // 내림차순 정렬 버튼 클릭 이벤트 리스너 등록
        descendingButton.onClick.AddListener(() => FetchAndDisplayData("DESC", isGroupedByMusician, searchInputField.text));

        // 오름차순 정렬 버튼 클릭 이벤트 리스너 등록
        ascendingButton.onClick.AddListener(() => FetchAndDisplayData("ASC", isGroupedByMusician, searchInputField.text));

        // 뮤지션별 그룹화 버튼 클릭 이벤트 리스너 등록
        groupByMusicianButton.onClick.AddListener(ToggleGroupByMusician);

        // 검색 InputField의 값이 변경될 때마다 데이터를 다시 가져오도록 리스너 등록
        searchInputField.onValueChanged.AddListener(delegate {
            FetchAndDisplayData(ascendingButton.name == "ASC" ? "ASC" : "DESC", isGroupedByMusician, searchInputField.text);
        });

        // 초기 데이터 가져오기 (기본 오름차순 정렬, 그룹화 없음)
        FetchAndDisplayData("ASC", false, "");
    }

    // 뮤지션별 그룹화 버튼을 클릭했을 때 호출되는 메서드
    private void ToggleGroupByMusician()
    {
        isGroupedByMusician = !isGroupedByMusician;  // 그룹화 상태를 반전
        FetchAndDisplayData(ascendingButton.name == "ASC" ? "ASC" : "DESC", isGroupedByMusician, searchInputField.text); // 변경된 그룹화 상태로 데이터를 다시 가져옴
    }

    // 서버로부터 데이터를 가져오고 UI에 표시하는 메서드
    private void FetchAndDisplayData(string sortOrder, bool groupByMusician, string searchName)
    {
        StartCoroutine(GetDataFromServer(sortOrder, groupByMusician, searchName));  // 데이터를 가져오는 코루틴 시작
    }

    // 서버에서 데이터를 비동기로 가져오는 코루틴
    private IEnumerator GetDataFromServer(string sortOrder, bool groupByMusician, string searchName)
    {
        // URL을 구성하여 정렬 순서, 그룹화 상태, 검색어를 포함
        string url = $"{baseURL}?sortOrder={sortOrder}&groupByMusician={groupByMusician.ToString().ToLower()}&searchName={UnityWebRequest.EscapeURL(searchName)}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))  // GET 요청을 보냄
        {
            yield return www.SendWebRequest();   // 요청이 완료될 때까지 대기

            if (www.result != UnityWebRequest.Result.Success)          // 요청 실패 시
            {
                Debug.LogError(www.error);           // 오류 메시지 출력
            }
            else    // 요청 성공 시
            {
                ProcessResponse(www.downloadHandler.text);              // 서버의 응답 데이터를 처리
            }
        }
    }

    // 서버의 응답을 처리하는 메서드
    private void ProcessResponse(string jsonResponse)
    {
        // JSON 응답을 파싱하여 Response 객체로 변환
        Response response = JsonUtility.FromJson<Response>(jsonResponse);

        if (response.success)                                  // 데이터 조회 성공 시
        {
            if (response.data.Length > 0)                  // 데이터가 존재할 경우
            {
                PopulateUIWithData(response.data);  // 데이터를 UI에 표시
            }
            else
            {
                Debug.Log("일치하지 않습니다.");         // 일치하는 데이터가 없음을 알림
            }
        }
        else  // 데이터 조회 실패 시
        {
            Debug.LogWarning($"데이터 조회 실패: {response.message}");  // 오류 메시지 출력
        }
    }

    public static string ExtractYouTubeId(string URL)
    {
        string[] parts = URL.Split('/');
        string lastPart = parts[parts.Length - 1];

        string videoId = lastPart.Split('?')[0];

        return videoId;
    }

    // 서버에서 받아온 데이터를 UI에 표시하는 메서드
    private void PopulateUIWithData(Data[] data)
    {
        // 데이터 개수에 맞춰 배열 초기화
        names = new string[data.Length];
        musicians = new string[data.Length];
        urls = new string[data.Length];
        memos = new string[data.Length];

        // 배열에 데이터를 저장
        for (int i = 0; i < data.Length; i++)
        {
            names[i] = data[i].name;
            musicians[i] = data[i].musician;
            urls[i] = data[i].url;
            memos[i] = data[i].memo;
        }

        // 기존 UI 요소 제거 (이전의 데이터 삭제)
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 새로운 데이터로 UI를 채우기
        for (int i = 0; i < data.Length; i++)
        {
            string nn = names[i];
            string mm = musicians[i];
            string uu = urls[i];

            // 새로운 버튼 생성
            GameObject buttonObj = new GameObject($"Button_{i}");
            buttonObj.transform.SetParent(content, false);

            // 버튼의 RectTransform 설정
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(350, 100);
            buttonRect.anchorMin = new Vector2(0.5f, 1);
            buttonRect.anchorMax = new Vector2(0.5f, 1);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.anchoredPosition = new Vector2(0, -100 * i);

            Button newButton = buttonObj.AddComponent<Button>();               // 버튼 컴포넌트 추가

            // 버튼의 배경 이미지 설정
            GameObject background = new GameObject("Background");
            background.transform.SetParent(buttonObj.transform, false);
            Image backgroundImage = background.AddComponent<Image>();
            backgroundImage.sprite = customBackground;
            newButton.targetGraphic = backgroundImage;

            RectTransform bgRect = backgroundImage.GetComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(700, 100);
            bgRect.localScale = Vector3.one;

            // 버튼의 텍스트 레이블 설정
            GameObject label = new GameObject("Label");
            label.transform.SetParent(buttonObj.transform, false);
            Text buttonText = label.AddComponent<Text>();
            buttonText.text = $"{names[i]} - {musicians[i]} - {urls[i]}";
            buttonText.font = customFont;
            buttonText.fontSize = 25;
            buttonText.color = Color.black;
            buttonText.alignment = TextAnchor.MiddleLeft;

            RectTransform textRect = buttonText.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(250, 100);
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = new Vector2(10, 0);

            // 버튼 클릭 시 패널에 세부 정보 표시
            newButton.onClick.AddListener(delegate {
                panel.gameObject.SetActive(true);     // 패널 표시
                nameText.text = nn;                            // 이름 텍스트 업데이트
                musicianText.text = mm;                    // 뮤지션 텍스트 업데이트
                urlText.text = uu;                               // URL 텍스트 업데이트

                gamestart.onClick.AddListener(() => {
                videoID = ExtractYouTubeId(uu);
                

                if (PlayerPrefs.HasKey(videoID)) 
                {
                    PlayerPrefs.DeleteKey(videoID);
                }
                else
                {
                    PlayerPrefs.SetString("VideoId", videoID);
                    PlayerPrefs.Save();
                }
                SceneManager.LoadScene("Game Scene");

                Debug.Log("videoID 저장됨: " + videoID);
                });
            });
        }
    }

    public void LoadImageFromDatabase(int imageID)
    {
        StartCoroutine(LoadImageCoroutine(imageID));
    }

    private IEnumerator LoadImageCoroutine(int imageID)
    {
        string url = "http://localhost/public_4/fetch_image.php?num=" + imageID;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error loading image: " + request.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            albumImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }

    // JSON 응답에서 사용할 데이터 구조 정의
    [System.Serializable]
    public class Response
    {
        public bool success;        // 요청 성공 여부
        public string message;    // 응답 메시지
        public Data[] data;          // 데이터 배열
    }

    // 데이터 항목의 구조 정의
    [System.Serializable]
    public class Data
    {
        public string name;           // 데이터의 이름
        public string musician;     // 뮤지션 이름
        public string url;              // 관련 URL
        public string memo;        // 메모
    }
}
