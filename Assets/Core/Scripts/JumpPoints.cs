using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPoints : Interactable
{
    [SerializeField] Transform m_target;
    [SerializeField] float m_timeToJump;
    [SerializeField] string m_bool_animationName;
    public override void Interact()
    {
        base.Interact();
        StartCoroutine(Jump());
    }

    private IEnumerator Jump()
    {
        float jumpTime = 0;
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        playerMovement.gameObject.GetComponent<Animator>().SetBool(m_bool_animationName, true);
        playerMovement.StopAllMovement();
        while(jumpTime < m_timeToJump)
        {
            jumpTime += Time.deltaTime;
            playerMovement.gameObject.transform.position = new Vector3(Mathf.Lerp(this.transform.position.x,m_target.position.x, jumpTime/m_timeToJump), Mathf.Lerp(this.transform.position.y, m_target.position.y, jumpTime / m_timeToJump), Mathf.Lerp(this.transform.position.z, m_target.position.z, jumpTime / m_timeToJump));
            yield return null;
        }
        playerMovement.gameObject.GetComponent<Animator>().SetBool(m_bool_animationName, false);
        playerMovement.gameObject.transform.position = m_target.position;
        playerMovement.AllowMovement();
        playerMovement.ResetDestination();
    }
}
