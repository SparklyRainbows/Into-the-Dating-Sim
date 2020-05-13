using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    private BattleController controller;

    private void Start() {
        controller = GetComponentInParent<BattleController>();
    }

    public void EndTurn() {
        controller.ChangeState<EnemyTurnState>();
    }

    #region Show/Hide
    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
    #endregion
}
