using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSequenceState : BattleState {
    public override void Enter() {
        base.Enter();

        owner.tileSelectionIndicator.GetComponent<Cursor>().canMove = false;
        StartCoroutine(Sequence());
    }

    public override void Exit() {
        base.Exit();

        owner.tileSelectionIndicator.GetComponent<Cursor>().canMove = true;
    }

    IEnumerator Sequence() {
        Movement m = owner.currentUnit.GetComponent<Movement>();
        yield return StartCoroutine(m.Traverse(owner.currentTile));
        
        owner.currentUnit.GetComponent<PlayerUnit>().moved = true;

        ForgetPrevMove();

        owner.ChangeState<ChooseWeaponState>();
    }

    private void ForgetPrevMove() {
        foreach (PlayerUnit unit in playerUnits) {
            if (unit != owner.currentUnit.GetComponent<PlayerUnit>()) {
                unit.prevTile = null;
            }
        }
    }
}