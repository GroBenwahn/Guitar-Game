using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class SignManager : MonoBehaviour
{
    // URL 설정
    private string baseURL = "http://localhost/public/";

    // Input Fields
    public InputField emailInputField;
    public InputField passwordInputField;
    

    // 결과 텍스트 출력
    private Text resultText;

    public TextMeshProUGUI profile;

    // 사용자 추가 메소드
    public void OnSignInButtonClick()
    {
        // 입력값 가져오기
        string email = emailInputField.text;
        string password = passwordInputField.text;

        // 사용자 추가 코루틴 실행
        StartCoroutine(Sign_in(email, password));
        
    }

    // 사용자 추가 코루틴
    private IEnumerator Sign_in(string email, string password)
    {
        string url = baseURL + "sign_in.php";  // sign_in.php 파일이 서버에 있어야 합니다.

        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                //resultText.text = "Error: " + request.error; // 결과 텍스트에 오류 메시지 출력
            }
            else
            {
                string json = request.downloadHandler.text;
                Debug.Log("Response: " + json);
                //resultText.text = "Response: " + json; // 결과 텍스트에 서버 응답 출력

                // JSON 파싱
                Response response = JsonUtility.FromJson<Response>(json);

                if (response.success)
                {
                    Debug.Log("Login successfully. Welcome " + response.name + "!");
                    //resultText.text = "Login successfully. Welcome " + response.name + "!";
                    profile.text = response.name;
                    Debug.Log(profile);
                }
                else
                {
                    Debug.LogError("Failed to Login: " + response.message);
                    //resultText.text = "Failed to Login: " + response.message;
                }
            }
        }
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
