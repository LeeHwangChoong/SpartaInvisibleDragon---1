using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour, IDamagable, IHeatable
{   
    public UIConditions uiCondition;
    public DayNightCycle dayNightCycle;    
    public BuffUI buffUI;

    public Condition health { get { return uiCondition.health; } }
    public Condition hunger { get { return uiCondition.hunger; } }
    public Condition temperature { get { return uiCondition.temperature; } }
      
    private float noHungerHealthDecay = 3;
    private float coldHealthDecay = 4;    
    private bool isInvincibility;
    public bool isBoosted;
    private bool isAntiCold;
    private bool isPlayerNearHeatSource;

    public event Action onTakenDamage;

    Animator animator;

    public Transform respawnPoint;

    public bool isAlive { get; private set; } = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        dayNightCycle = FindObjectOfType<DayNightCycle>();
        HeatSource heatSource = FindObjectOfType<HeatSource>();

        if (heatSource != null)
        {
            heatSource.OnPlayerEnterHeatZone += HandleHeatZoneEvent;
        }

        buffUI = FindObjectOfType<BuffUI>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isAlive) return;

        hunger.Subtract(hunger.passiveValue * Time.deltaTime);
                
        HealthFromHunger();
        HealthFromCold();                
    }

    public void Heal(float amout)
    {
        if (!isAlive) return;
        health.Add(amout);
    }

    public void Eat(float amout)
    {
        if (!isAlive) return;
        hunger.Add(amout);
    }
        
    public void Die()
    {
        if (!isAlive) return;

        GetComponent<PlayerController>().enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero; // Rigidbody�� �ӵ��� �ʱ�ȭ
            rb.angularVelocity = Vector3.zero; // Angular velocity �ʱ�ȭ (ȸ����)
            rb.isKinematic = true; // Rigidbody�� Kinematic ���� ����
        }
        
        isAlive = false;
        animator.SetTrigger("Death");
        StartCoroutine(Respawn());                
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f); // 5�� �Ŀ� ������

        transform.position = respawnPoint.position; // ������ �������� �̵�
        RecoverStatus();
        isAlive = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // Rigidbody�� Kinematic ��� ����
        }

        GetComponent<PlayerController>().enabled = true;
        animator.SetTrigger("Respawn"); // ������ �ִϸ��̼� Ʈ����
    }

    private void RecoverStatus()
    {
        health.SetMaxValue(health.maxValue);
        hunger.SetMaxValue(hunger.maxValue);
        temperature.SetMaxValue(temperature.maxValue);

        health.Add(health.maxValue * 0.5f); // ü�� ���� ȸ��
        hunger.Add(hunger.maxValue * 0.5f); // ����� ���� ȸ��
        temperature.Add(temperature.maxValue * 0.5f); // �µ� ���� ȸ��
    }

    public void TakePhysicalDamage(int damage)
    {
        if (isInvincibility || !isAlive) return; // �׾��ų� ���� ������ ���� �������� ���� ����

        health.Subtract(damage);
        onTakenDamage?.Invoke();

        if (health.curValue <= 0)
        {
            Die();
        }
    }

    public void ApplyBuff(EBuffType buffType, float duration)
    {
        StartCoroutine(BuffCoroutine(buffType, duration));
    }

    private IEnumerator BuffCoroutine(EBuffType buffType, float duration)
    {
        switch (buffType)
        {
            case EBuffType.BOOST:
                isBoosted = true;
                buffUI?.ShowBoost(true);
                break;
            case EBuffType.INVINCIBILITY:
                isInvincibility = true;
                buffUI?.ShowInvincibility(true);
                break;
            case EBuffType.ANTICOLD:
                isAntiCold = true;
                buffUI?.ShowAntiCold(true);
                break;
        }

        yield return new WaitForSeconds(duration);

        switch (buffType)
        {
            case EBuffType.BOOST:
                isBoosted = false;
                buffUI?.ShowBoost(false);
                break;
            case EBuffType.INVINCIBILITY:
                isInvincibility = false;
                buffUI?.ShowInvincibility(false);
                break;
            case EBuffType.ANTICOLD:
                isAntiCold = false;
                buffUI?.ShowAntiCold(false);
                break;
        }
    }
    public void CanInvincibility(bool isActive)
    {
        isInvincibility = isActive;
    }

    private void HealthFromHunger()
    {
        if (hunger.curValue == 0f && !isInvincibility && isAlive)
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        if (health.curValue == 0f)
        {
            Die();
        }
    }

    private void HealthFromCold()
    {
        if (dayNightCycle.IsDayTime() || isPlayerNearHeatSource || isAntiCold)
        {
            temperature.Add(1 * Time.deltaTime);
        }
        else
        {
            temperature.Subtract(1 * Time.deltaTime);
        }

        if (temperature.curValue <= 0 && !isInvincibility && isAlive)
        {
            health.Subtract(coldHealthDecay * Time.deltaTime);
        }
    }

    public void IncreaseTemperature(float amount)
    {
        if (!isAlive) return;
        temperature.Add(amount);
    }

    public void SetMaxValue(float newMaxValue)
    {
        health.SetMaxValue(newMaxValue);
    }

    private void HandleHeatZoneEvent(bool isPlayerInHeatZone)
    {
        isPlayerNearHeatSource = isPlayerInHeatZone;
    }
}
