using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float m_projectileSpeed;
    public float m_projectileRange;
    public Vector3 m_projectileStartPos;
    public Vector3 m_projectileDir;
    public LayerMask m_projectileLayerMask;
    public float m_projectileHeightFromGround;
    private float m_projectileDistanceCovered;
    private Vector3 m_projectileLastPos;

    // Start is called before the first frame update
    void Start()
    {
        m_projectileLastPos = m_projectileStartPos;
    }

    // Update is called once per frame
    void Update()
    {
        MoveProjectile();
    }

    void MoveProjectile()
    {
        m_projectileDistanceCovered += m_projectileSpeed * Time.deltaTime;
        if (m_projectileDistanceCovered >= m_projectileRange)
        {
            DestroyProjectile();
        }
        Vector3 pos = FindProjectileHeight(m_projectileLastPos + m_projectileDir * m_projectileSpeed * Time.deltaTime);
        m_projectileLastPos = pos;
        this.gameObject.transform.position = pos;
    }

    Vector3 FindProjectileHeight(Vector3 desiredPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(desiredPosition.x, desiredPosition.y + 10, desiredPosition.z), transform.up * -1, out hit, 100, m_projectileLayerMask))
        {
            Debug.Log(hit.transform.gameObject.name);
            return new Vector3(hit.point.x, hit.point.y + m_projectileHeightFromGround, hit.point.z);
        }
        else
        {
            return desiredPosition;
        }
    }

    void DestroyProjectile()
    {
        Destroy(this.gameObject);
    }
}
