using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTargetState : BattleState {
    List<List<Tile>> tiles;
    List<Tile> group;

    private int damage;

    public override void Enter() {
        base.Enter();
        damage = owner.currentUnit.GetComponent<UnitStats>().GetAttack(board, owner.attackPattern.data);

        UpdateTiles();
    }

    public override void Exit() {
        base.Exit();
        DeselectTiles();
        tiles = null;
        group = null;
    }

    protected override void OnMove(object sender, InfoEventArgs<Point> e) {
        SelectTile(e.info);

        if (group != null) {
            DeselectGroup();
        }

        if (tiles == null) {
            return;
        }

        group = owner.attackPattern.GetGroup(tiles, owner.currentTile);
        if (group != null) {
            SelectGroup();
        }
    }

    protected override void OnSelect(object sender, EventArgs e) {
        if (group != null) {
            owner.ChangeState<AttackSequenceState>();
        } else if (owner.currentTile == owner.currentUnit.tile) {
            owner.ChangeState<SelectUnitState>();
        }
    }

    protected override void OnCancel(object sender, EventArgs e) {
        owner.ChangeState<ChooseWeaponState>();
    }

    private void SelectTiles() {
        foreach (List<Tile> group in tiles) {
            board.SelectTiles(group);
        }
    }

    private void DeselectTiles() {
        foreach (List<Tile> group in tiles) {
            board.DeSelectTiles(group);
            board.DeselectForecastWithoutColor(group);
        }
    }

    private void SelectGroup() {
        board.SelectTiles(group, UnitType.ENEMY);

        foreach (Tile t in group) {
            board.DamageForecast(t, damage);
        }
    }

    private void DeselectGroup() {
        board.SelectTiles(group, UnitType.PLAYER);
        board.DeselectForecastWithoutColor(group);

        group = null;
    }

    public void UpdateTiles() {
        if (tiles != null) {
            DeselectTiles();
        }

        tiles = owner.attackPattern.GetTilesInRange(board);
        SelectTiles();
    }
}
