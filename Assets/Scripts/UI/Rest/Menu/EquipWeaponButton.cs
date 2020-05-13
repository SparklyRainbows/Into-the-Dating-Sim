using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipWeaponButton : MonoBehaviour, IPointerClickHandler {
    private EquipWeaponUI ui;

    public void OnPointerClick(PointerEventData eventData) {
        ui = GetComponentInParent<EquipWeaponUI>();

        if (ui == null) {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
            ui.ToggleSelect();
        else if (eventData.button == PointerEventData.InputButton.Right)
            ui.Unequip();
    }
}
