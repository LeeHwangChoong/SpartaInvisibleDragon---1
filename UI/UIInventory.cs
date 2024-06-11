using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;
    public ItemBundle[] items;

    public GameObject inventoryWindow;
    public Transform slotPanel;   
   
    private void Start()
    {      
        slots = new ItemSlot[slotPanel.childCount];

        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
        }
        items = CharacterManager.Instance.Player.inventory.GetItems();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        items = CharacterManager.Instance.Player.inventory.GetItems();
        for (int i = 0; i < slots.Length; i++)
        {
            if(i < items.Length)
            {
                slots[i].item = items[i];
                slots[i].Set();
            }
            else slots[i].Clear();           
        }
        int idx = CharacterManager.Instance.Player.inventory.GetEquipIndex();
        slots[idx].equipped = CharacterManager.Instance.Player.inventory.CheckEquipped();
    }    

    //public void OnEquipButton()
    //{
    //    if (selectedItem != null && selectedItem.itemType == EItemType.EQUIPABLE)
    //    {
    //        if (curEquipIndex != -1)
    //        {
    //            UnEquip(curEquipIndex);
    //        }

    //        slots[selectedIndex].equipped = true;
    //        curEquipIndex = selectedIndex;
    //        EquipItem(selectedItem);
    //        UpdateUI();
    //    }
    //}

    //public void OnUnEquipButton()
    //{
    //    UnEquip(selectedIndex);
    //}

    //private void UnEquip(int index)
    //{
    //    slots[index].equipped = false;
    //    UnEquipItem();
    //    UpdateUI();
    //}

   

    //private void UnEquipItem()
    //{
    //    controller.UnEquipCurrentItem();
    //}
}
