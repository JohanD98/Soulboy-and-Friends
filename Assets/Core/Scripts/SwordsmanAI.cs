using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanAI : Enemy
{

    [SerializeField] float m_strafeDistance;
    [SerializeField] float m_strafeChance;
    [SerializeField] float m_minStrafeTime;
    [SerializeField] float m_maxStrafeTime;
    [SerializeField] float m_doubleStrafeChance;
    [SerializeField] float m_doubleStrafeTime;
    [SerializeField] float m_timeStrafing;
    [SerializeField] float m_jumpTime;
    [SerializeField] float m_jumpHeight;
    [SerializeField] float m_jumpDistance;
    private float m_lastStrafe;
    private float m_nextStrafe;
    private bool m_isStrafing;

    private void Start()
    {
        EnemyStart();
    }

    private void Update()
    {
        EnemyUpdate();
        if(m_isFollowingPlayer)
        {
            if(Input.GetKeyDown(KeyCode.J))
            {
                JumpAttack();
            }
            else
            {
                UpdateWithPlayer();
            }
        }
    }

    private void UpdateWithPlayer()
    {
        HandleStrafe();
    }

    private void HandleStrafe()
    {
        m_lastStrafe += Time.deltaTime;
        if(m_lastStrafe > m_nextStrafe)
        {
            PerformStrafeAttempt();
        }
    }

    private void PerformStrafeAttempt()
    {
        m_lastStrafe = 0;
        m_nextStrafe = Random.Range(m_minStrafeTime, m_maxStrafeTime);
        if (Random.Range(0f, 1f) < m_doubleStrafeChance) { m_nextStrafe = m_doubleStrafeTime; }
        if(Random.Range(0f,1f) < m_strafeChance)
        {
            SuccessfulStrafe();
        }
    }

    private void SuccessfulStrafe()
    {
        m_isStrafing = true;
        int posOrNeg;
        if(Random.Range(0, 2) == 0)
        {
            posOrNeg = 1;
        }
        else
        {
            posOrNeg = -1;
        }
        Vector3 dir = (m_agent.transform.position - m_target.transform.position).normalized;
        dir = new Vector3(dir.z * -1, dir.y, dir.x);
        m_agent.SetDestination((posOrNeg * m_strafeDistance * dir) + m_target.transform.position);
        StartCoroutine(StrafeTimer());
    }

    private void JumpAttack()
    {
        Vector3 targetPos;
        if (Vector3.Distance(m_agent.transform.position,m_target.transform.position) < m_jumpDistance)
        {
            targetPos = m_target.transform.position;
        }
        else
        {
            targetPos = (m_agent.transform.position - m_target.transform.position).normalized * m_jumpDistance;
        }
        JumpAttackJump(targetPos);
    }

    private IEnumerator JumpAttackJump(Vector3 targetPos)
    {
        m_agent.autoTraverseOffMeshLink = false;
        Vector3 p0 = m_agent.transform.position;
        Vector3 p1 = new Vector3((m_agent.transform.position.x + targetPos.x/3), m_agent.transform.position.y + m_jumpHeight, m_agent.transform.position.z - targetPos.z/3);
        Vector3 p2 = new Vector3((m_agent.transform.position.x +  2* targetPos.x / 3), m_agent.transform.position.y + m_jumpHeight, m_agent.transform.position.z - 2 * targetPos.z / 3); ;
        Vector3 p3 = targetPos;
        float timeJumping = 0;
        m_movementAllowed = false;
        while (timeJumping < m_timeStrafing)
        {
            timeJumping += Time.deltaTime;
            m_agent.transform.position = CalculateCubicBezierPoint(timeJumping / m_jumpTime,p0,p1,p2,p3);
            yield return null;
        }

        m_movementAllowed = true;
        m_agent.autoTraverseOffMeshLink = true;
    }

    private Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    private IEnumerator StrafeTimer()
    {
        float timeStrafing = 0;
        m_movementAllowed = false;
        while(timeStrafing < m_timeStrafing)
        {
            timeStrafing += Time.deltaTime;
            yield return null;
        }

        m_movementAllowed = true;
    }


    private void OnDrawGizmosSelected()
    {
        Vector3 targetPos = new Vector3(0, 0, 100) + this.gameObject.transform.position;
        if (Vector3.Distance(this.gameObject.transform.position,targetPos) < m_jumpDistance)
        {
            Debug.Log("target in ranage");
        }
        else
        {
            Debug.Log("taget not in range");
            targetPos = this.gameObject.transform.position + (targetPos - this.gameObject.transform.position).normalized * m_jumpDistance;
        }
        Vector3 p0 = this.gameObject.transform.position;
        Vector3 p1 = new Vector3((this.gameObject.transform.position.x*2 + targetPos.x)/3, this.gameObject.transform.position.y + m_jumpHeight, (this.gameObject.transform.position.z*2 + targetPos.z)/3);
        Vector3 p2 = new Vector3((this.gameObject.transform.position.x + targetPos.x * 2)/3, this.gameObject.transform.position.y + m_jumpHeight, (this.gameObject.transform.position.z + targetPos.z *2) / 3); ;
        Vector3 p3 = targetPos;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(p0, 0.5f);
        Gizmos.DrawSphere(p1, 0.5f);
        Gizmos.DrawSphere(p2, 0.5f);
        Gizmos.DrawSphere(p3, 0.5f);
        Gizmos.color = Color.white;
        for (int i = 0; i < 25; i++)
        {
            float timeJumping = (float)i/25f;
            Gizmos.DrawSphere(CalculateCubicBezierPoint(timeJumping, p0, p1, p2, p3), .5f);
        }
    }
}
