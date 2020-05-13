using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipWeaponUI : MonoBehaviour {
    public Sprite emptySprite;
    public Image panel;
    public Image image;
    public Text text;
    public Text equippedText;

    public ItemData data;

    private Color selectedColor = new Color32(112, 215, 255, 255);
    private Color equippedColor = Color.gray;
    private Color defaultColor = Color.white;

    private EquipUI equipUI;

    private bool selected;
    //If this is null, that means this object is in the scrollview;
    //Otherwise, this is a reference to the EquipWeaponUI of the corresponding object in the scrollview
    [HideInInspector] public EquipWeaponUI scrollViewUI;
    [HideInInspector] public bool isEquipSlot;
    [HideInInspector] public bool equipped;

    private void Start() {
        equipUI = GetComponentInParent<EquipUI>();
    }

    #region Select
    public void Unequip() {
        if (!isEquipSlot) {
            return;
        }

        data = null;
        equipUI.Unequip(this);
        ResetEquipped();

        scrollViewUI = null;
        equipped = false;
    }

    public void Deselect() {
        selected = false;
        panel.color = defaultColor;

        if (equipped) {
            panel.color = equippedColor;
        }
    }

    public void ToggleSelect() {
        if (selected) {
            Deselect();
            equipUI.Deselect(this);
        } else if (!equipped) {
            selected = equipUI.Select(this); ;

            if (selected) {
                panel.color = selectedColor;
            }
        }
    }
    #endregion

    #region Set Data
    public void ResetEquipped() {
        data = null;
        image.sprite = emptySprite;
        text.text = "Empty";
        equippedText.text = "";
    }

    public void SetEquipped(ItemData data, EquipWeaponUI scrollView) {
        this.data = data;
        scrollViewUI = scrollView;

        image.sprite = data.sprite;
        text.text = data.name + "\n" + data.description;

        if (data.hasLimitedUse) {
            text.text += "\nUses per battle: " + data.usesPerBattle;
        }
        if (data.consumed) {
            text.text += "\nConsumed on use.";
        }

        equippedText.text = "";
    }

    public void Set(ItemData data, bool equipped) {
        scrollViewUI = null;
        this.data = data;

        if (data == null) {
            image.sprite = emptySprite;
            text.text = "";
        } else {
            image.sprite = data.sprite;
            text.text = data.name + "\n" + data.description;
        }

        if (equipped) {
            SetEquipped();
        } else {
            SetUnequipped();
        }
    }

    public void SetUnequipped() {
        equipped = false;
        equippedText.text = "";
        panel.color = defaultColor;
    }

    public void SetEquipped() {
        equipped = true;
        equippedText.text = "Equipped";
        panel.color = equippedColor;
    }
    #endregion
}
