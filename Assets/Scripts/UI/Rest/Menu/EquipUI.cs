using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipUI : MonoBehaviour
{
    public GameObject equipPrefab;
    public GameObject inventoryPanel;

    public EquipWeaponUI[] slots;
    private PlayerUnitInfo unit;

    private List<GameObject> children = new List<GameObject>();

    private EquipWeaponUI selected;

    #region equipping
    public bool Select(EquipWeaponUI ui) {
        if (selected == null) {
            selected = ui;
            return true;
        } else if (CanEquip(ui)) {
            Equip(ui);
            return false;
        }

        return false;
    }

    public void Deselect(EquipWeaponUI ui) {
        if (selected == ui) {
            selected = null;
        }
    }

    private void Equip(EquipWeaponUI ui) {
        EquipWeaponUI equippedUI = selected.isEquipSlot ? selected : ui;
        EquipWeaponUI other = selected.isEquipSlot ? ui : selected;

        if (equippedUI.scrollViewUI != null) {
            equippedUI.scrollViewUI.SetUnequipped();
        }

        equippedUI.SetEquipped(other.data, other);
        other.SetEquipped();

        UpdateUnitInfo();

        equippedUI.Deselect();
        other.Deselect();
        selected = null;
    }

    public void Unequip(EquipWeaponUI ui) {
        if (ui.scrollViewUI != null) {
            ui.scrollViewUI.SetUnequipped();
        }

        UpdateUnitInfo();
        ui.Deselect();

        if (selected == ui) {
            selected = null;
        }
    }

    private bool CanEquip(EquipWeaponUI ui) {
        if (!(selected.isEquipSlot ^ ui.isEquipSlot)) {
            return false;
        }

        if (selected.data != null && ui.data != null) {
            if (selected.data.name.Equals(ui.data.name)) {
                return false;
            }
        }

        EquipWeaponUI equippedUI = selected.isEquipSlot ? selected : ui;
        EquipWeaponUI other = selected.isEquipSlot ? ui : selected;

        if (slots[1] == equippedUI && !other.equipped) {
            return true;
        } else {
            return !other.data.consumed && !other.equipped;
        }
    }

    private void UpdateUnitInfo() {
        unit.WeaponMain = slots[0].data;
        unit.WeaponSecondary = slots[1].data;
    }
    #endregion

    #region set up scrollview and inventory
    private void SetEquipped(ItemData item, EquipWeaponUI scrollView, bool main) {
        int index = main ? 0 : 1;
        slots[index].SetEquipped(item, scrollView);
    }

    private void ResetEquipped() {
        slots[0].ResetEquipped();
        slots[1].ResetEquipped();

        slots[0].isEquipSlot = true;
        slots[1].isEquipSlot = true;
    }

    private void InitWeaponInventory() {
        ResetEquipped();

        foreach (ItemData item in GameInformation.instance.GetInventory()) {
            bool equippedByUnit = (unit.WeaponMain != null && unit.WeaponMain.name.Equals(item.name))
                || (unit.WeaponSecondary != null && unit.WeaponSecondary.name.Equals(item.name));

            if (item.equipped && !equippedByUnit) {
                continue;
            }

            if (item.type != WeaponType.NONE && item.type != unit.weaponType) {
                continue;
            }

            GameObject temp = Instantiate(equipPrefab, inventoryPanel.transform);
            EquipWeaponUI ui = temp.GetComponent<EquipWeaponUI>();

            ui.Set(item, equippedByUnit);

            if (equippedByUnit) {
                SetEquipped(item, ui, 
                    unit.WeaponMain != null && unit.WeaponMain.name.Equals(item.name));
            }

            children.Add(temp);
        }
    }

    private void DestroyChildren() {
        foreach (GameObject obj in children) {
            Destroy(obj);
        }
    }
    #endregion

    public void Show(PlayerUnitInfo unit) {
        this.unit = unit;

        DestroyChildren();
        InitWeaponInventory();

        gameObject.SetActive(true);
    }
}
