using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wandering,
    Attacking,
    Fleeing
}

public class Monster : MonoBehaviour, IDamagable
{
    public MonsterData monsterData;

    private AIState aiState;
    private float lastAttackTime;
    private float playerDistance;

    private NavMeshAgent agent;
    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Agent is not on NavMesh at start");
            return;
        }
        SetState(AIState.Wandering);
    }

    private void Update()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("Agent is not on NavMesh in Update");
            HandleAgentOffNavMesh();
            return;
        }

        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);

        animator.SetBool("Moving", aiState != AIState.Idle);

        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
            case AIState.Fleeing:
                FleeingUpdate();
                break;
        }
    }

    private void SetState(AIState state)
    {
        aiState = state;

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Agent is not on NavMesh in SetState");
            return;
        }

        switch (aiState)
        {
            case AIState.Idle:
                agent.speed = monsterData.walkSpeed;
                agent.isStopped = true;
                break;
            case AIState.Wandering:
                agent.speed = monsterData.walkSpeed;
                agent.isStopped = false;
                WanderToNewLocation();
                break;
            case AIState.Attacking:
                agent.speed = monsterData.runSpeed;
                agent.isStopped = false;
                break;
            case AIState.Fleeing:
                agent.speed = monsterData.runSpeed;
                agent.isStopped = false;
                break;
        }

        animator.speed = agent.speed / monsterData.walkSpeed;
    }

    private void PassiveUpdate()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Agent is not on NavMesh in PassiveUpdate");
            return;
        }

        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(monsterData.minWanderWaitTime, monsterData.maxWanderWaitTime));
        }

        if (playerDistance < monsterData.detectDistance)
        {
            SetState(AIState.Attacking);
        }
    }

    private void AttackingUpdate()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Agent is not on NavMesh in AttackingUpdate");
            return;
        }

        if (playerDistance > monsterData.attackDistance || !IsPlayerInFieldOfView())
        {
            agent.isStopped = false;
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
            {
                agent.SetDestination(CharacterManager.Instance.Player.transform.position);
            }
            else
            {
                SetState(AIState.Fleeing);
            }
        }
        else
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime > monsterData.attackRate)
            {
                lastAttackTime = Time.time;
                CharacterManager.Instance.Player.controller.GetComponent<IDamagable>().TakePhysicalDamage(monsterData.damage);
                animator.speed = 1;
                animator.SetTrigger("Attack");
            }
        }
    }

    private void FleeingUpdate()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Agent is not on NavMesh in FleeingUpdate");
            return;
        }

        if (agent.remainingDistance < 0.1f)
        {
            agent.SetDestination(GetFleeLocation());
        }
        else
        {
            SetState(AIState.Wandering);
        }
    }

    private void WanderToNewLocation()
    {
        if (aiState != AIState.Idle)
        {
            return;
        }
        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    private bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < monsterData.fieldOfView * 0.5f;
    }

    private Vector3 GetFleeLocation()
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * monsterData.safeDistance), out hit, monsterData.maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (GetDestinationAngle(hit.position) > 90 || playerDistance < monsterData.safeDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * monsterData.safeDistance), out hit, monsterData.maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }

        return hit.position;
    }

    private Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(monsterData.minWanderDistance, monsterData.maxWanderDistance)), out hit, monsterData.maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (Vector3.Distance(transform.position, hit.position) < monsterData.detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(monsterData.minWanderDistance, monsterData.maxWanderDistance)), out hit, monsterData.maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }

        return hit.position;
    }

    private float GetDestinationAngle(Vector3 targetPos)
    {
        return Vector3.Angle(transform.position - CharacterManager.Instance.Player.transform.position, transform.position + targetPos);
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        monsterData.health -= damageAmount;
        if (monsterData.health <= 0)
            Die();

        StartCoroutine(DamageFlash());
    }

    private void Die()
    {
        // 사망 처리 로직 (예: 사운드 재생, 애니메이션 트리거 등)
        DropItem();
        // NPC 오브젝트 삭제
        Destroy(gameObject);
    }

    private void DropItem()
    {
        foreach (var item in monsterData.dropOnDeath)
        {
            float rate = Random.Range(0f, 1f);
            if (rate <= item.dropRate)
            {
                ItemDataManager.Instance.CreateItemObject(item.name, 1, transform.position);
            }
        }        
    }

    private IEnumerator DamageFlash()
    {
        for (int x = 0; x < meshRenderers.Length; x++)
            meshRenderers[x].material.color = new Color(1.0f, 0.6f, 0.6f);

        yield return new WaitForSeconds(0.1f);
        for (int x = 0; x < meshRenderers.Length; x++)
            meshRenderers[x].material.color = Color.white;
    }

    private void HandleAgentOffNavMesh()
    {
        // 에이전트가 NavMesh에 없을 때 처리하는 로직 (예: 재배치 시도, 상태 전환 등)
        // 여기서는 로그만 출력하고, 필요에 따라 적절한 처리를 추가할 수 있습니다.
        Debug.LogWarning("Agent is off NavMesh. Handle accordingly.");
    }
}