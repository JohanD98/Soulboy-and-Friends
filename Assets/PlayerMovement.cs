using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] NavMeshAgent m_agent;
    [SerializeField] Camera m_cam;
    [SerializeField] Animator m_animator;
    [SerializeField] LayerMask m_layerMask;
    [SerializeField] float m_timeBetweenMoveCommands;
    [SerializeField] float m_dashCooldown;
    [SerializeField] float m_dashDistance;
    [SerializeField] float m_dashTime;
    private bool m_isDashing;
    private float m_timeSinceLastMoveCommand;
    private float m_timeSinceLastDash;
    // Start is called before the first frame update
    void Start()
    {
        m_timeSinceLastDash = m_dashCooldown;
        if(m_cam == null)
        {
            m_cam = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_timeSinceLastMoveCommand += Time.deltaTime;
        m_timeSinceLastDash += Time.deltaTime;
        if(Input.GetMouseButton(1))
        {
            if(m_timeSinceLastMoveCommand >= m_timeBetweenMoveCommands)
            {
                MoveCommandCalled();
            }
        }
        else if(Input.GetMouseButtonDown(1))
        {
            MoveCommandCalled();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Dash();
        }
        CheckAnimations();
    }

    private void CheckAnimations()
    {
        if(m_agent.remainingDistance > 0.2f)
        {
            m_animator.SetFloat("runSpeed", m_agent.speed);
        }
        else
        {
            m_animator.SetFloat("runSpeed", 0);
        }
    }

    private void MoveCommandCalled()
    {
        if(m_isDashing) { return; }

        m_timeSinceLastMoveCommand = 0;

        Ray ray = m_cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, m_layerMask))
        {
            m_agent.SetDestination(hit.point);
        }

    }

    private void Dash()
    {
        if(m_timeSinceLastDash >= m_dashCooldown)
        {
            m_agent.isStopped = true;
            m_timeSinceLastDash = 0;
            StartCoroutine(IDash());
            m_agent.ResetPath();
        }
    }

    private IEnumerator IDash()
    {
        m_isDashing = true;
        float dashTime = 0;
        Ray ray = m_cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
        }
        Vector3 startPos = m_agent.gameObject.transform.position;
        Vector3 endPos = m_agent.gameObject.transform.position += (hit.point - startPos).normalized * m_dashDistance;
        while(dashTime < m_dashTime)
        {
            dashTime += Time.deltaTime;
            m_agent.gameObject.transform.position = new Vector3(Mathf.Lerp(startPos.x, endPos.x, dashTime / m_dashTime),Mathf.Lerp(startPos.y, endPos.y, dashTime / m_dashTime),Mathf.Lerp(startPos.z, endPos.z, dashTime / m_dashTime));
            NavMeshHit navHit;
            if (!NavMesh.SamplePosition(m_agent.gameObject.transform.position, out navHit, .5f, NavMesh.AllAreas))
            {
                dashTime -= Time.deltaTime;
                m_agent.gameObject.transform.position = new Vector3(Mathf.Lerp(startPos.x, endPos.x, dashTime / m_dashTime), Mathf.Lerp(startPos.y, endPos.y, dashTime / m_dashTime), Mathf.Lerp(startPos.z, endPos.z, dashTime / m_dashTime));
                break;
            }
            yield return null;
        }
        m_agent.isStopped = false;
        m_isDashing = false;
    }
}
