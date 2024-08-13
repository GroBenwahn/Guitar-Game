using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Array_List : MonoBehaviour
{
    // 스크롤뷰의 콘텐츠 오브젝트 (토글 버튼들이 자식으로 배치된 오브젝트)
    public Transform content;

    // 내림차순 정렬을 트리거하는 Toggle
    public Toggle descendingToggle;

    // 오름차순 정렬을 트리거하는 Toggle
    public Toggle ascendingToggle;

    // 원래 순서 유지용 리스트
    private Toggle[] originalOrder;

    private void Start()
    {
        // 모든 Toggle의 원래 순서를 저장
        originalOrder = content.GetComponentsInChildren<Toggle>();

        // 내림차순 토글의 onValueChanged 이벤트 리스너 추가
        descendingToggle.onValueChanged.AddListener(delegate { SortTogglesByName(descendingToggle.isOn, true); });

        // 오름차순 토글의 onValueChanged 이벤트 리스너 추가
        ascendingToggle.onValueChanged.AddListener(delegate { SortTogglesByName(ascendingToggle.isOn, false); });
    }

    // 정렬 메소드
    private void SortTogglesByName(bool isOn, bool isDescending)
    {
        if (!isOn)
        {
            // 토글이 꺼지면 원래 순서로 복구
            RestoreOriginalOrder();
            return;
        }

        // 다른 정렬 토글 비활성화
        if (isDescending)
        {
            ascendingToggle.isOn = false;
        }
        else
        {
            descendingToggle.isOn = false;
        }

        // 자식들(Toggle들)을 리스트로 가져오기
        var toggles = content.GetComponentsInChildren<Toggle>().ToList();

        // 이름 순으로 정렬
        if (isDescending)
        {
            toggles = toggles.OrderByDescending(toggle => toggle.name).ToList();
        }
        else
        {
            toggles = toggles.OrderBy(toggle => toggle.name).ToList();
        }

        // 자식들의 순서를 다시 설정
        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].transform.SetSiblingIndex(i);
        }
    }

    // 원래 순서로 복구하는 메소드
    private void RestoreOriginalOrder()
    {
        for (int i = 0; i < originalOrder.Length; i++)
        {
            originalOrder[i].transform.SetSiblingIndex(i);
        }
    }
}
