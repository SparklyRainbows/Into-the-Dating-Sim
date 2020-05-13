using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject statusScreen;
    public GameObject equipScreen;
    public GameObject upgradeScreen;

    private RestController controller;
    private UnitWheelUI wheelUI;
    private StatusUI statusUI;
    private EquipUI equipUI;
    private UpgradeUI upgradeUI;

    private int currentUnit;

    private Menu currentPanel = Menu.STATUS;

    private void Start() {
        controller = GetComponentInParent<RestController>();
        statusUI = GetComponentInChildren<StatusUI>();
        wheelUI = GetComponentInChildren<UnitWheelUI>();
        equipUI = GetComponentInChildren<EquipUI>();
        upgradeUI = GetComponentInChildren<UpgradeUI>();
        
        Hide();
    }

    private void UpdatePanel() {
        statusScreen.SetActive(false);
        equipScreen.SetActive(false);
        upgradeScreen.SetActive(false);

        switch (currentPanel) {
            case Menu.STATUS:
                ShowStatus();
                return;
            case Menu.EQUIP:
                ShowEquip();
                return;
            case Menu.UPGRADE:
                ShowUpgrade();
                return;
        }
    }

    #region Show panels
    private void ShowStatus() {
        statusUI.Show(GameInformation.instance.playerInfo[currentUnit]);
    }

    private void ShowEquip() {
        equipUI.Show(GameInformation.instance.playerInfo[currentUnit]);
    }

    private void ShowUpgrade() {
        upgradeUI.Show(GameInformation.instance.playerInfo[currentUnit]);
    }
    #endregion

    #region Set Panel
    public void SetStatus() {
        currentPanel = Menu.STATUS;
        UpdatePanel();
    }

    public void SetUpgrade() {
        currentPanel = Menu.UPGRADE;
        UpdatePanel();
    }

    public void SetEquip() {
        currentPanel = Menu.EQUIP;
        UpdatePanel();
    }
    #endregion

    #region Change unit
    public void SpinLeft() {
        currentUnit = GetLeft();
        UpdatePanel();

        wheelUI.SpinRight(currentUnit);
    }

    public void SpinRight() {
        currentUnit = GetRight();
        UpdatePanel();

        wheelUI.SpinLeft(currentUnit);
    }

    public int GetLeft() {
        int left = currentUnit - 1;

        if (left < 0) {
            left = GameInformation.instance.playerInfo.Count - 1;
        }

        return left;
    }

    public int GetRight() {
        int right = currentUnit + 1;

        if (right > GameInformation.instance.playerInfo.Count - 1) {
            right = 0;
        }

        return right;
    }
    #endregion

    #region Show and hide
    public void Show() {
        UpdatePanel();
        gameObject.SetActive(true);
    }

    public void Hide() {
        controller.ChangeState<DefaultRestState>();

        gameObject.SetActive(false);

    }
    #endregion

    private enum Menu {
        STATUS,
        EQUIP,
        UPGRADE
    }
}