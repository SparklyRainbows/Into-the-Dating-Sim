using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MoveTargetState : BattleState {
    List<Tile> tiles;

    public override void Enter() {
        base.Enter();

        PlayerUnit unit = owner.currentUnit.GetComponent<PlayerUnit>();
        if (unit.prevTile != null && unit.tile != unit.prevTile) {
            unit.Place(unit.prevTile);
            unit.Match();
        }

        Movement mover = owner.currentUnit.GetComponent<Movement>();
        tiles = mover.GetTilesInRange(board);

        UpdateSelectedTiles();
    }

    public override void Exit() {
        base.Exit();
        
        if (owner.currentUnit != null && !tiles.Contains(owner.currentUnit.tile)) {
            tiles.Add(owner.currentUnit.tile);
        }
        board.DeSelectTiles(tiles);
        tiles = null;
    }

    protected override void OnMove(object sender, InfoEventArgs<Point> e) {
        SelectTile(e.info);
        UpdateSelectedTiles();
    }

    protected override void OnSelect(object sender, EventArgs e) {
        if (tiles.Contains(owner.currentTile)) {
            owner.currentUnit.GetComponent<PlayerUnit>().UpdatePrevTile();
            owner.ChangeState<MoveSequenceState>();
        }
    }

    protected override void OnCancel(object sender, EventArgs e) {
        if (owner.currentUnit != null && !tiles.Contains(owner.currentUnit.tile)) {
            tiles.Add(owner.currentUnit.tile);
        }
        owner.currentUnit = null;
        owner.ChangeState<SelectUnitState>();
    }

    private void UpdateSelectedTiles() {
        board.SelectTiles(tiles);
        if (tiles.Contains(owner.currentTile)) {
            board.SelectTilesBetween(owner.currentUnit.tile, owner.currentTile, 
                owner.currentUnit.GetComponent<Movement>().CanTraverse);
        }
    }
}