using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class SigninManager : MonoBehaviour
{
    // URL 설정
    private string baseURL = "http://localhost/public/";

    // Input Fields
    public InputField emailInputField;
    public InputField passwordInputField;

    // 결과 텍스트 출력
    private Text resultText;

    // 자동 로그인 여부를 위한 Toggle
    public Toggle autoLoginToggle;

    // 로그인 name 가져오는 변수
    public TextMeshProUGUI profile;

    // profile 박스& 로그인 토글
    public GameObject profileBox;
    public Toggle loginToggle;


    // 로그인 버튼 클릭 메소드
    public void OnSignInButtonClick()
    {
        // 입력값 가져오기
        string email = emailInputField.text;
        string password = passwordInputField.text;

        // 로그인 코루틴 실행
        StartCoroutine(SignIn(email, password));
    }

    // 앱 시작 시 자동 로그인 여부 확인
    private void Start()
    {
        // 자동 로그인 여부 체크
        bool isAutoLoginEnabled = PlayerPrefs.GetInt("autoLogin", 0) == 1;
        autoLoginToggle.isOn = isAutoLoginEnabled; // UI Toggle 상태 설정

        Debug.Log("Auto login enabled: " + isAutoLoginEnabled);

        // 자동 로그인이 활성화된 경우, 저장된 이메일과 비밀번호로 자동 로그인 시도
        if (isAutoLoginEnabled)
        {
            if (PlayerPrefs.HasKey("email") && PlayerPrefs.HasKey("password"))
            {
                string email = PlayerPrefs.GetString("email");
                string password = PlayerPrefs.GetString("password");

                Debug.Log("Attempting auto sign-in with email: " + email);

                StartCoroutine(AutoSignIn(email, password));
            }
            else
            {
                Debug.Log("No stored credentials found.");
            }
        }
    }


    // 로그인 코루틴
    private IEnumerator SignIn(string email, string password)
    {
        string url = baseURL + "sign_in.php";

        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                Debug.Log("Response: " + json);

                // JSON 파싱
                Response response = JsonUtility.FromJson<Response>(json);

                if (response.success)
                {
                    Debug.Log("Login successful. Welcome " + response.name + "!");
                    profile.text = response.name;
                    profileBox.SetActive(true);
                    loginToggle.gameObject.SetActive(false);

                    // 자동 로그인 설정 여부에 따라 저장
                    if (autoLoginToggle.isOn)
                    {   
                        // 자동로그인 -> password 저장
                        PlayerPrefs.SetString("name", response.name);
                        PlayerPrefs.SetString("email", email);
                        PlayerPrefs.SetString("password", password);
                        PlayerPrefs.SetInt("autoLogin", 1);
                        Debug.Log("Auto Login set 1");
                    }
                    else
                    {   // 일반로그인 -> password 삭제
                        PlayerPrefs.SetString("name", response.name);
                        PlayerPrefs.SetString("email", email);
                        PlayerPrefs.DeleteKey("password");
                        PlayerPrefs.SetInt("autoLogin", 0);
                        Debug.Log("Auto Login set 0");
                    }

                    PlayerPrefs.Save();
                }
                else
                {
                    Debug.LogError("Failed to Login: " + response.message);
                }
            }
        }
    }

    // 자동 로그인 코루틴
    private IEnumerator AutoSignIn(string email, string password)
    {
        string url = baseURL + "sign_in.php";

        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                Debug.Log("Auto sign-in response: " + json);

                Response response = JsonUtility.FromJson<Response>(json);

                if (response.success)
                {
                    profile.text = response.name;
                    profileBox.SetActive(true);
                    loginToggle.gameObject.SetActive(false);
                    Debug.Log("Auto login successful. Welcome back " + response.name + "!");
                }
                else
                {
                    Debug.LogError("Failed to Auto Login: " + response.message);
                    // 자동 로그인 실패 시, 로그인 화면 유지
                }
            }
        }
    }

    // 자동 로그인 Toggle 변경 시 호출
    public void OnAutoLoginToggleChanged(bool isOn)
    {
        if (isOn)
        {
            Debug.Log("자동 로그인 활성화");
            PlayerPrefs.SetInt("autoLogin", 1);
        }
        else
        {
            Debug.Log("자동 로그인 비활성화");
            PlayerPrefs.DeleteKey("email");
            PlayerPrefs.DeleteKey("password");
            PlayerPrefs.SetInt("autoLogin", 0);
        }
        PlayerPrefs.Save();
    }


    // 로그아웃 메소드
    public void OnLogoutButtonClick()
    {
        PlayerPrefs.DeleteKey("name");
        PlayerPrefs.DeleteKey("email");
        PlayerPrefs.DeleteKey("password");
        PlayerPrefs.SetInt("autoLogin", 0);
        PlayerPrefs.Save();

        // inputField 초기화
        emailInputField.text = "";
        passwordInputField.text = "";

        Debug.Log("Logged out successfully.");

        profileBox.SetActive(false);
        loginToggle.gameObject.SetActive(true);
    }

    // JSON 응답 파싱용 클래스
    [System.Serializable]
    public class Response
    {
        public bool success;
        public string message;
        public string name; // 서버에서 전달받은 이름 저장
    }
}
