using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;

    private PlayerUnitInfo info;
    private List<ItemData> items;
    private int index;

    public Image arrow;
    public Text upgradeText;
    public Text upgradeButtonText;
    public Button upgradeButton;

    [Header("Items")]
    public Image beforeSprite;
    public Text beforeText;
    public Image afterSprite;
    public Text afterText;

    private RestUI restUI;

    private void Start() {
        restUI = GetComponentInParent<RestUI>();
        items = new List<ItemData>();
    }

    public void Show(PlayerUnitInfo info) {
        gameObject.SetActive(true);

        this.info = info;
        SetItems();

        SelectItem();
    }

    public void Upgrade() {
        if (items == null || items.Count == 0) {
            SetItems();
        }

        ItemData item = items[index];

        if (GameInformation.instance.SubtractFlour(item.GetUpgradeCost())) {
            GameInformation.instance.audio.Play("upgrade");

            item.Upgrade();
            restUI.UpdateFlour();

            SelectItem();
        }
    }

    #region Selecting items
    public void Next() {
        index++;
        if (index >= items.Count) {
            index = 0;
        }

        SelectItem();
    }

    public void Prev() {
        index--;
        if (index < 0) {
            index = items.Count - 1;
        }

        SelectItem();
    }

    private void SelectItem() {
        if (items.Count == 0) {
            return;
        }

        ItemData item = items[index];

        beforeSprite.sprite = item.sprite;
        beforeText.text = item.GetRank() + " " + item.name;

        if (item.rank == WeaponRank.GOLD) {
            afterSprite.enabled = false;
            afterText.enabled = false;
            arrow.enabled = false;

            upgradeButton.interactable = false;
            upgradeButtonText.text = "Cannot upgrade";
        } else {
            afterSprite.enabled = true;
            afterText.enabled = true;
            arrow.enabled = true;

            afterSprite.sprite = GameInformation.instance.GetWeaponSprite(item, item.GetNextWeaponRank());
            afterText.text = item.GetNextRank() + " " + item.name;

            upgradeButton.interactable = true;
            upgradeButtonText.text = "Upgrade\nx" + item.GetUpgradeCost();
        }

        upgradeText.text = item.GetNextUpgrade();
    }
    #endregion

    private void SetItems() {
        if (items == null) {
            items = new List<ItemData>();
        }
        items.Clear();
        index = 0;
        
        if (info.WeaponMain != null) {
            items.Add(info.WeaponMain);
        }
        if (info.WeaponSecondary != null && info.WeaponSecondary.type != WeaponType.NONE) {
            items.Add(info.WeaponSecondary);
        }
        
        foreach (ItemData item in GameInformation.instance.GetInventory()) {
            if (info.WeaponMain == item || info.WeaponSecondary == item) {
                continue;
            }

            if (item.type == WeaponType.NONE || info.weaponType != item.type) {
                continue;
            }

            items.Add(item);
        }

        if (items.Count  < 2) {
            leftButton.interactable = false;
            rightButton.interactable = false;
        } else {
            leftButton.interactable = true;
            rightButton.interactable = true;
        }
    }
}
