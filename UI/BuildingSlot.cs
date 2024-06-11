using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingSlot : MonoBehaviour, IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
//, IDragHandler
{
    public BuildData building; // BuildData 필드 추가

    public Button button;
    public Image icon;

    public UIBuilding buildingUI;
    public UIBuildToolTip toolTip;

    public int index;

    private void Awake()
    {
        //if (button != null)
        //{
        //    button.onClick.AddListener(OnClick); // Button 컴포넌트의 OnClick 이벤트에 메서드 추가
        //}
    }

    private void OnEnable()
    {
        //if (building != null)
        //{
        //    Set();
        //}
    }

    private void OnDisable()
    {
        //if (button != null)
        //{
        //    button.onClick.RemoveListener(OnClick); // Button 컴포넌트의 OnClick 이벤트에서 메서드 제거
        //}
        toolTip.gameObject.SetActive(false);
    }

    public void Set()
    {
        icon.sprite = building.icon;
        icon.gameObject.SetActive(true);
    }

    public void Clear()
    {
        building = null;
        icon.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        //buildingUI?.SelectBuilding(index);
        //buildingUI?.StartPlacingBuilding(); // 건축 모형 배치 시작
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (building == null) return;
        toolTip.SelectRecipe(building);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (building == null || toolTip.CheckSelected()) return;
        toolTip.gameObject.SetActive(true);
        toolTip.SetMouseOverRecipe(building);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (toolTip.gameObject.activeInHierarchy && !toolTip.CheckSelected())
        {
            toolTip.gameObject.SetActive(false);
        }
    }

}