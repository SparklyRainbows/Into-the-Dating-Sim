using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnState : BattleState
{
    public override void Enter() {
        base.Enter();
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence() {
        ApplyStatuses();

        yield return new WaitForSeconds(.3f);
        
        if (owner.enemyUnits.Count == 0) {
            owner.EndBattle(true);
        } else if (owner.playerUnits.Count == 0 || owner.RestaurantsDestroyed()) {
            owner.EndBattle(false);
        } else {
            board.ClearForecast();

            foreach (EnemyUnit enemy in owner.enemyUnits) {
                yield return StartCoroutine(enemy.TakeTurn(owner.board));

                battleUI.SetHealthText(owner.GetRestaurantHealth());
            }

            if (owner.enemyUnits.Count == 0) {
                yield return new WaitForSeconds(.3f);
                owner.EndBattle(true);
            } else if (owner.playerUnits.Count == 0 || owner.RestaurantsDestroyed()) {
                yield return new WaitForSeconds(.3f);
                owner.EndBattle(false);
            }

            owner.ChangeState<PlayerTurnStartState>();
        }
    }

    private void ApplyStatuses() {
        foreach (EnemyUnit unit in owner.enemyUnits) {
            if (unit != null) {
                unit.GetComponent<UnitStats>().ApplyStatuses();
            }
        }

        UpdateUnitDisplay();
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
            }
        }
        while (deadEnemies.Count > 0) {
            owner.enemyUnits.Remove(deadEnemies.Dequeue());
        }
    }
}
