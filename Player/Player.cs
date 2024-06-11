using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;
    public PlayerCondition condition;
    public Equipment equip;
    public Inventory inventory;   
    public Status status;

    //public ItemData itemData;
    public Action<ItemBundle> addItem;

    public Transform dropPosition;

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        equip = GetComponent<Equipment>();
        inventory = GetComponent<Inventory>();      
        status = GetComponent<Status>();
    }
}
