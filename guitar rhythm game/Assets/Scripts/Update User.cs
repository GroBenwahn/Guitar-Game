using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpdateUser : MonoBehaviour
{
    // URL 설정
    private string baseURL = "http://localhost/public/";

    // Edit Input Fileds
    public InputField editnameInputField;
    public InputField editemailInputField;
    public InputField editpasswordInputField;

    public void OnProfileBox()
    {
        string name = PlayerPrefs.GetString("name");
        string email = PlayerPrefs.GetString("email");
        string password = PlayerPrefs.GetString("password");

        // 로그인 된 정보 EditField 에 저장
        editNameInputField.value = name;
        editEmailInputField.value = email;
        editPasswordInputField.value = password;
    }

    // 사용자 추가 메소드
    public void OnEditUserButtonClick()
    {
        string old_name = PlayerPrefs.GetString("name");
        string old_email = PlayerPrefs.GetString("email");
        string old_password = PlayerPrefs.GetString("password");
        // 입력값 가져오기
        string new_name = editNameInputField.text;
        string new_email = editEmailInputField.text;
        string new_password = editPasswordInputField.text;

        // 사용자 추가 코루틴 실행
        StartCoroutine(EditUser(old_name, old_email, old_password, new_name, new_email, new_password));
    }

    // 사용자 정보 수정 코루틴
    private IEnumerator EditUser(string old_name, string old_email, string old_password, string new_name, string new_email, string new_password)
    {
        string url = baseURL + "edit_user.php";

        WWWForm form = new WWWForm();
        form.AddField("old_name", old_name);
        form.AddField("email", old_email);
        form.AddField("password", old_password);

        form.AddField("name", new_name);
        form.AddField("email", new_email);
        form.AddField("password", new_password);

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
                    Debug.Log("User edited successfully.");
                }
                else
                {
                    Debug.LogError("Failed to edit user: " + response.message);
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
    }
}
