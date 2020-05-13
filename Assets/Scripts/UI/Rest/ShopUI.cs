using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private RestUI restUI;

    public GameObject shopObjPrefab;
    public GameObject shopPanel;

    private List<ItemData> items;
    private List<GameObject> shopItems;

    [Header("Info panel")]
    public Text itemName;
    public Text itemDescription;
    public Image itemImage;
    public Text buyText;

    private ItemData selectedItem;
    private GameObject selectedItemObj;

    private void Start() {
        restUI = GetComponentInParent<RestUI>();
        shopItems = new List<GameObject>();

        Hide();
    }

    #region Updating Item
    private void InitShop() {
        items = GameInformation.instance.GetShopItems();

        foreach (ItemData item in items) {
            GameObject temp = Instantiate(shopObjPrefab, shopPanel.transform);

            temp.GetComponent<ShopItem>().SetItem(item);

            shopItems.Add(temp);
        }

        if (items.Count > 0) {
            Select(shopItems[0], items[0]);
        } else {
            SelectEmpty();
        }
    }

    private void DestroyChildren() {
        foreach (GameObject obj in shopItems) {
            Destroy(obj);
        }
    }

    public void Select(GameObject obj, ItemData item) {
        itemImage.enabled = true;

        selectedItem = item;
        selectedItemObj = obj;

        itemName.text = item.name;
        itemDescription.text = item.description;
        itemImage.sprite = item.sprite;
        buyText.text = "Buy - " + item.value + "g";
    }

    private void SelectEmpty() {
        selectedItem = null;
        selectedItemObj = null;

        itemName.text = "";
        itemDescription.text = "";
        itemImage.enabled = false;
        buyText.text = "Buy";
    }
    #endregion
    
    public void Buy() {
        if (selectedItem == null) {
            return;
        }

        if (GameInformation.instance.SubtractFlour(selectedItem.value)) {
            GameInformation.instance.audio.Play("buy");

            GameInformation.instance.AddToInventory(selectedItem);

            restUI.UpdateFlour();

            items.Remove(selectedItem);
            shopItems.Remove(selectedItemObj);

            Destroy(selectedItemObj);
            SelectEmpty();
        }
    }

    #region Show/Hide
    public void Show() {
        gameObject.SetActive(true);
        InitShop();
    }

    public void Hide() {
        DestroyChildren();

        GetComponentInParent<RestController>().ChangeState<DefaultRestState>();
        gameObject.SetActive(false);
    }
    #endregion
}
