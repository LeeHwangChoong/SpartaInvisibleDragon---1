using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "SOData/Recipe")]
public class RecipeData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string prefabName;
    public string description;
    public Source[] sources;  
    public float craftingTime;
    public int resultNumOfCraft;
    public Sprite icon;
}