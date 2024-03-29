using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State
    {
        Idle,
        Chasing,
        Attacking,
    };
    State currentState;

    public ParticleSystem deathEffect;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;

    Material skinMaterial;

    Color originalColor;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1;
    float damage = 1f;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget = false;

    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindWithTag("Player") != null)
        {
            hasTarget = true;

            target = GameObject.FindWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }
    protected override void Start()
    {
        base.Start();

        if (hasTarget)
        {
            currentState = State.Chasing;

            targetEntity.OnDeath += OnTargetDeath;

            StartCoroutine(UpdatePath());

        }
    }

    public void SetCharacteristics(float moveSpeed, int hitsTokKillPlayer, float enemyHealth, Color skinColor)
    {
        pathfinder.speed = moveSpeed;

        if (hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsTokKillPlayer);
        }
        startingHealth = enemyHealth;

        skinMaterial = transform.Find("Graphics").gameObject.GetComponent<Renderer>().sharedMaterial;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= health)
        {
            Destroy( Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.curveMultiplier);
        }
        base.TakeHit(damage, hitPoint, hitDirection);

    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;

                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine (Attack());
                }

            }

        }
    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-percent * percent + percent) * 4; // -4x^2 + 4x
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while (hasTarget)
        {
            if (currentState == State.Chasing) 
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                if (!dead)
                { 
                    pathfinder.SetDestination(targetPosition);
                }
            }

            yield return new WaitForSeconds(refreshRate);
        }
    }
}
