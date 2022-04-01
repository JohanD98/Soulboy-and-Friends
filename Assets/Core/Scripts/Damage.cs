using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Damage
{
    public float damageAmount;
    public Entity attacker;
    public Entity attacked;

    public Damage(float _damageAmount, Entity _attacker, Entity _attacked)
    {
        damageAmount = _damageAmount;
        attacker = _attacker;   
        attacked = _attacked;
    }
}
