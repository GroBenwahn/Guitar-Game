using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Array_List : MonoBehaviour
{
    
    public Transform content;

    
    public Toggle descendingToggle;

    public Toggle ascendingToggle;

    private Toggle[] originalOrder;

    private void Start()
    {
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
