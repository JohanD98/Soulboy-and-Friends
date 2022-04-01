using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] Camera m_cam;
    [SerializeField] GameObject m_player;
    [SerializeField] Vector3 m_cameraOffset;
    private Transform m_target;
    private bool m_isTargetingPlayer;
    private bool m_isFullyOnTarget;
    // Start is called before the first frame update
    void Start()
    {
        m_isTargetingPlayer = true;
        if(m_cam == null)
        {
            m_cam = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isTargetingPlayer)
        {
            m_cam.transform.position = m_player.transform.position + m_cameraOffset;
        }
        if(m_isFullyOnTarget && m_target != null)
        {
            m_cam.transform.position = m_target.position + m_cameraOffset;
        }
    }

    public void PanToTarget(Transform target, float timeToTarget)
    {
        m_isTargetingPlayer = false;
        StartCoroutine(IStartPanToTarget(target, timeToTarget));
    }

    public void ReturnToPlayer(float timeToReturn)
    {
        m_isTargetingPlayer = false;
        StartCoroutine(IReturnToPlayer(timeToReturn));
    }

    private IEnumerator IStartPanToTarget(Transform target, float timeToTarget)
    {
        StopAllCoroutines();
        m_isFullyOnTarget = false;
        m_target = target;
        float timePanned = 0;
        Vector3 startPos = m_cam.transform.position;
        while(timePanned < timeToTarget)
        {
            timeToTarget += Time.deltaTime;
            m_cam.transform.position = Vector3.Lerp(startPos, target.transform.position + m_cameraOffset, Mathf.Clamp01(timePanned / timeToTarget));
            yield return null;
        }
        m_isFullyOnTarget = true;

    }

    private IEnumerator IReturnToPlayer(float timeToPlayer)
    {
        StopAllCoroutines();
        m_isFullyOnTarget = false;
        float timePanned = 0;
        Vector3 startPos = m_cam.transform.position;
        while (timePanned < timeToPlayer)
        {
            timeToPlayer += Time.deltaTime;
            m_cam.transform.position = Vector3.Lerp(startPos, m_player.transform.position + m_cameraOffset, Mathf.Clamp01(timePanned / timeToPlayer));
            yield return null;
        }
        m_isTargetingPlayer = true;
    }
}
