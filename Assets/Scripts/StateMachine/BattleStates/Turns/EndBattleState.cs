using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndBattleState : BattleState
{
    public override void Enter() {
        base.Enter();
        battleUI.ShowEndBattle();
        GetComponentInChildren<Cursor>().canMove = false;

        StartCoroutine(WaitForInput());
    }

    private IEnumerator WaitForInput() {
        RemoveConsumedItems();

        while (!battleUI.readyToEnd) {
            yield return null;
        }

        foreach (PlayerUnit player in playerUnits) {
            Destroy(player.gameObject);
        }
        board.DestroyBoard();
        owner.ResetAll();
        
        GameInformation.instance.AddFlour(100 * GameInformation.instance.currentLevel);
        GameInformation.instance.currentLevel++;

        yield return null;

        if (GameInformation.instance.WonGame()) {
            SceneManager.LoadScene("WinScene");
        } else {
            SceneManager.LoadScene("RestScene");
        }
    }

    private void RemoveConsumedItems() {
        foreach (PlayerUnitInfo player in GameInformation.instance.playerInfo) {
            if (player.WeaponMain != null && player.WeaponMain.Consumed()) {
                GameInformation.instance.RemoveFromInventory(player.WeaponMain);
                player.WeaponMain = null;
            }

            if (player.WeaponSecondary != null && player.WeaponSecondary.Consumed()) {
                GameInformation.instance.RemoveFromInventory(player.WeaponSecondary);
                player.WeaponSecondary = null;
            }
        }
    }
}
