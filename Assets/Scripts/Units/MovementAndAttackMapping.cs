using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovementAndAttackMapping
{
    public static void SetMovement(MovementType type, GameObject obj) {
        switch (type) {
            case MovementType.WALK:
                obj.AddComponent<WalkMovement>();
                return;
            case MovementType.WALKANDSWIM:
                obj.AddComponent<WalkAndSwimMovement>();
                return;
            default:
                Debug.LogWarning("Cannot find movement: " + type);
                return;
        }
    }

    public static void SetAttack(ItemData weapon, GameObject obj) {
        if (weapon == null) {
            return;
        }
        
        AttackPattern script = (AttackPattern)obj.AddComponent(Type.GetType(weapon.attackPattern.name));
        script.SetData(weapon);
    }
}

public enum MovementType {
    NONE,
    WALK,
    WALKANDSWIM
}
