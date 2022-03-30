using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] NavMeshAgent m_agent;
    [SerializeField] float m_timeBetweenMoveCommands;
    [SerializeField] Camera m_cam;
    [SerializeField] LayerMask m_layerMask;
    private float m_timeSinceLastMoveCommand;
    // Start is called before the first frame update
    void Start()
    {
        if(m_cam == null)
        {
            m_cam = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_timeSinceLastMoveCommand += Time.deltaTime;
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
    }

    private void MoveCommandCalled()
    {
        m_timeSinceLastMoveCommand = 0;

        Ray ray = m_cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, m_layerMask))
        {
            m_agent.SetDestination(hit.point);
        }

    }
}
