using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitListUI : MonoBehaviour {
    private UnitListPanelObj[] unitListPanels;
    private Dictionary<UnitID, UnitListPanelObj> dict;

    private void Start() {
        if (unitListPanels == null) {
            Init();
        }
    }

    public void Select(UnitID unit) {
        dict[unit].Select();
    }

    public void Deselect(UnitID unit) {
        dict[unit].Deselect();
    }

    public void FinishTurn(UnitID unit) {
        dict[unit].FinishTurn();
    }

    public void Die(UnitID unit) {
        dict[unit].Die();
    }

    public void ResetAll() {
        if (unitListPanels == null) {
            Init();
        }

        foreach (UnitListPanelObj panel in unitListPanels) {
            panel.Reset();
        }
    }

    private void Init() {
        unitListPanels = GetComponentsInChildren<UnitListPanelObj>();
        dict = new Dictionary<UnitID, UnitListPanelObj>();

        List<PlayerUnitInfo> unitInfo = GameInformation.instance.playerInfo;
        for (int i = 0; i < unitInfo.Count; i++) {
            dict.Add(unitInfo[i].id, unitListPanels[i]);
            unitListPanels[i].Set(unitInfo[i]);
        }
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

