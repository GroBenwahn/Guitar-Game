using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpdateUser : MonoBehaviour
{
    // URL ����
    private string baseURL = "http://localhost/public/";

    // Edit Input Fields
    public InputField editnameInputField;  // ���⿡ �̸� ����
    public InputField editemailInputField; // ���⿡ �̸� ����
    public InputField editpasswordInputField; // ���⿡ �̸� ����

    public void OnProfileBox()
    {   
        // InputField �ʱ�ȭ
        editnameInputField.text = "";
        editemailInputField.text = "";
        editpasswordInputField.text = "";

        editnameInputField.text = PlayerPrefs.GetString("name");
        editemailInputField.text = PlayerPrefs.GetString("email");
    }

    // ����� �߰� �޼ҵ�
    public void OnEditUserButtonClick()
    {
        string old_name = PlayerPrefs.GetString("name");
        string old_email = PlayerPrefs.GetString("email");
        // �Է°� ��������
        string new_name = editnameInputField.text;  // ���⿡ �̸� ����
        string new_email = editemailInputField.text; // ���⿡ �̸� ����
        string new_password = editpasswordInputField.text; // ���⿡ �̸� ����

        // ����� �߰� �ڷ�ƾ ����
        StartCoroutine(EditUser(old_name, old_email, new_name, new_email, new_password));
    }

    

    // ����� ���� ���� �ڷ�ƾ
    private IEnumerator EditUser(string old_name, string old_email, string new_name, string new_email, string new_password)
    {
        string url = baseURL + "edit_user.php";

        WWWForm form = new WWWForm();
        // ���� ����
        form.AddField("old_name", old_name);
        form.AddField("old_email", old_email);

        // ����� ����
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

                // JSON �Ľ�
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

    // JSON ���� �Ľ̿� Ŭ����
    [System.Serializable]
    public class Response
    {
        public bool success;
        public string message;
    }
}
