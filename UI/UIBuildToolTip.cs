using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIBuildToolTip : MonoBehaviour
{
    public TextMeshProUGUI buildName;
    public TextMeshProUGUI buildDescription;
    public TextMeshProUGUI buildSource;
    public Button buildBtn;

    public GameObject uiTabWindow;

    private BuildData building;

    private Action BuildingAction;

    private bool isSelectIcon = false;

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
        building = null;
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

    public void SetMouseOverRecipe(BuildData data)
    {
        building = data;
        UpdateUI();
    }

    public bool CheckSelected()
    {
        return isSelectIcon;
    }

    private void UpdateUI()
    {
        buildName.text = building.displayName;
        buildDescription.text = building.description;

        buildSource.text = string.Empty;
        foreach (Source source in building.sources)
        {
            int numOfInventory = CharacterManager.Instance.Player.inventory.GetTotalItemAmount(source.prefabName);
            buildSource.text += source.displayName + $"\t{numOfInventory}/{source.count}\n"; // TODO : 인벤토리 내 갯수 추적 및 반영            
        }
    }

    public void SelectRecipe(BuildData data)
    {
        if (data == building) ToggleUI();
        else if (data != building)
        {
            SetMouseOverRecipe(data);
            SetBoxPosition();
        }
        //BuildingAction = buildingAction;
        UpdateUI();
    }

    public void BuildButtonClick()
    {
        //StartCoroutine(CallCraftingAction());
        BuildingManager.Instance.StartPlacingBuilding(building.prefabName);
        uiTabWindow.SetActive(false);
    }

    private void ToggleUI()
    {
        isSelectIcon = !isSelectIcon;
        buildBtn.gameObject.SetActive(isSelectIcon);
        gameObject.GetComponentInChildren<Image>().raycastTarget = isSelectIcon;
    }
}