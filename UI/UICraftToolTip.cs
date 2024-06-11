using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICraftToolTip : MonoBehaviour
{
    public TextMeshProUGUI craftName;
    public TextMeshProUGUI craftDescription;
    public TextMeshProUGUI craftSource;
    public TextMeshProUGUI craftCount;
    public Slider slider;
    public Button craftBtn;

    public UIHandiFillBar uiHandiBar;

    private RecipeData recipe;

    private bool isSelectIcon = false;

    private Action CraftingAction;

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
        recipe = null;
        InitUI();
    }

    private void Update()
    {
        if (!isSelectIcon)
            SetBoxPosition();
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

    public void SetMouseOverRecipe(RecipeData data)
    {        
        recipe = data;
        slider.value = 1;
        UpdateUI();
    }

    public bool CheckSelected()
    {
        return isSelectIcon;
    }

    private void UpdateUI()
    {
        craftName.text = recipe.displayName;
        craftDescription.text = recipe.description;

        craftSource.text = string.Empty;        
        slider.maxValue = int.MaxValue;
        craftCount.text = slider.value.ToString();
        foreach (Source source in recipe.sources)
        {
            int numOfInventory = CharacterManager.Instance.Player.inventory.GetTotalItemAmount(source.prefabName);
            craftSource.text += source.displayName + $"\t{numOfInventory}/{source.count * (slider.value > 0 ? slider.value : 1)}\n"; // TODO : 인벤토리 내 갯수 추적 및 반영
            slider.maxValue = Mathf.Min(numOfInventory / source.count, slider.maxValue);
        }        
    }

    public void SelectRecipe(RecipeData data, Action craftingAction)
    {
        if (data == recipe) ToggleUI();
        else if (data != recipe)
        {
            SetMouseOverRecipe(data);
            SetBoxPosition();
        }
        CraftingAction = craftingAction;
        UpdateUI();
    }

    public void CraftingButtonClick()
    {
        StartCoroutine(CallCraftingAction());
    }

    public IEnumerator CallCraftingAction()
    {
        int count = (int)slider.value;
        for (int i = 0; i < count; i++)
        {
            uiHandiBar.gameObject.SetActive(true);
            uiHandiBar.InitUI(recipe.craftingTime);
            yield return new WaitForSeconds(recipe.craftingTime);                        
            CraftingAction?.Invoke();            
            slider.value--;     
        }        
    }

    private void ToggleUI()
    {
        isSelectIcon = !isSelectIcon;
        craftCount.gameObject.SetActive(isSelectIcon);
        slider.gameObject.SetActive(isSelectIcon);
        craftBtn.gameObject.SetActive(isSelectIcon);
        gameObject.GetComponentInChildren<Image>().raycastTarget = isSelectIcon;
    }

    public void SliderValueChange()
    {
        //if (slider.value == 0) return;
        UpdateUI();
    }
}
