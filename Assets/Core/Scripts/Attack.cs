using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Animator animator;
    public Transform BasicAttackPoint;
    public float BasicAttackRange = 0.5f;
    public LayerMask enemyLayers;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            basicAttack();
        }   
    }

    void basicAttack()
    {
        animator.SetTrigger("attack");

        Collider[] hitEnemies = Physics.OverlapSphere(BasicAttackPoint.position, BasicAttackRange, enemyLayers);

        foreach(Collider enemy in hitEnemies)
        {
            Debug.Log("We hit");
        }
    }
}
