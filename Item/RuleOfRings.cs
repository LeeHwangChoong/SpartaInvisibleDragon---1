using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleOfRings : ItemObject
{
    public void CanLookInvisibility()
    {
        Camera.main.cullingMask |= LayerMask.GetMask("Invisibility");
    }
}
