using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatSource : MonoBehaviour
{
    public float heatRate = 0.5f;
    public delegate void HeatZoneEventHandler(bool isPlayerInHeatZone);
    public event HeatZoneEventHandler OnPlayerEnterHeatZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {            
            OnPlayerEnterHeatZone?.Invoke(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {            
            OnPlayerEnterHeatZone?.Invoke(false);
        }
    }
}