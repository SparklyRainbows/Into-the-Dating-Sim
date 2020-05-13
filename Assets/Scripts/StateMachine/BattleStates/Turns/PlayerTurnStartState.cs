using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnStartState : BattleState {
    public override void Enter() {
        base.Enter();

        EnemyUnit.enemyAttackTiles.Clear();

        owner.talked = false;

        StartCoroutine(Init());
    }

    private IEnumerator Init() {
        foreach (PlayerUnit unit in owner.playerUnits) {
            unit.finishedTurn = false;
        }
        owner.battleUI.NextTurn();

        ApplyStatuses();
        yield return new WaitForSeconds(.3f);

        battleUI.SetHealthText(owner.GetRestaurantHealth());

        if (owner.enemyUnits.Count == 0) {
            owner.EndBattle(true);
        } else if (owner.playerUnits.Count == 0 || owner.RestaurantsDestroyed()) {
            owner.EndBattle(false);
        } else {
            ShowEnemyForecast();

            owner.ChangeState<SelectUnitState>();
        }
    }

    private void ApplyStatuses() {
        foreach (PlayerUnit unit in owner.playerUnits) {
            unit.GetComponent<UnitStats>().ApplyStatuses();
        }
        foreach (TerrainUnit restaurant in owner.restaurantUnits) {
            restaurant.GetComponent<UnitStats>().ApplyStatuses();
        }

        UpdateUnitDisplay();
    }

    private void UpdateUnitDisplay() {
        for (int i = 0; i < owner.playerUnits.Count; i++) {
            PlayerUnit unit = owner.playerUnits[i];
            if (unit.GetComponent<UnitStats>().IsDead()) {
                owner.battleUI.Die(unit.GetComponent<UnitStats>().playerUnitInfo);
                owner.playerUnits.Remove(unit);
                unit.Die();
            }
        }

        for (int i = 0; i < owner.enemyUnits.Count; i++) {
            EnemyUnit unit = owner.enemyUnits[i];
            if (unit.GetComponent<UnitStats>().IsDead()) {
                owner.enemyUnits.Remove(unit);
                unit.Die();

                board.DeselectForecast(unit.GetNextMove());
            }
        }
    }

    private void ShowEnemyForecast() {
        foreach (EnemyUnit enemy in owner.enemyUnits) {
            List<Tile> target = enemy.GetNextMove();

            if (target != null) {
                foreach (Tile t in target) {
                    board.Forecast(t);
                }
            }
        }
    }
}
