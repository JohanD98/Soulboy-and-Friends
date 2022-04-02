using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] ScriptableStats m_scriptableStats;
    Stats m_stats;

    // Start is called before the first frame update

    protected void EntityStart()
    {
        SetStatsToScriptableStats(m_scriptableStats);
        Debug.Log("start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(Damage damage)
    {
        m_stats.health = Mathf.Max(m_stats.health - (damage.damageAmount / Mathf.Max(1 + ( m_stats.armor/ 100), 1)), 0);
        if(m_stats.health <= 0)
        {
            //DEATH
        }
    }

    private void SetStatsToScriptableStats(ScriptableStats scriptableStats)
    {
        m_stats.health = scriptableStats.health;
        m_stats.maxHealth = scriptableStats.maxHealth;
        m_stats.healthRegen = scriptableStats.healthRegen;
        m_stats.mana = scriptableStats.mana;
        m_stats.manaRegen = scriptableStats.manaRegen;
        m_stats.maxMana = scriptableStats.maxMana;
        m_stats.armor = scriptableStats.armor;
        m_stats.attackSpeed = scriptableStats.attackSpeed;
        m_stats.movementSpeed = scriptableStats.movementSpeed;

    }

    public float GetHealth()
    {
        return m_stats.health;
    }

}
