using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Dragon : MonoBehaviour
{
    [Header("Stats")]
    public int health = 100;
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float detectDistance = 10f;
    public float safeDistance = 15f;

    [Header("Wandering")]
    public float minWanderDistance = 5f;
    public float maxWanderDistance = 20f;
    public float minWanderWaitTime = 2f;
    public float maxWanderWaitTime = 5f;

    [Header("Combat")]
    public int damage = 10;
    public float attackRate = 1f;
    public float attackDistance = 2f;
    public float fieldOfView = 120f;

    [Header("Drop Items")]
    public ItemData[] dropOnDeath;

    private Animator animator;
    private NavMeshAgent agent;
    private float lastAttackTime;
    private float playerDistance;
    private SkinnedMeshRenderer[] meshRenderers;

    private int IdleSimple;
    private int IdleAgressive;
    private int IdleRestless;
    private int Walk;
    private int BattleStance;
    private int Bite;
    private int Drakaris;
    private int FlyingFWD;
    private int FlyingAttack;
    private int Hover;
    private int Lands;
    private int TakeOff;
    private int DieHash;

    private enum AIState
    {
        Idle,
        Wandering,
        Attacking,
        Fleeing
    }

    private AIState aiState;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        IdleSimple = Animator.StringToHash("IdleSimple");
        IdleAgressive = Animator.StringToHash("IdleAgressive");
        IdleRestless = Animator.StringToHash("IdleRestless");
        Walk = Animator.StringToHash("Walk");
        BattleStance = Animator.StringToHash("BattleStance");
        Bite = Animator.StringToHash("Bite");
        Drakaris = Animator.StringToHash("Drakaris");
        FlyingFWD = Animator.StringToHash("FlyingFWD");
        FlyingAttack = Animator.StringToHash("FlyingAttack");
        Hover = Animator.StringToHash("Hover");
        Lands = Animator.StringToHash("Lands");
        TakeOff = Animator.StringToHash("TakeOff");
        DieHash = Animator.StringToHash("Die");
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
            return;
        }

        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);

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
                agent.speed = walkSpeed;
                agent.isStopped = true;
                animator.SetTrigger(IdleSimple);
                break;
            case AIState.Wandering:
                agent.speed = walkSpeed;
                agent.isStopped = false;
                animator.SetTrigger(Walk);
                WanderToNewLocation();
                break;
            case AIState.Attacking:
                agent.speed = runSpeed;
                agent.isStopped = false;
                animator.SetTrigger(BattleStance);
                break;
            case AIState.Fleeing:
                agent.speed = runSpeed;
                agent.isStopped = false;
                animator.SetTrigger(FlyingFWD);
                break;
        }

        animator.speed = agent.speed / walkSpeed;
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
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        if (playerDistance < detectDistance)
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

        if (playerDistance > attackDistance || !IsPlayerInFieldOfView())
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
            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;
                CharacterManager.Instance.Player.controller.GetComponent<IDamagable>().TakePhysicalDamage(damage);
                animator.speed = 1;
                animator.SetTrigger(Bite);
            }
        }

        // 플레이어와 드래곤이 부딪혔을 때 드래곤이 공격하도록 설정
        if (Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position) < agent.radius)
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;
                CharacterManager.Instance.Player.controller.GetComponent<IDamagable>().TakePhysicalDamage(damage);
                animator.speed = 1;
                animator.SetTrigger(Bite);
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
        return angle < fieldOfView * 0.5f;
    }

    private Vector3 GetFleeLocation()
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * safeDistance), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (GetDestinationAngle(hit.position) > 90 || playerDistance < safeDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * safeDistance), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }

        return hit.position;
    }

    private Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
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
        health -= damageAmount;
        if (health <= 0)
            Die();

        StartCoroutine(DamageFlash());
    }

    private void Die()
    {
        animator.SetTrigger(DieHash);
        // 사망 애니메이션 재생 후 객체 삭제
        StartCoroutine(RemoveAfterDeathAnimation());
    }

    private IEnumerator RemoveAfterDeathAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

    private IEnumerator DamageFlash()
    {
        for (int x = 0; x < meshRenderers.Length; x++)
            meshRenderers[x].material.color = new Color(1.0f, 0.6f, 0.6f);
        yield return new WaitForSeconds(0.1f);
        for (int x = 0; x < meshRenderers.Length; x++)
            meshRenderers[x].material.color = Color.white;
    }
}