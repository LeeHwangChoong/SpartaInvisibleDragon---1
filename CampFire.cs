using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class CampFire : MonoBehaviour
{
    public int damage;
    public float damageRate;    

    List<IDamagable> things = new List<IDamagable>();
    List<Coroutine> coroutines = new List<Coroutine>();   

    // Start is called before the first frame update

    void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate);    
    }
    
    private void DealDamage()
    {
        for(int i =0;i< things.Count; i++)
        {
            things[i].TakePhysicalDamage(damage);
        }
    }

    private IEnumerator DealDamage(IDamagable target)
    {
        while (true)
        {
            target.TakePhysicalDamage(damage);
            yield return new WaitForSecondsRealtime(damageRate);
        }        
    }
        
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamagable damagable))
        {            
            things.Add(damagable);
            Coroutine co = StartCoroutine(DealDamage(damagable));
            coroutines.Add(co);
        }                
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out IDamagable damagable))
        {
            int index = things.IndexOf(damagable);
            things.Remove(damagable);
            StopCoroutine(coroutines[index]);
            coroutines.RemoveAt(index);
        }                
    }
}
