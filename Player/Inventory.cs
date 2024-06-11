using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Inventory : MonoBehaviour
{
    public List<ItemBundle> items;
    private readonly int maxItemSlotAmount = 28;
    public Transform dropPosition;
    private Transform handTransform;

    private bool isEquipped = false;
    private int equipIdx;

    private void Start()
    {
        CharacterManager.Instance.Player.addItem += AddItem;
        handTransform = CharacterManager.Instance.Player.controller.animator.GetBoneTransform(HumanBodyBones.RightHand);
    }

    public ItemBundle[] GetItems()
    {
        return items.ToArray();
    }
    
    public int GetEquipIndex()
    {
        return equipIdx;
    }

    public void AddItem(ItemBundle newItem)
    {        
        foreach (ItemBundle item in items.Where(x => x.data.displayName == newItem.data.displayName)) // ���� �����ۿ� �����ֱ�
        {
            if (item.count == item.data.maxStackAmount || item.data.canStack == false) continue; // �̹� �ִ�ġ�� Ȥ�� ���úҰ��� ���� ����ã�� 
            if (item.count + newItem.count <= item.data.maxStackAmount) // ���� �������� ������ ȹ�淮�� ���� �ִ�ġ�� �Ѵ� ���
            {
                item.count += newItem.count;
                newItem.count = 0;
                break;
            }
            else
            {
                newItem.count -= item.data.maxStackAmount - item.count;
                item.count = item.data.maxStackAmount;                                
            }
        }
        if (newItem.count == 0) return;
        
        if (newItem.count > 0 && items.Count < maxItemSlotAmount) // �κ��丮������ ���� ���
        {    
            //if (newItem.Try)
            items.Add(newItem);
        }
        else // �ƴ� ���
        {
            ItemDataManager.Instance.CreateItemObject(newItem.data.name, newItem.count, dropPosition.position);
        }
    }

    public void ThrowItem(int idx, int count)
    {
        if(count == 0) return;
        items[idx].count -= count;
        ItemDataManager.Instance.CreateItemObject(items[idx].data.name, count, dropPosition.position);
        if (items[idx].count == 0) items.RemoveAt(idx);
    }

    public void UseItem(int idx, int count)
    {
        if (items[idx].data.itemType == EItemType.CONSUMABLE)
        {
            for (int i = 0; i < items[idx].data.consumables.Length; i++)
            {
                switch (items[idx].data.consumables[i].type)
                {
                    case EConsumableType.HEALTH:
                        CharacterManager.Instance.Player.condition.Heal(items[idx].data.consumables[i].value * count);
                        break;
                    case EConsumableType.HUNGER:
                        CharacterManager.Instance.Player.condition.Eat(items[idx].data.consumables[i].value * count);
                        break;
                }
            }            
        }

        else if (items[idx].data.itemType == EItemType.BUFFABLE)
        {
            switch (items[idx].data.buffType)
            {
                case EBuffType.BOOST:                    
                    CharacterManager.Instance.Player.controller.ApplyBuff(EBuffType.BOOST, items[idx].data.duration * count);
                    break;
                case EBuffType.INVINCIBILITY:                    
                    CharacterManager.Instance.Player.controller.ApplyBuff(EBuffType.INVINCIBILITY, items[idx].data.duration * count);
                    break;
                case EBuffType.ANTICOLD:                    
                    CharacterManager.Instance.Player.controller.ApplyBuff(EBuffType.ANTICOLD, items[idx].data.duration * count);
                    break;                
            }
        }

        items[idx].count -= count;
        if (items[idx].count == 0) items.RemoveAt(idx);
    }

    public void EquipItem(int idx)
    {      
            // �θ� ���� (���� Ʈ������ ����)            
            isEquipped = true;
            equipIdx = idx;
            ItemData item = items[idx].data;
            GameObject equippedItem = ItemDataManager.Instance.CreateEquipItem(item.equipPrefab, handTransform);

            //// ��ġ�� ȸ���� ����
            //if (item.weaponTransform != null)
            //{
            //    equippedItem.transform.localPosition = item.weaponTransform.position;
            //    equippedItem.transform.localRotation = Quaternion.Euler(item.weaponTransform.rotation);
            //}
            //else
            //{
            //    equippedItem.transform.localPosition = Vector3.zero;
            //    equippedItem.transform.localRotation = Quaternion.identity;
            //}

            //// �θ� ���� (���� Ʈ������ ����)
            //equippedItem.transform.SetParent(handTransform, false);

            CharacterManager.Instance.Player.controller.SetEquippedItem(equippedItem);

            EquipTool equipTool = equippedItem.GetComponent<EquipTool>();
            if (equipTool != null)
            {
                CharacterManager.Instance.Player.status.EquipItem(equipTool);
            }
        
        
    }

    public void UnEquipItem(int idx)
    {
        isEquipped = false;
        CharacterManager.Instance.Player.controller.UnEquipCurrentItem();
    }

    public bool CheckEquipped()
    {
        return isEquipped;
    }

    public int GetTotalItemAmount(string itemName)
    {
        int total = 0;
        foreach (var item in items.Where(x => x.data.name == itemName))
        {
            total += item.count;
        }

        return total;
    }

    public void SubtractItem(string itemName, int count)
    {
        foreach(var item in items.Where(x => x.data.name == itemName))
        {
            if(item.count > count)
            {
                item.count -= count;
                break;
            }
            else
            {
                count -= item.count;
                item.count = 0;               
            }
        }
        items.RemoveAll(x => x.count == 0);
    }
}
