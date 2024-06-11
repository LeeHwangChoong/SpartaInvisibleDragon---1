using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
//, IDragHandler
{
    public ItemBundle item;

    public Button button;
    public Image icon;
    public TextMeshProUGUI quantityText;
    private Outline outline;

    public UIInventory inventory;
    public UIInventoryToolTip toolTip;

    public int index;
    public bool equipped = false;
    public int quantity;


    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        outline.enabled = equipped;
    }

    private void OnDisable()
    {
        toolTip.gameObject.SetActive(false);
    }

    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.data.icon;
        quantityText.text = item.count > 1 ? item.count.ToString() : string.Empty;
        equipped = false;
        if(outline != null )
        {
            outline.enabled = equipped;
        }
    }

    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
    } 

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null) return;
        toolTip.itemSlot = this;
        toolTip.SelectItem(item, index);        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null || toolTip.CheckSelected()) return;
        toolTip.gameObject.SetActive(true);
        toolTip.itemSlot = this;
        toolTip.SetMouseOverItem(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (toolTip.gameObject.activeInHierarchy && !toolTip.CheckSelected())
        {
            toolTip.itemSlot = null;
            toolTip.gameObject.SetActive(false);
        }
    }
}
