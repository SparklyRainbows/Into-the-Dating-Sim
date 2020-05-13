using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryObjPrefab;
    public GameObject inventoryPanel;

    [Header("Info panel")]
    public Text itemName;
    public Text itemDescription;
    public Image itemImage;

    private List<GameObject> inventoryItems;

    private void Start() {
        inventoryItems = new List<GameObject>();
        Hide();
    }

    #region Updating Item
    private void InitInventory() {
        List<ItemData> inventory = GameInformation.instance.GetInventory();

        foreach (ItemData item in inventory) {
            GameObject temp = Instantiate(inventoryObjPrefab, inventoryPanel.transform);
            temp.GetComponent<InventoryItem>().SetItem(item);

            inventoryItems.Add(temp);
        }

        if (inventory.Count > 0) {
            Select(inventory[0]);
        } else {
            SelectEmpty();
        }
    }

    private void DestroyChildren() {
        foreach (GameObject obj in inventoryItems) {
            Destroy(obj);
        }
    }

    public void Select(ItemData item) {
        itemName.text = item.name;
        itemDescription.text = item.description;
        itemImage.sprite = item.sprite;
    }

    private void SelectEmpty() {
        itemName.text = "";
        itemDescription.text = "";
        itemImage.sprite = null;
    }
    #endregion

    #region Show/Hide
    public void Show() {
        gameObject.SetActive(true);
        InitInventory();
    }

    public void Hide() {
        DestroyChildren();

        GetComponentInParent<RestController>().ChangeState<DefaultRestState>();
        gameObject.SetActive(false);
    }
    #endregion

}
