using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingManager : Singleton<BuildingManager>
{
    private List<BuildData> buildings;  // 건축 데이터 배열
    private bool isPlacing;
    private GameObject buildingPreview;
    private BuildData selectedBuilding;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (instance == this)
                Destroy(Instance);
        }

        buildings = new List<BuildData>();
        LoadBuildings();
    }

    void Update()
    {
        if (isPlacing && buildingPreview != null)
        {
            UpdateBuildingPreview();
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (IsPlacementValid())
                {
                    PlaceBuilding();
                }
            }
        }
    }

    private void LoadBuildings()
    {
        int index = 0;
        string fileName = "Build";
        while (true)
        {
            index++;
            string path = "Assets/Scripts/ScriptableObject/Data/Build/" + fileName + index.ToString("D3") + ".asset";
            BuildData data = AssetDatabase.LoadAssetAtPath<BuildData>(path);
            if (data == null) break;
            buildings.Add(data);
        }
    }

    public BuildData[] GetBuildingDatas()
    {
        return buildings.ToArray();
    }

    public void StartPlacingBuilding(string prefabName)
    {
        selectedBuilding = buildings.Find(x => x.prefabName == prefabName);
        string path = "Assets/Prefabs/Build/" + prefabName + ".prefab";
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);        
        if (selectedBuilding != null)
        {
            isPlacing = true;
            buildingPreview = Instantiate(obj);
            buildingPreview.GetComponent<MeshCollider>().convex = false;
            SetLayerRecursively(buildingPreview, LayerMask.NameToLayer("Ignore Raycast"));
            SetColorRecursively(buildingPreview, Color.green);  // 프리뷰 색상을 녹색으로 설정
        }
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            buildingPreview.transform.position = hitInfo.point;

            if (IsPlacementValid())
            {
                SetColorRecursively(buildingPreview, Color.green); // 건축 가능
            }
            else
            {
                SetColorRecursively(buildingPreview, Color.red); // 건축 불가
            }
        }
    }

    private bool IsPlacementValid()
    {
        Vector3 position = buildingPreview.transform.position;
        Vector3 halfExtents = buildingPreview.GetComponent<Collider>().bounds.extents;

        Collider[] colliders = Physics.OverlapBox(position, halfExtents, buildingPreview.transform.rotation);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != buildingPreview && collider.gameObject.tag != "Ground")
            {
                return false; // 다른 객체와 겹치는 경우 및 ground가 아닌 경우
            }
        }
        return true; // 겹치지 않는 경우
    }

    private void PlaceBuilding()
    {
        if (selectedBuilding != null)
        {
            isPlacing = false;
            SetLayerRecursively(buildingPreview, 0);  // 기본 레이어로 변경
            SetColorRecursively(buildingPreview, Color.white);  // 색상을 원래대로 되돌림
            string path = "Assets/Prefabs/Build/" + selectedBuilding.prefabName + ".prefab";
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Instantiate(obj, buildingPreview.transform.position, Quaternion.identity);  // 실제 건축물 배치            
            CharacterManager.Instance.Player.controller.ToggleCursor();
            Destroy(buildingPreview);  // 프리뷰 삭제
            buildingPreview = null;
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child != null)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }

    private void SetColorRecursively(GameObject obj, Color color)
    {
        if (obj == null) return;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.color = color;
            }
        }
    }
}