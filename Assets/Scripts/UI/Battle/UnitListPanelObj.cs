using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitListPanelObj : MonoBehaviour
{
    private Text nameText;
    private Image panel;

    private Color defaultColor = Color.white;
    private Color selectedColor = new Color32(195, 224, 128, 255);
    private Color finishedTurnColor = Color.grey;
    private Color deadColor = Color.black;

    public bool dead;
    public bool finishedTurn;

    private void Start() {
        panel = GetComponent<Image>();
        nameText = GetComponentInChildren<Text>();
        panel.color = defaultColor;
    }

    public void Set(PlayerUnitInfo info) {
        if (nameText == null || panel == null) {
            nameText = GetComponentInChildren<Text>();
            panel = GetComponent<Image>();
        }

        nameText.text = info.name;
    }

    public void Select() {
        panel.color = selectedColor;
    }

    public void FinishTurn() {
        finishedTurn = true;

        panel.color = finishedTurnColor;
    }

    public void Reset() {
        if (dead) {
            return;
        }

        finishedTurn = false;
        panel.color = defaultColor;
    }

    public void Deselect() {
        if (dead || finishedTurn) {
            return;
        }

        panel.color = defaultColor;
    }

    public void Die() {
        dead = true;
        panel.color = deadColor;
    }
}
