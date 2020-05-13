using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopItem : MonoBehaviour {
    public Text nameAndCost;
    public Image image;

    private ItemData info;

    public void SetItem(ItemData info) {
        this.info = info;

        nameAndCost.text = info.name;
        image.sprite = info.sprite;
    }
    
    public void OnClick() { 
        GetComponentInParent<ShopUI>().Select(gameObject, info);
    }
}
