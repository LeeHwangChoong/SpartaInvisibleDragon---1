using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ItemDataManager : Singleton<ItemDataManager>
{   
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
    }

    public void CreateItemObject(string prefabName, int count, Vector3 position)
    {
        string path = "Assets/Prefabs/Item/" + prefabName + ".prefab";
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        GameObject newObj = Instantiate(obj, position, Quaternion.identity);
        newObj.name = prefabName;
        newObj.GetComponent<ItemObject>().item.count = count;
    }

    public GameObject CreateEquipItem(string prefabName, Transform parent)
    {
        string path = "Assets/Prefabs/Equip/" + prefabName + ".prefab";
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        GameObject newObj = Instantiate(obj, Vector3.zero, Quaternion.identity);
        newObj.name = prefabName;
        newObj.transform.parent = parent;
        newObj.transform.localPosition = Vector3.zero;
        newObj.transform.localRotation = Quaternion.identity;
        return newObj; 
    }
}
