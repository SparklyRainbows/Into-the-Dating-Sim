using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    private PlayerUnitInfo info;
    
    public Text healthText;
    public Text moveText;

    public void Show(PlayerUnitInfo info) {
        Show();

        this.info = info;
        healthText.text = "Health: " + info.maxHealth;
        moveText.text = "Move: " + info.baseMove;

        ShowRelationships();
    }

    private void ShowRelationships() {
        RelationshipUI[] relationshipUIs = GetComponentsInChildren<RelationshipUI>();
        UnitID[] ids = { UnitID.MINKEE, UnitID.PHYNNE, UnitID.VHALL };

        int index = 0;
        foreach (UnitID id in ids) {
            if (id != info.id) {
                relationshipUIs[index].ShowRelationship(info.id, id);
                index++;
            }
        }
    }

    #region Show and hide
    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
    #endregion
}
