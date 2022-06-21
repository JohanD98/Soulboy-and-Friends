using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] Animator m_animator;
    [SerializeField] LayerMask m_enemyLayers;
    [SerializeField] Transform m_basicAttackPoint;
    [SerializeField] float m_moveStopTime;
    [SerializeField] float m_bAttackHitDelay;
    [SerializeField] float m_coolDownTimeBA;
    [SerializeField] float m_basicAttackRange;
    [SerializeField] float m_damage;
    private float m_coolUpTimeBA;
    private Coroutine m_lastCoroutine;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > m_coolUpTimeBA)
        {
            BasicAttack();
            m_coolUpTimeBA = Time.time + m_coolDownTimeBA;
        }   
    }

    void BasicAttack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            this.gameObject.transform.LookAt(new Vector3(hit.point.x, this.transform.position.y, hit.point.z));
        }

        m_animator.SetTrigger("basicAttack");

        if(m_lastCoroutine != null)
        {
            StopCoroutine(m_lastCoroutine);
        }
        m_lastCoroutine = StartCoroutine(PerformBasicAttack());
    }

    IEnumerator PerformBasicAttack()
    {

        float moveStopTimeRemaining = m_moveStopTime;
        float bAttackHitDelayRemaining = m_bAttackHitDelay;
        bool hasAppliedDmg = false;
        bool hasStartedMovement = false;
        this.gameObject.GetComponent<PlayerMovement>().StopAllMovement();

        while (moveStopTimeRemaining > 0 || bAttackHitDelayRemaining > 0)
        {
            moveStopTimeRemaining -= Time.deltaTime;
            bAttackHitDelayRemaining -= Time.deltaTime;

            if (moveStopTimeRemaining <= 0 && !hasStartedMovement)
            {
                this.gameObject.GetComponent<PlayerMovement>().AllowMovement();

                hasStartedMovement = true;
            }

            if (bAttackHitDelayRemaining <= 0 && !hasAppliedDmg)
            {
                ApplyAttack(Physics.OverlapSphere(m_basicAttackPoint.position, m_basicAttackRange, m_enemyLayers));

                hasAppliedDmg = true;
            }

            yield return null;
        }
    }

    private void ApplyAttack(Collider[] enemiesHit)
    {
        foreach (Collider enemy in enemiesHit)
        {
            Debug.Log("We hit");

            Onhit(enemy.GetComponent<Entity>());
        }
    }

    void Onhit(Entity enemy)
    {
        enemy.TakeDamage(new Damage(m_damage, this.gameObject.GetComponent<Entity>(), enemy));
        Debug.Log(enemy.GetHealth());
    }
}
