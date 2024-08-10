using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpdateUser : MonoBehaviour
{
    // URL 설정
    private string baseURL = "http://localhost/public/";

    // Edit Input Fields
    public InputField editnameInputField;  // 여기에 이름 수정
    public InputField editemailInputField; // 여기에 이름 수정
    public InputField editpasswordInputField; // 여기에 이름 수정

    public void OnProfileBox()
    {
        editnameInputField.text = "";
        editemailInputField.text = "";
        editpasswordInputField.text = "";

        editnameInputField.text = PlayerPrefs.GetString("name");
        editemailInputField.text = PlayerPrefs.GetString("email");
    }

    /*
    // 사용자 정보 요청 코루틴
    private IEnumerator RequestUserInfo(string name, string email)
    {
        string url = baseURL + "get_user.php";

        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("email", email);

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
                    Debug.Log("Get User Information successfully." + response.name + "!");
                    editnameInputField.text = name;
                    editemailInputField.text = email;
                }
                else
                {
                    Debug.LogError("Failed to Login: " + response.message);\
                }
            }
        }
    }
    */

    // 사용자 추가 메소드
    public void OnEditUserButtonClick()
    {
        string old_name = PlayerPrefs.GetString("name");
        string old_email = PlayerPrefs.GetString("email");
        // 입력값 가져오기
        string new_name = editnameInputField.text;  // 여기에 이름 수정
        string new_email = editemailInputField.text; // 여기에 이름 수정
        string new_password = editpasswordInputField.text; // 여기에 이름 수정

        // 사용자 추가 코루틴 실행
        StartCoroutine(EditUser(old_name, old_email, new_name, new_email, new_password));
    }

    

    // 사용자 정보 수정 코루틴
    private IEnumerator EditUser(string old_name, string old_email, string new_name, string new_email, string new_password)
    {
        string url = baseURL + "edit_user.php";

        WWWForm form = new WWWForm();
        form.AddField("old_name", old_name);
        form.AddField("old_email", old_email); // 여기에서 'email'을 'old_email'로 수정

        form.AddField("new_name", new_name); // 여기에서 'name'을 'new_name'으로 수정
        form.AddField("new_email", new_email); // 여기에서 'email'을 'new_email'으로 수정
        form.AddField("new_password", new_password); // 여기에서 'password'를 'new_password'로 수정

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
                    Debug.Log("User Updated successfully.");
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
