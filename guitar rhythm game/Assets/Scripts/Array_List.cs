using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Array_List : MonoBehaviour
{
    // ��ũ�Ѻ��� ������ ������Ʈ (��� ��ư���� �ڽ����� ��ġ�� ������Ʈ)
    public Transform content;

    // �������� ������ Ʈ�����ϴ� Toggle
    public Toggle descendingToggle;

    // �������� ������ Ʈ�����ϴ� Toggle
    public Toggle ascendingToggle;

    // ���� ���� ������ ����Ʈ
    private Toggle[] originalOrder;

    private void Start()
    {
        // ��� Toggle�� ���� ������ ����
        originalOrder = content.GetComponentsInChildren<Toggle>();

        // �������� ����� onValueChanged �̺�Ʈ ������ �߰�
        descendingToggle.onValueChanged.AddListener(delegate { SortTogglesByName(descendingToggle.isOn, true); });

        // �������� ����� onValueChanged �̺�Ʈ ������ �߰�
        ascendingToggle.onValueChanged.AddListener(delegate { SortTogglesByName(ascendingToggle.isOn, false); });
    }

    // ���� �޼ҵ�
    private void SortTogglesByName(bool isOn, bool isDescending)
    {
        if (!isOn)
        {
            // ����� ������ ���� ������ ����
            RestoreOriginalOrder();
            return;
        }

        // �ٸ� ���� ��� ��Ȱ��ȭ
        if (isDescending)
        {
            ascendingToggle.isOn = false;
        }
        else
        {
            descendingToggle.isOn = false;
        }

        // �ڽĵ�(Toggle��)�� ����Ʈ�� ��������
        var toggles = content.GetComponentsInChildren<Toggle>().ToList();

        // �̸� ������ ����
        if (isDescending)
        {
            toggles = toggles.OrderByDescending(toggle => toggle.name).ToList();
        }
        else
        {
            toggles = toggles.OrderBy(toggle => toggle.name).ToList();
        }

        // �ڽĵ��� ������ �ٽ� ����
        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].transform.SetSiblingIndex(i);
        }
    }

    // ���� ������ �����ϴ� �޼ҵ�
    private void RestoreOriginalOrder()
    {
        for (int i = 0; i < originalOrder.Length; i++)
        {
            originalOrder[i].transform.SetSiblingIndex(i);
        }
    }
}
