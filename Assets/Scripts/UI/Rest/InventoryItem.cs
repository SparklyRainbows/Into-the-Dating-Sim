using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler {
    public Text nameText;
    public Text equippedText;
    public Image sprite;

    private ItemData info;

    private InventoryUI inventoryUI;

    private void Start() {
        inventoryUI = GetComponentInParent<InventoryUI>();
    }

    public void SetItem(ItemData info) {
        this.info = info;

        nameText.text = info.name;
        sprite.sprite = info.sprite;

        equippedText.enabled = info.equipped;
    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        inventoryUI.Select(info);
    }
}
