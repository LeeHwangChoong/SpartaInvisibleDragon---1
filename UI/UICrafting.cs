using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class UICrafting : MonoBehaviour
{
    public RecipeSlot[] slots;
    public RecipeData[] recipes;
   
    public Transform slotPanel;    
    
    private void Start()
    {              
        slots = new RecipeSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<RecipeSlot>();
            slots[i].index = i;
            slots[i].crafting = this;
        }
        recipes = CraftingManager.Instance.GetRecipes();
        UpdateUI();        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateUI()
    {
        for (int i = 0; i < recipes.Length; i++)
        {
            slots[i].recipe = recipes[i];            
            slots[i].Set();
        }
    }   
}
