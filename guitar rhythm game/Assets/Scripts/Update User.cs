using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpdateUser : MonoBehaviour
{
    // URL ����
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

        // �α��� �� ���� EditField �� ����
        editNameInputField.value = name;
        editEmailInputField.value = email;
        editPasswordInputField.value = password;
    }

    // ����� �߰� �޼ҵ�
    public void OnEditUserButtonClick()
    {
        string old_name = PlayerPrefs.GetString("name");
        string old_email = PlayerPrefs.GetString("email");
        string old_password = PlayerPrefs.GetString("password");
        // �Է°� ��������
        string new_name = editNameInputField.text;
        string new_email = editEmailInputField.text;
        string new_password = editPasswordInputField.text;

        // ����� �߰� �ڷ�ƾ ����
        StartCoroutine(EditUser(old_name, old_email, old_password, new_name, new_email, new_password));
    }

    // ����� ���� ���� �ڷ�ƾ
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

                // JSON �Ľ�
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

    // JSON ���� �Ľ̿� Ŭ����
    [System.Serializable]
    public class Response
    {
        public bool success;
        public string message;
    }
}
