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

    private Interactable m_focus;

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
        if(m_focus != null)
        {
            m_agent.SetDestination(m_focus.transform.position);
            LookAtTarget();
        }
        CheckAnimations();
    }

    private void CheckAnimations()
    {
        if(m_isDashing)
        {
            //Dash animation
        }
        if (!m_agent.isStopped && m_agent.hasPath)
        {
            m_animator.SetFloat("moveSpeed", m_agent.speed);
        }
        else
        {
            m_animator.SetFloat("moveSpeed", 0);
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
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if(interactable != null)
            {
                SetFocus(interactable);
            }
            else
            {
                ResetFocus();
                m_agent.SetDestination(hit.point);
            }
        }

    }

    private void SetFocus(Interactable interactable)
    {

        if(interactable != m_focus)
        {
            if(m_focus != null)
            {
                m_focus.DeFocused();
            }
            m_focus = interactable;
            m_agent.stoppingDistance = m_focus.radius * .8f;
            m_agent.updateRotation = false;
        }

        interactable.OnFocused(this.transform);
    }

    private void ResetFocus()
    {
        if(m_focus != null)
        {
            m_focus.DeFocused();
        }
        m_agent.stoppingDistance = 0;
        m_agent.updateRotation = true;
        m_focus = null;
        
    }

    private void InteractWithInteractable()
    {

    }

    private void LookAtTarget()
    {
        Vector3 dir = (m_focus.transform.position - m_agent.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = lookRotation;
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
        Vector3 endPos = m_agent.gameObject.transform.position + (new Vector3(hit.point.x - startPos.x, 0, hit.point.z - startPos.z)).normalized * m_dashDistance;
        Vector3 dir = (startPos - endPos).normalized;
        m_agent.gameObject.transform.LookAt(new Vector3(endPos.x, 0, endPos.z));
        while(dashTime < m_dashTime)
        {
            dashTime += Time.deltaTime;
            m_agent.gameObject.transform.position = new Vector3(Mathf.Lerp(startPos.x, endPos.x, dashTime / m_dashTime),0,Mathf.Lerp(startPos.z, endPos.z, dashTime / m_dashTime));
            NavMeshHit navHit;
            if (!NavMesh.SamplePosition(m_agent.gameObject.transform.position, out navHit, .5f, NavMesh.AllAreas))
            {
                dashTime -= Time.deltaTime;
                m_agent.gameObject.transform.position = new Vector3(Mathf.Lerp(startPos.x, endPos.x, dashTime / m_dashTime), 0, Mathf.Lerp(startPos.z, endPos.z, dashTime / m_dashTime));
                break;
            }
            yield return null;
        }
        m_agent.isStopped = false;
        m_isDashing = false;
    }
}
