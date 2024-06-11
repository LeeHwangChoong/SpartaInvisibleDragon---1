using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeSlot : MonoBehaviour, IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
    //, IDragHandler

{
    public RecipeData recipe;

    public Button button;
    public Image icon;    
    private Outline outline;

    public UICrafting crafting;
    public UICraftToolTip toolTip;

    public int index;    

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        toolTip.gameObject.SetActive(false);
    }

    public void Set()
    {
        icon.sprite = recipe.icon;
        icon.gameObject.SetActive(true);        
    }

    public void Clear()
    {
        recipe = null;
        icon.gameObject.SetActive(false);        
    }

    public void CraftingItem()
    {
        Transform dropPosition = CharacterManager.Instance.Player.inventory.dropPosition;
        foreach(var source in recipe.sources)
        {
            CharacterManager.Instance.Player.inventory.SubtractItem(source.prefabName, source.count);
        }
        ItemDataManager.Instance.CreateItemObject(recipe.prefabName, recipe.resultNumOfCraft, dropPosition.position);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (recipe == null) return;
        toolTip.SelectRecipe(recipe, CraftingItem);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (recipe == null || toolTip.CheckSelected()) return;
        toolTip.gameObject.SetActive(true);
        toolTip.SetMouseOverRecipe(recipe);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (toolTip.gameObject.activeInHierarchy && !toolTip.CheckSelected())
        {
            toolTip.gameObject.SetActive(false);
        }
    }
}