using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryToolTip : MonoBehaviour
{
    public TextMeshProUGUI invenName;
    public TextMeshProUGUI invenDescription;
    public TextMeshProUGUI invenEffect;
    public TextMeshProUGUI invenCount;
    public Slider slider;
    public Button invenDropBtn;
    public Button invenUseBtn;
    public Button invenEquipBtn;
    public Button invenUnEquipBtn;
    public Transform invenEquipBtnGroup;
    public ItemSlot itemSlot;

    private ItemBundle item;
    
    private bool isSelectIcon = false;
    private bool isEquipped = false;
    private int selectedIdx;

    private void Start()
    {
        InitUI();
    }

    private void OnEnable()
    {
        SetBoxPosition();
    }

    private void OnDisable()
    {
        item = null;
        InitUI();
    }

    private void Update()
    {
        if (!isSelectIcon)
        {
            SetBoxPosition();            
        }
        ToggleEquipButton();
    }

    private void InitUI()
    {
        isSelectIcon = true;
        ToggleUI();
        isSelectIcon = false;
    }

    private void SetBoxPosition()
    {
        Vector2 mousePos = Input.mousePosition;
        transform.position = mousePos;
    }

    public void SetMouseOverItem(ItemBundle data)
    {
        item = data;
        slider.value = 1;
        UpdateUI();
    }

    public bool CheckSelected()
    {
        return isSelectIcon;
    }

    private void UpdateUI()
    {
        if (item.count == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        invenName.text = item.data.displayName;
        invenDescription.text = item.data.description;

        invenEffect.text = string.Empty;
        DisplayEffectText();
        slider.maxValue = item.count;
        invenCount.text = slider.value.ToString();       
    }

    private void DisplayEffectText()
    {
        switch (item.data.itemType)
        {
            case EItemType.EQUIPABLE:
                DisplayEquipableStatusText();
                break;
            case EItemType.CONSUMABLE:
                DisplayConsumableEffectText();
                break;
            case EItemType.BUFFABLE:
                break;
            case EItemType.RESOURCE:
                break;
        }
    }

    private void DisplayEquipableStatusText()
    {
        invenEffect.text += "공격력\t" + item.data.attackPoint.ToString("F0");
        invenEffect.text += "\n방어력\t" + item.data.defencePoint.ToString("F0");
        invenEffect.text += "\n손재주\t" + item.data.handiCraftPoint.ToString("F0");
    }

    private void DisplayConsumableEffectText()
    {
        foreach(var consum in item.data.consumables)
        {
            invenEffect.text += ConsumableTypeToString(consum.type) + "\t" + consum.value.ToString() + "\n";
        }
    }

    private string ConsumableTypeToString(EConsumableType type)
    {
        switch(type)
        {
            case EConsumableType.HUNGER:
                return "배고픔";
            case EConsumableType.HEALTH:
                return "체력";
        }
        return "";
    }


    public void SelectItem(ItemBundle data, int idx)
    {       
        if (data == item) ToggleUI();
        else if (data != item)
        {            
            SetMouseOverItem(data);
            SetBoxPosition();
        }        
        selectedIdx = idx;
        UpdateUI();
    }    

    public void DropButtonClick()
    {
        CharacterManager.Instance.Player.inventory.ThrowItem(selectedIdx, (int)slider.value);
        UpdateUI();
    }

    public void UseButtonClick()
    {
        // TODO : 아이템 사용
        CharacterManager.Instance.Player.inventory.UseItem(selectedIdx, (int)slider.value);
        UpdateUI();
    }

    public void EquipButtonClick()
    {
        // TODO : 장착
        CharacterManager.Instance.Player.inventory.EquipItem(selectedIdx);        
        UpdateUI();        
    }

    public void UnEquipButtonClick()
    {
        CharacterManager.Instance.Player.inventory.UnEquipItem(selectedIdx);        
        UpdateUI();        
    }

    public void ToggleEquipButton()
    {
        invenUnEquipBtn.gameObject.SetActive(itemSlot.equipped);
        invenEquipBtn.gameObject.SetActive(!itemSlot.equipped);
    }

   
    private void ToggleUI()
    {
        isSelectIcon = !isSelectIcon;
        invenCount.gameObject.SetActive(isSelectIcon);
        slider.gameObject.SetActive(isSelectIcon);           
        invenDropBtn.gameObject.SetActive(isSelectIcon);
        gameObject.GetComponentInChildren<Image>().raycastTarget = isSelectIcon;

        invenEquipBtnGroup.gameObject.SetActive(false);        
        invenUseBtn.gameObject.SetActive(false);

        if (item == null) return;        

        switch (item.data.itemType)
        {
            case EItemType.EQUIPABLE:
                invenEquipBtnGroup.gameObject.SetActive(isSelectIcon);
                break;
            case EItemType.CONSUMABLE:
                invenUseBtn.gameObject.SetActive(isSelectIcon);
                break;
            case EItemType.BUFFABLE:
                invenUseBtn.gameObject.SetActive(isSelectIcon);
                break;
            case EItemType.RESOURCE:
                break;
        }        
    }

    public void SliderValueChange()
    {
        //if (slider.value == 0) return;
        UpdateUI();
    }
}
