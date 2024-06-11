using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITabWindow : MonoBehaviour
{       
    public Toggle toggleInventory;
    public Toggle toggleCrafting;
    public Toggle toggleBuilding;

    private PlayerController controller;

    private void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        controller.inventory += ToggleInventory;
        controller.crafting += ToggleCrafting;
        controller.building += ToggleBuilding;
        gameObject.SetActive(false);        
    }
    
    public void ToggleInventory()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            toggleInventory.isOn = true;            
            controller.ToggleCursor();
        }
        else if (toggleInventory.isOn)
        {            
            gameObject.SetActive(false);
            toggleInventory.isOn = false;
            controller.ToggleCursor();
        }
        else
        {
            toggleInventory.isOn = true;
        }
    }

    public void ToggleCrafting()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            toggleCrafting.isOn = true;            
            controller.ToggleCursor();
        }
        else if (toggleCrafting.isOn)
        {
            gameObject.SetActive(false);
            toggleCrafting.isOn = false;            
            controller.ToggleCursor();
        }
        else
        {
            toggleCrafting.isOn = true;
        }
    }

    public void ToggleBuilding()
    {
        if(!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            toggleBuilding.isOn = true;
            controller.ToggleCursor();
        }
        else if (toggleBuilding.isOn)
        {
            gameObject.SetActive(false);
            toggleBuilding.isOn = false;
            controller.ToggleCursor();
        }
        else
        {
            toggleBuilding.isOn = true;
        }
    }
}
