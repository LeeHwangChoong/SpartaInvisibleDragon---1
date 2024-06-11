using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Build Data")]
public class BuildData : ScriptableObject
{
    public string displayName;
    public string prefabName;
    public string description;
    public Source[] sources;
    public float craftingTime;    
    public Sprite icon;      
}