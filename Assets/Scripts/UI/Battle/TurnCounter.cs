using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCounter : MonoBehaviour
{
    private Text text;
    private int turn;

    private void Start() {
        text = GetComponentInChildren<Text>();
    }

    public void StartBattle() {
        turn = 0;
        UpdateText();
    }

    public void NextTurn() {
        turn++;
        UpdateText();
    }

    private void UpdateText() {
        if (text == null) {
            text = GetComponentInChildren<Text>();
        }

        text.text = "Turn " + turn;
    }

    #region Show/Hide
    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
    #endregion
}
