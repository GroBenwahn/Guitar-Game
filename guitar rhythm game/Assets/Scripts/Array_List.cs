using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Array_List : MonoBehaviour
{
    public Transform content;
    public Toggle descendingToggle;
    public Toggle ascendingToggle;

    private List<Toggle> toggleList = new List<Toggle>();
    private Toggle[] originalOrder;

    private void Start()
    {
        // 기존 UI 토글을 가져와서 원래 순서를 저장합니다.
        originalOrder = content.GetComponentsInChildren<Toggle>();

        descendingToggle.onValueChanged.AddListener(delegate { SortTogglesByName(descendingToggle.isOn, true); });
        ascendingToggle.onValueChanged.AddListener(delegate { SortTogglesByName(ascendingToggle.isOn, false); });
    }

    private void SortTogglesByName(bool isOn, bool isDescending)
    {
        if (!isOn)
        {
            RestoreOriginalOrder();
            return;
        }

        if (isDescending)
        {
            ascendingToggle.isOn = false;
        }
        else
        {
            descendingToggle.isOn = false;
        }

        var toggles = content.GetComponentsInChildren<Toggle>().ToList();

        if (isDescending)
        {
            toggles = toggles.OrderByDescending(toggle => toggle.name).ToList();
        }
        else
        {
            toggles = toggles.OrderBy(toggle => toggle.name).ToList();
        }

        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].transform.SetSiblingIndex(i);
        }
    }

    private void RestoreOriginalOrder()
    {
        for (int i = 0; i < originalOrder.Length; i++)
        {
            originalOrder[i].transform.SetSiblingIndex(i);
        }
    }
}
