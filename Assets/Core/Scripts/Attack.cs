using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Animator animator;
    public Transform BasicAttackPoint;
    public float BasicAttackRange = 0.5f;
    public LayerMask enemyLayers;
    public float damage;
    [SerializeField] float coolDownTimeBA;
    private float coolUpTimeBA;
    [SerializeField] float moveStopTime;
    [SerializeField] float bAttackHitDelay;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > coolUpTimeBA)
        {
            basicAttack();
            coolUpTimeBA = Time.time + coolDownTimeBA;
        }   
    }

    void basicAttack()
    {


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            this.gameObject.transform.LookAt(new Vector3(hit.point.x, this.transform.position.y, hit.point.z));
        }

        animator.SetTrigger("basicAttack");

        StopAllCoroutines();
        //StopCoroutine(PerformBasicAttack());
        StartCoroutine(PerformBasicAttack());


        Collider[] hitEnemies = Physics.OverlapSphere(BasicAttackPoint.position, BasicAttackRange, enemyLayers);



        



    }

    IEnumerator PerformBasicAttack()
    {

        float moveStopTimeRemaining = moveStopTime;
        float bAttackHitDelayRemaining = bAttackHitDelay;
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
                ApplyAttack(Physics.OverlapSphere(BasicAttackPoint.position, BasicAttackRange, enemyLayers));

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
        enemy.TakeDamage(new Damage(damage, this.gameObject.GetComponent<Entity>(), enemy));
        Debug.Log(enemy.GetHealth());
     
    }
}
