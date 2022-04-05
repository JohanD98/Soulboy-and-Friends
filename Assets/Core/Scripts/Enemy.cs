using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    [SerializeField] NavMeshAgent m_agent;
    [SerializeField] Vector3 m_wanderPos;
    [SerializeField] float m_wanderRadius;
    [SerializeField] float m_minWanderCooldown;
    [SerializeField] float m_maxWanderCooldown;
    [SerializeField] float m_detectionRadius;
    [SerializeField] float m_alertRadius;
    [SerializeField] LayerMask m_playerMask;
    [SerializeField] LayerMask m_enemyMask;
    private float m_lastWanderTime;
    private bool m_isStanding;
    private float m_nextWanderTime;
    [SerializeField] Player m_target;
    private bool m_isFollowingPlayer;
    private float m_timeSincePlayerLastFound;
    [SerializeField] float m_timeUntilLostIntrest;
    [SerializeField] float m_checkForPlayerCooldown;
    [SerializeField] float m_attackRange;

    // Start is called before the first frame update
    void Start()
    {
        EntityStart();
        m_nextWanderTime = Random.Range(m_minWanderCooldown, m_maxWanderCooldown);
        StartCoroutine(CoCheckPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isFollowingPlayer)
        {
            FollowPlayer();
        }
        else
        {
            CheckWander();
        }
    }

    private IEnumerator CoCheckPlayer()
    {
        while(true)
        {
            CheckPlayer();
            yield return new WaitForSeconds(m_checkForPlayerCooldown);
        }
    }

    public void CheckPlayer()
    {
        if (Vector3.Distance(m_target.transform.position, m_agent.transform.position) < m_detectionRadius)
        {
            FoundPlayer();
            m_timeUntilLostIntrest = 0;
        }
        else
        {
            m_timeSincePlayerLastFound += m_checkForPlayerCooldown;
            if(m_timeSincePlayerLastFound >= m_timeUntilLostIntrest)
            {
                LostPlayer();
            }
        }
    }

    private void FoundPlayer()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(m_agent.transform.position, m_alertRadius, m_enemyMask);

        foreach (Collider enemy in hitEnemies)
        {
            Enemy foundEnemy = enemy.GetComponent<Enemy>();
            if(foundEnemy != this)
            {
                foundEnemy.FoundPlayer();
            }
        }
        m_agent.stoppingDistance = m_attackRange;
        m_isFollowingPlayer = true;
    }

    private void LostPlayer()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(m_agent.transform.position, m_alertRadius, m_enemyMask);

        foreach (Collider enemy in hitEnemies)
        {
            Enemy foundEnemy = enemy.GetComponent<Enemy>();
            if (foundEnemy != this)
            {
                foundEnemy.LostPlayer();
            }
        }
        m_agent.stoppingDistance = 0;
        m_isFollowingPlayer = false;
        Wander();
    }

    private void FollowPlayer()
    {
        if(Vector3.Distance(m_target.transform.position, m_agent.transform.position) < m_attackRange)
        {
            //Attack
        }
        m_agent.SetDestination(m_target.transform.position);
    }

    private void CheckWander()
    {
        if(m_isStanding)
        {
            m_lastWanderTime += Time.deltaTime;
            if(m_lastWanderTime >= m_nextWanderTime)
            {
                Wander();
            }
        }
        else if(!m_agent.hasPath)
        {
            m_isStanding = true;
        }

    }

    private void Wander()
    {
        m_lastWanderTime = 0;
        m_nextWanderTime = Random.Range(m_minWanderCooldown, m_maxWanderCooldown);
        m_agent.SetDestination(Random.insideUnitSphere*m_wanderRadius + m_wanderPos);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(m_wanderPos, m_wanderRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_wanderPos, 0.25f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(m_agent.transform.position, m_detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_agent.transform.position, m_alertRadius);
    }
}
