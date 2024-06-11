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
    [Range(0f, 1f)]public float dropRate; // ���� óġ �� ��� Ȯ��

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
    public string equipPrefab; // ��� ������ �������� �����ϱ� ����   
}