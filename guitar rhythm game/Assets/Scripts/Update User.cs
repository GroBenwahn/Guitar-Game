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
        // InputField 초기화
        editnameInputField.text = "";
        editemailInputField.text = "";
        editpasswordInputField.text = "";

        editnameInputField.text = PlayerPrefs.GetString("name");
        editemailInputField.text = PlayerPrefs.GetString("email");
    }

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
        // 이전 정보
        form.AddField("old_name", old_name);
        form.AddField("old_email", old_email);

        // 변경된 정보
        form.AddField("new_name", new_name);
        form.AddField("new_email", new_email);
        form.AddField("new_password", new_password);

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
