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
    private float m_lastWanderTime;
    private bool m_isStanding;
    private float m_nextWanderTime;

    // Start is called before the first frame update
    void Start()
    {
        EntityStart();
        m_nextWanderTime = Random.Range(m_minWanderCooldown, m_maxWanderCooldown);
    }

    // Update is called once per frame
    void Update()
    {
        CheckWander();
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
    }
}
