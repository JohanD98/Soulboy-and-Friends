using UnityEngine;

 [CreateAssetMenu(fileName = "Stats", menuName = "ScriptableObjects/Stats")]
public class ScriptableStats : ScriptableObject
{
    public float health;
    public float maxHealth;
    public float healthRegen;
    public float mana;
    public float maxMana;
    public float manaRegen;
    public float damage;
    public float armor;
    public float attackSpeed;
    public float movementSpeed;
}
