using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestUI : MonoBehaviour
{
    public Text flourText;
    public Text flourText2;

    private RestController controller;
    private StatusUI statusUI;
    private DialogueManager dialogueManager;
    private InventoryUI inventoryUI;
    private ShopUI shopUI;
    private MenuUI menuUI;

    private void Awake() {
        controller = GetComponentInParent<RestController>();
        statusUI = GetComponentInChildren<StatusUI>();
        dialogueManager = GetComponentInChildren<DialogueManager>();
        inventoryUI = GetComponentInChildren<InventoryUI>();
        shopUI = GetComponentInChildren<ShopUI>();
        menuUI = GetComponentInChildren<MenuUI>();
    }

    private void Start() {
        UpdateFlour();
    }

    #region Show and Hide
    public void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }
    #endregion
    
    public void StartDialogue(List<DialogueLine> lines, RelationshipUI from, RelationshipRank rank, UnitPair pair) {
        dialogueManager.StartDialogue(lines, rank, pair);
        dialogueManager.fromUI = from;
        controller.ChangeState<DialogueState>();
    }

    #region Inventory
    public void ShowInventory() {
        if (controller.CurrentState.stateName.Equals("DefaultRestState")) {
            controller.ChangeState<InventoryState>();
            inventoryUI.Show();
        }
    }
    #endregion

    #region Shop
    public void ShowShop() {
        if (controller.CurrentState.stateName.Equals("DefaultRestState")) {
            controller.ChangeState<ShopState>();
            shopUI.Show();
        }
    }

    public void UpdateFlour() {
        flourText.text = "x" + GameInformation.instance.GetFlour();
        flourText2.text = "x" + GameInformation.instance.GetFlour();
    }
    #endregion

    #region Menu
    public void ShowMenu() {
        if (controller.CurrentState.stateName.Equals("DefaultRestState")) {
            controller.ChangeState<MenuState>();
            menuUI.Show();
        }
    }
    #endregion

    public void GoToDefaultState() {
        controller.ChangeState<DefaultRestState>();
    }
}
