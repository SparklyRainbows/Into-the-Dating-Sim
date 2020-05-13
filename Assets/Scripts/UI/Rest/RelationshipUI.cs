using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelationshipUI : MonoBehaviour
{
    private RestController controller;

    private UnitID currentUnit;
    private PlayerUnitInfo otherUnit;
    private Relationship relationship;

    public Image unitSprite;
    [Tooltip("From C -> S")]
    public List<Image> letters;
    public Text nameText;

    private List<Button> buttons;

    private Color hasRelationshipColor = Color.black;
    private Color noRelationshipColor = Color.gray;
    private Color hasConversationColor = new Color32(89, 227, 255, 255);

    private int curr;

    private void Start() {
        controller = GetComponentInParent<RestController>();
    }

    public void ShowRelationship(UnitID current, UnitID other) {
        UpdateButtons();

        relationship = RelationshipManager.GetRelationshipBetween(current, other);
        otherUnit = GameInformation.instance.GetPlayerInfo(other);
        currentUnit = current;

        unitSprite.sprite = otherUnit.unitSprite;
        nameText.text = otherUnit.name;

        curr = -1;
        switch (relationship.GetRank()) {
            case RelationshipRank.CRUDE:
                curr = 0;
                break;
            case RelationshipRank.BLAND:
                curr = 1;
                break;
            case RelationshipRank.APPETIZING:
                curr = 2;
                break;
            case RelationshipRank.SPICY:
                curr = 3;
                break;
        }

        UpdateRelationships();
    }

    private void UpdateRelationships() {
        for (int i = 0; i < curr + 1; i++) {
            letters[i].color = hasRelationshipColor;
            buttons[i].enabled = false;
        }
        
        for (int i = curr + 1; i < letters.Count; i++) {
            letters[i].color = noRelationshipColor;
            buttons[i].enabled = false;
        }

        if (relationship.ReachedNextRelationship()) {
            letters[curr + 1].color = hasConversationColor;
            SetButton(buttons[curr + 1]);
        }
    }

    private void SetButton(Button b) {
        b.enabled = true;
        b.onClick.AddListener(StartDialogue);
    }

    private void StartDialogue() {
        GetComponentInParent<RestUI>().StartDialogue(Dialogue.GetDialogueBetween(currentUnit, otherUnit.id), this,
            relationship.GetNextRank(), relationship.pair);
    }

    public void UpdateRelationshipUI() {
        curr++;
        UpdateRelationships();
    }

    private void UpdateButtons() {
        buttons = new List<Button>();
        foreach (Image m in letters) {
            buttons.Add(m.GetComponent<Button>());
        }
    }

    public void FinishSupportConversation() {
        relationship.FinishSupportConversation();
    }
}
