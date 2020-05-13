using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDialogueState : BattleState {
    public override void Enter() {
        base.Enter();

        owner.talked = true;
        owner.currentUnit.talked = true;
    }

    public override void Exit() {
        RelationshipManager.GetRelationshipBetween(owner.currentUnit.GetComponent<UnitStats>().ID,
            owner.otherUnit.GetComponent<UnitStats>().ID);
        owner.currentUnit.PlayHeart();
        owner.otherUnit.PlayHeart();

        owner.otherUnit = null;
    }
}
