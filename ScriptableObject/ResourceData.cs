using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "SOData/Resource")]
public class ResourceData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public int capacity;
    public EResourceType resourceType;

    [Header("Crops")]
    public int growthGrade;
    public int maxGrowthGrade;
    public float growthInterval;
    public GameObject [] ResourcePrefabs;

    [Header("Respawnable")]
    public float respawnTime;
    public int QuantityPerHit;
}