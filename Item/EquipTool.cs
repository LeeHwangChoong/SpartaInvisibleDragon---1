using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    private bool attacking;
    public float attackDistance;
    public float useStamina;

    [Header("Resources Gathering")]
    public bool doesGatherResources;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;

    [Header("Weapon Type")]
    public EWeaponType weaponType;

    public LayerMask layerMask;

    private Camera camera;
    private Player player;
        
    // Start is called before the first frame update
    void Start()
    {        
        camera = Camera.main;
        player = CharacterManager.Instance.Player;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            SoundManager.Instance.PlayAttackSound();
            attacking = true;
            Invoke("OnCanAttack", attackRate);
            Debug.Log("АјАн!");
            OnHit();
        }
    }

    private void OnCanAttack()
    {
        attacking = false;
    }

    public void OnHit()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Vector3 rayStart = transform.position - ray.origin;
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position + camera.transform.parent.localPosition, ray.direction.normalized, out hit, attackDistance, layerMask))
        {
            if (doesGatherResources && hit.collider.TryGetComponent(out Resource resource))
            {
                Debug.Log("1hit");
                resource.Gather(hit.point, hit.normal);
            }

            if (doesDealDamage && hit.collider.TryGetComponent(out IDamagable damagable))
            {
                Debug.Log("hit");
                damagable.TakePhysicalDamage(damage);
            }
        }
    }
}
