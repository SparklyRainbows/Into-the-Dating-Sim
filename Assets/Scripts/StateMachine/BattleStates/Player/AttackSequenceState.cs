using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSequenceState : BattleState {
    public override void Enter() {
        base.Enter();

        owner.tileSelectionIndicator.GetComponent<Cursor>().canMove = false;
        StartCoroutine(Sequence());
    }

    public override void Exit() {
        base.Exit();

        owner.tileSelectionIndicator.GetComponent<Cursor>().canMove = true;
        owner.attackPattern = null;
    }

    IEnumerator Sequence() {
        yield return StartCoroutine(owner.attackPattern.Attack(owner.currentTile));
        yield return StartCoroutine(owner.attackPattern.HitUnits(owner.board, owner.currentTile, owner.currentUnit.tile));

        owner.currentUnit.finishedTurn = true;
        owner.battleUI.FinishTurn(owner.currentUnit.gameObject.GetComponent<UnitStats>().playerUnitInfo);

        yield return null;

        battleUI.SetHealthText(owner.GetRestaurantHealth());

        UpdateUnitDisplay();
        ForgetPrevMove();

        if (owner.enemyUnits.Count == 0) {
            owner.EndBattle(true);
        } else if (owner.playerUnits.Count == 0 || owner.RestaurantsDestroyed()) {
            owner.EndBattle(false);
        } else {
            owner.ChangeState<SelectUnitState>();
        }
    }

    private void UpdateUnitDisplay() {
        Queue<PlayerUnit> deadPlayers = new Queue<PlayerUnit>();
        for (int i = 0; i < owner.playerUnits.Count; i++) {
            PlayerUnit unit = owner.playerUnits[i];
            if (unit.GetComponent<UnitStats>().IsDead()) {
                owner.battleUI.Die(unit.GetComponent<UnitStats>().playerUnitInfo);
                unit.Die();

                deadPlayers.Enqueue(unit);
            }
        }
        while (deadPlayers.Count > 0) {
            owner.playerUnits.Remove(deadPlayers.Dequeue());
        }

        Queue<EnemyUnit> deadEnemies = new Queue<EnemyUnit>();
        for (int i = 0; i < owner.enemyUnits.Count; i++) {
            EnemyUnit unit = owner.enemyUnits[i];

            if (unit.GetComponent<UnitStats>().IsDead()) {
                unit.Die();
                board.DeselectForecast(unit.GetNextMove());

                deadEnemies.Enqueue(unit);
            } else if (unit.GetComponent<UnitStats>().HasStatus(StatusEffects.FREEZE)) {
                board.DeselectForecast(unit.GetNextMove());
                unit.ResetMove();
            }
        }
        while (deadEnemies.Count > 0) {
            owner.enemyUnits.Remove(deadEnemies.Dequeue());
        }
    }

    private bool MovedAllUnits() {
        foreach (PlayerUnit unit in owner.playerUnits) {
            if (!unit.finishedTurn) {
                return false;
            }
        }

        return true;
    }

    private void ForgetPrevMove() {
        foreach (PlayerUnit unit in owner.playerUnits) {
            if (unit != owner.currentUnit.GetComponent<PlayerUnit>()) {
                unit.prevTile = null;
            }
        }
    }
}
