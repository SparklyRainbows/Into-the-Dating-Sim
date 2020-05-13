using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    private RestController controller;

    public void SetLevel(int level) {
        controller = GetComponentInParent<RestController>();

        GetComponentInChildren<Text>().text = "Level " + level;

        if (level != GameInformation.instance.currentLevel) {
            GetComponentInChildren<Button>().interactable = false;
        }
    }

    public void StartBattle() {
        if (controller.CurrentState.stateName.Equals("DefaultRestState")) {
            SceneManager.LoadScene("BattleScene");
        }
    }
}
