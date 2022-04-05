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
    [SerializeField] float attackTime;

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

        Collider[] hitEnemies = Physics.OverlapSphere(BasicAttackPoint.position, BasicAttackRange, enemyLayers);

        foreach(Collider enemy in hitEnemies)
        {
            Debug.Log("We hit");

            Onhit(enemy.GetComponent<Entity>());
        }

        StartCoroutine(MoveStop());



    }

    IEnumerator MoveStop()
    {
        float attackTimeRemaining = attackTime;

        this.gameObject.GetComponent<PlayerMovement>().StopAllMovement();

        while (attackTimeRemaining > 0)
        {
            attackTimeRemaining -= Time.deltaTime;
            yield return null;

        }

        this.gameObject.GetComponent<PlayerMovement>().AllowMovement();

    }

    void Onhit(Entity enemy)
    {
        enemy.TakeDamage(new Damage(damage, this.gameObject.GetComponent<Entity>(), enemy));
        Debug.Log(enemy.GetHealth());
     
    }
}
