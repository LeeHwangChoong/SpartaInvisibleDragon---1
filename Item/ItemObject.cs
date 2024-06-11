using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemBundle item;

    public string GetInteractPrompt()
    {
        string str = $"{item.data.displayName}\n{item.data.description}";
        return str;
    }

    public void OnInteract()
    {
        //CharacterManager.Instance.Player.itemData = data;
        //ItemDataManager.Instance.CreateItemObject(data.name, count, transform.position);
        CharacterManager.Instance.Player.addItem?.Invoke(item);
        Destroy(gameObject);  
    }
}
