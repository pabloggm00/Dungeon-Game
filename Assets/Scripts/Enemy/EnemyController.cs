using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IDamage
{
    public Transform target;
    public int hP;

    [Header("Attack Settings")]
    public float attackDistance;
    public float attackInterval;
    float attackTime;
    float distanceToTarget;

    [Header("Chase Settings")]
    public float distanceToChase;
    NavMeshAgent agent;

    public DetectSpawn detectSpawn;

    // Start is called before the first frame update
    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();
        attackTime = attackInterval;
    }

    // Update is called once per frame
    void Update()
    {
        
        //Vector3 posNoRot = new Vector3 (target.position.x, 0, target.position.z);
        transform.LookAt (target);

        distanceToTarget = Vector3.Distance (transform.position ,target.position);

        Chase();

        ControlAttack();
    }

    void ControlAttack()
    {
        attackTime -= Time.deltaTime;   

        if (attackTime < 0)
        {
            if (distanceToTarget < attackDistance)
            {
                attackTime = attackInterval;

                //Atacar
            }
        }
    }

    void Chase()
    {

        agent.SetDestination(target.position);
        
        
    }


    public void DoDamage(int damage, bool isPlayer)
    {
        if (isPlayer)
        {
            hP -= damage;

            if (hP <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        //Habrá que hacer un pull de enemigos
        Destroy(gameObject);
    }

    public bool IsInAreaPlayer()
    {
        return detectSpawn.IsInAreaEnemy();
    }

}
