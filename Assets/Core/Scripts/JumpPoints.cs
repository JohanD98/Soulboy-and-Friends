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
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if(!playerMovement.MovementStopped())
        {
            StartCoroutine(Jump(playerMovement));
        }
    }

    private IEnumerator Jump(PlayerMovement playerMovement)
    {
        float jumpTime = 0;
        playerMovement.ResetDestination();
        playerMovement.gameObject.GetComponent<Animator>().SetBool(m_bool_animationName, true);
        playerMovement.ResetFocus();
        playerMovement.StopAllMovement();
        playerMovement.transform.LookAt(new Vector3(m_target.position.x, this.transform.position.y ,m_target.position.z));
        while(jumpTime < m_timeToJump)
        {
            jumpTime += Time.deltaTime;
            playerMovement.gameObject.transform.position = new Vector3(Mathf.Lerp(this.transform.position.x,m_target.position.x, jumpTime/m_timeToJump), Mathf.Lerp(this.transform.position.y, m_target.position.y, jumpTime / m_timeToJump), Mathf.Lerp(this.transform.position.z, m_target.position.z, jumpTime / m_timeToJump));
            yield return null;
        }
        playerMovement.gameObject.GetComponent<Animator>().SetBool(m_bool_animationName, false);
        playerMovement.gameObject.transform.position = m_target.position;
        playerMovement.ResetFocus();
        playerMovement.AllowMovement();
        playerMovement.ResetDestination();
    }
}
