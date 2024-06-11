using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "SOData/Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public EItemType itemType;
    public Sprite icon;
    public float weight;
    public GameObject dropPrefab;
    [Range(0f, 1f)]public float dropRate; // 몬스터 처치 시 드랍 확률

    [Header("Stacking")]
    public bool canStack;    
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

    [Header("Equip")]
    public float attackPoint;
    public float defencePoint;
    public float handiCraftPoint;

    [Header("Buffable")]
    public EBuffType buffType;
    public float duration;

    [Header("Prefab")] 
    public string equipPrefab; // 장비 아이템 프리팹을 저장하기 위해   
}