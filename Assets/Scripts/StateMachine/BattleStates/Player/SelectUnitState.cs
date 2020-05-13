using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectUnitState : BattleState {
    private List<Tile> highlightedTiles;
    private List<Tile> forecastedTiles;

    public override void Enter() {
        base.Enter();
        owner.currentUnit = null;

        owner.tileSelectionIndicator.GetComponent<Cursor>().NotSelecting();
    }

    public override void Exit() {
        base.Exit();

        if (highlightedTiles != null) {
            HideTiles();
        }
        highlightedTiles = null;

        if (forecastedTiles != null) {
            HideDamageForecast();
        }
        forecastedTiles = null;
    }

    protected override void OnMove(object sender, InfoEventArgs<Point> e) {
        SelectTile(e.info);

        HideTiles();
        HideDamageForecast();
        battleUI.HidePortrait();

        GameObject content = owner.currentTile.content;
        if (content != null) {
            if (content.GetComponent<Unit>().type == UnitType.ENEMY) {
                //ShowMove(UnitType.ENEMY);
                ShowDamageForecast();
            } else if (content.GetComponent<Unit>().type == UnitType.TERRAIN) {
                //Show Terrain
            } else if (!content.GetComponent<PlayerUnit>().moved) {
                ShowMove(UnitType.PLAYER);
            } else if (!content.GetComponent<PlayerUnit>().finishedTurn) {
                battleUI.ShowPortrait(owner.currentTile.content.GetComponent<UnitStats>().playerUnitInfo);
                //ShowAttack();
            } else {
                battleUI.ShowPortrait(owner.currentTile.content.GetComponent<UnitStats>().playerUnitInfo);
            }
        }
    }

    protected override void OnSelect(object sender, EventArgs e) {
        GameObject content = owner.currentTile.content;

        if (content != null && content.GetComponent<PlayerUnit>() != null
                && !content.GetComponent<PlayerUnit>().CanMove()) {
            
            if (!content.GetComponent<PlayerUnit>().finishedTurn) {
                owner.tileSelectionIndicator.GetComponent<Cursor>().Selecting();
                owner.currentUnit = content.GetComponent<PlayerUnit>();

                owner.ChangeState<ChooseWeaponState>();
            }

            return;
        }

        if (content != null && content.GetComponent<Unit>().type == UnitType.PLAYER) {
            owner.tileSelectionIndicator.GetComponent<Cursor>().Selecting();
            owner.currentUnit = content.GetComponent<PlayerUnit>();

            if (!owner.currentUnit.moved) {
                owner.ChangeState<MoveTargetState>();
            }
        }
    }

    protected override void OnCancel(object sender, EventArgs e) {
        bool resetUnit = false;

        foreach (PlayerUnit unit in playerUnits) {
            if (unit.prevTile != null) {
                unit.Place(unit.prevTile);
                unit.Match();
                unit.prevTile = null;
                unit.moved = false;

                resetUnit = true;
            }
        }

        if (resetUnit) {
            HideTiles();
        }
    }

    private void ShowAttack() {
        AttackPattern pattern = owner.currentTile.content.GetComponent<AttackPattern>();
        highlightedTiles = pattern.GetAllTilesInRange(owner.board);
        board.SelectTiles(highlightedTiles);

        battleUI.ShowPortrait(owner.currentTile.content.GetComponent<UnitStats>().playerUnitInfo);
    }

    private void ShowMove(UnitType type) {
        Movement mover = owner.currentTile.content.GetComponent<Movement>();
        highlightedTiles = mover.GetTilesInRange(board);
        board.SelectTiles(highlightedTiles, type);

        if (type == UnitType.PLAYER) {
            battleUI.ShowPortrait(owner.currentTile.content.GetComponent<UnitStats>().playerUnitInfo);
        } else {
            battleUI.ShowPortrait(owner.currentTile.content.GetComponent<UnitStats>().enemyUnitInfo);
        }
    }

    private void HideDamageForecast() {
        if (forecastedTiles == null) {
            return;
        }

        board.HideDamageForecast(forecastedTiles);
        forecastedTiles = null;
    }

    private void ShowDamageForecast() {
        EnemyUnit enemy = owner.currentTile.content.GetComponent<EnemyUnit>();
        List<Tile> attacked = enemy.GetNextMove();

        if (attacked == null) {
            return;
        }

        foreach (Tile t in attacked) {
            board.DamageForecast(t, enemy.GetDamage());
        }

        forecastedTiles = attacked;
    }

    private void HideTiles() {
        if (highlightedTiles == null) {
            return;
        }

        board.DeSelectTiles(highlightedTiles);
        highlightedTiles = null;
    }
}