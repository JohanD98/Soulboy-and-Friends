using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] public float radius;

    private bool m_isFocus = false;
    Transform m_player;

    public virtual void Interact()
    {
        Debug.Log("INTERACTED WITH[" + gameObject.name + "]");
    }

    public void OnFocused(Transform playerTransform)
    {
        m_isFocus = true;
        m_player = playerTransform;
        StartCoroutine(OnUpdate());
    }

    public void DeFocused()
    {
        m_isFocus = false;
        m_player = null;
    }

    private IEnumerator OnUpdate()
    {
        while(m_isFocus)
        {
            float distance = Vector3.Distance(m_player.position, transform.position);
            if(distance <= radius)
            {
                Interact();
                break;
            }
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(this.gameObject.transform.position, radius);
    }
}
