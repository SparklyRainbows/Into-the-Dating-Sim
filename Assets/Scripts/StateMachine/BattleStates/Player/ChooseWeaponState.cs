using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseWeaponState : BattleState {

    public override void Enter() {
        base.Enter();

        owner.attackPattern = null;

        Tile currentTile = owner.currentUnit.tile;
        Cursor cursor = tileSelectionIndicator.GetComponent<Cursor>();

        cursor.SetPosition(currentTile.GetPosition(), currentTile.pos);
        cursor.canMove = false;

        GetComponentInChildren<UnitPortraitPanel>().UpdateButtons(currentTile.pos);
    }

    public override void Exit() {
        base.Exit();
        
        tileSelectionIndicator.GetComponent<Cursor>().canMove = true;
    }

    protected override void OnCancel(object sender, EventArgs e) {
        owner.ChangeState<SelectUnitState>();
    }
}
