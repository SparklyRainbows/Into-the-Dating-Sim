using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPortraitPanel : MonoBehaviour
{
    public Image bust;
    public Text name;

    public WeaponUI mainWeaponUI;
    public WeaponUI secondaryWeaponUI;

    public Button talkButton;
    public DialogueManager dialogueManager;
    private UnitID currentUnit;
    private PlayerUnit adjacentUnit;

    public WeaponUI comboUI;

    private BattleController controller;

    public void Show(GameObject unitObj, PlayerUnitInfo unit) {
        controller = GetComponentInParent<BattleController>();

        gameObject.SetActive(true);
        bust.gameObject.SetActive(true);

        bust.sprite = unit.defaultBust;
        name.text = unit.name;


        if (unit.WeaponMain != null) {
            mainWeaponUI.gameObject.SetActive(true);
            mainWeaponUI.Set(unitObj, unit.WeaponMain);
        } else {
            mainWeaponUI.gameObject.SetActive(false);
        }

        if (unit.WeaponSecondary != null) {
            secondaryWeaponUI.gameObject.SetActive(true);
            secondaryWeaponUI.Set(unitObj, unit.WeaponSecondary);
        } else {
            secondaryWeaponUI.gameObject.SetActive(false);
        }

        talkButton.gameObject.SetActive(false);
        currentUnit = unit.id;

        comboUI.SetObj(unitObj);
        comboUI.gameObject.SetActive(false);
    }

    public void UpdateButtons(Point pos) {
        adjacentUnit = DirectionsExtensions.GetAdjacentPlayer(controller.board, pos);

        if (adjacentUnit == null) {
            talkButton.gameObject.SetActive(false);
            comboUI.gameObject.SetActive(false);

            return;
        }

        UnitID adjacentID = adjacentUnit.GetComponent<UnitStats>().ID;

        if (!controller.currentUnit.talked
            && BattleDialogue.StillHasDialogue(currentUnit, adjacentID)
            && !controller.talked) {
            talkButton.gameObject.SetActive(true);
        }
        
        comboUI.gameObject.SetActive(true);
        UnitPair pair = RelationshipManager.GetRelationshipPair(currentUnit, adjacentID);
        comboUI.SetWithoutObj(GameInformation.instance.GetComboInfo(pair));
    }

    public void Talk() {
        controller.ChangeState<BattleDialogueState>();
        controller.otherUnit = adjacentUnit;

        Relationship r = RelationshipManager.GetRelationshipBetween(currentUnit, adjacentUnit.GetComponent<UnitStats>().ID);
        RelationshipRank rank = r.GetNextRank();
        UnitPair pair = r.pair;
        List<DialogueLine> lines = BattleDialogue.GetBattleDialogueBetween(currentUnit, adjacentUnit.GetComponent<UnitStats>().ID);

        dialogueManager.StartDialogue(lines, rank, pair);
    }

    public void Show(EnemyUnitInfo unit) {
        gameObject.SetActive(true);
        bust.gameObject.SetActive(false);

        name.text = unit.name;

        //mainWeaponUI.Set(unit.WeaponMain);
        mainWeaponUI.gameObject.SetActive(false);
        secondaryWeaponUI.gameObject.SetActive(false);

        talkButton.gameObject.SetActive(false);

        comboUI.gameObject.SetActive(false);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
