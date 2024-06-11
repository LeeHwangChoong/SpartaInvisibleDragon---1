using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIBuilding : MonoBehaviour
{
    public BuildingSlot[] slots;
    public BuildData[] buildings;

    public GameObject buildingWindow;
    public Transform slotPanel;
    public Transform dropPosition;

    private PlayerController controller;
    private PlayerCondition condition;

    private BuildData selectedBuilding;
    private int selectedIndex;

    private GameObject buildingPreview;
    private bool isPlacing;

    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        slots = new BuildingSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<BuildingSlot>();
            slots[i].index = i;
            slots[i].buildingUI = this;
        }
        buildings = BuildingManager.Instance.GetBuildingDatas();
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < buildings.Length; i++)
        {
            slots[i].building = buildings[i];
            slots[i].Set();
        }
    }

    

    public void SelectBuilding(int index)
    {
        if (index >= 0 && index < buildings.Length)
        {
            selectedBuilding = buildings[index];
            selectedIndex = index;
        }
    }

       
}