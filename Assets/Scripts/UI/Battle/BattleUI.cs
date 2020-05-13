using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public bool readyToEnd;

    private BattleController controller;
    private UnitListUI unitListUI;
    private UnitPortraitPanel unitPortraitUI;
    private TurnCounter turnCounterUI;
    private EndBattleUI endBattleUI;
    private EndTurnButton endTurnButton;

    public Text healthText;

    public GameObject tutorial;

    private void Awake() {
        controller = GetComponentInParent<BattleController>();
        unitListUI = GetComponentInChildren<UnitListUI>();
        unitPortraitUI = GetComponentInChildren<UnitPortraitPanel>();
        turnCounterUI = GetComponentInChildren<TurnCounter>();
        endBattleUI = GetComponentInChildren<EndBattleUI>();
        endTurnButton = GetComponentInChildren<EndTurnButton>();

        endBattleUI.Hide();
    }
    
    private void ResetAll() {
        if (unitListUI == null) {
            unitListUI = GetComponentInChildren<UnitListUI>();
        }

        unitListUI.ResetAll();
        HidePortrait();
        readyToEnd = false;
    }

    #region Show/Hide
    public void Show() {
        unitListUI.Show();
        turnCounterUI.Show();
        endTurnButton.Show();
    }

    public void Hide() {
        unitListUI.Hide();
        turnCounterUI.Hide();
        endTurnButton.Hide();
        HidePortrait();
    }
    #endregion

    #region UnitListUI
    public void Select(PlayerUnitInfo unit) {
        unitListUI.Select(unit.id);
        ShowPortrait(unit);
    }

    public void Deselect(PlayerUnitInfo unit) {
        unitListUI.Deselect(unit.id);
        HidePortrait();
    }

    public void FinishTurn(PlayerUnitInfo unit) {
        unitListUI.FinishTurn(unit.id);
        HidePortrait();
    }

    public void Die(PlayerUnitInfo unit) {
        unitListUI.Die(unit.id);
    }
    #endregion

    #region UnitPortraitUI
    public void ShowPortrait(EnemyUnitInfo unit) {
        unitPortraitUI.Show(unit);
    }

    public void ShowPortrait(PlayerUnitInfo unit) {
        GameObject unitObj = null;
        foreach (PlayerUnit script in controller.playerUnits) {
            if (script.gameObject.GetComponent<UnitStats>().ID == unit.id) {
                unitObj = script.gameObject;
                break;
            }
        }

        unitPortraitUI.Show(unitObj, unit);
    }

    public void HidePortrait() {
        unitPortraitUI.Hide();
    }
    #endregion

    #region Turn Counter
    public void StartBattle() {
        ResetAll();
        Show();

        if (turnCounterUI == null) {
            turnCounterUI = GetComponentInChildren<TurnCounter>();
        }

        turnCounterUI.StartBattle();
    }

    public void NextTurn() {
        turnCounterUI.NextTurn();
        ResetAll();
    }
    #endregion

    public void ShowEndBattle() {
        endBattleUI.Show();
    }

    public void ShowTutorial() {
        tutorial.gameObject.SetActive(true);
    }

    public void HideTutorial() {
        tutorial.gameObject.SetActive(false);
        controller.ChangeState<PlayerTurnStartState>();
    }

    public void SetHealthText(int health) {
        healthText.text = "Restaurant Health: " + health;
    }
}
