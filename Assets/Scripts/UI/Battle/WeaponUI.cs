using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Image weaponSprite;
    public GameObject infoPanel;
    public Text nameText;
    public Text descText;
    public Text useText;

    private ItemData info;
    private BattleController controller;
    private GameObject unitObj;

    private void Start() {
        controller = GetComponentInParent<BattleController>();
    }

    public void SetObj(GameObject unitObj) {
        this.unitObj = unitObj;
    }

    public void Set(GameObject unitObj, ItemData info) {
        this.unitObj = unitObj;
        this.info = info;

        weaponSprite.sprite = info.sprite;

        nameText.text = info.name;
        descText.text = info.description;
        
        if (info.hasLimitedUse) {
            useText.text = info.usesLeft.ToString();
        } else {
            useText.text = "";
        }

        if (info.NoMoreUses()) {
            GetComponentInChildren<Button>().interactable = false;
        } else {
            GetComponentInChildren<Button>().interactable = true;
        }

        HidePanel();
    }

    public void Set(ItemData info) {
        Set(null, info);
    }

    public void SetWithoutObj(ItemData info) {
        Set(unitObj, info);
    }

    public void ChooseWeapon() {
        if (unitObj == null) {
            Debug.LogWarning("Can't find unit object.");
            return;
        }

        if (info.NoMoreUses()) {
            return;
        }

        Component[] patterns = unitObj.GetComponents(Type.GetType(info.attackPattern.name));
        foreach (Component c in patterns) {
            AttackPattern p = (AttackPattern)c;

            if (p.data.name.Equals(info.name)) {
                controller.attackPattern = p;
                break;
            }
        }

        if (controller.CurrentState.stateName.Equals("AttackTargetState")) {
            controller.GetComponent<AttackTargetState>().UpdateTiles();
        } else {
            controller.ChangeState<AttackTargetState>();
        }
    }

    private void OnEnable() {
        if (info != null && info.Consumed()) {
            gameObject.SetActive(false);
        }
    }
    
    private void ShowPanel() {
        infoPanel.SetActive(true);
    }

    private void HidePanel() {
        infoPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        ShowPanel();
    }

    public void OnPointerExit(PointerEventData eventData) {
        HidePanel();
    }
}
