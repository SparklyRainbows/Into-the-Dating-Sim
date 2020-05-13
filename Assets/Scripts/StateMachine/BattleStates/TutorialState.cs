using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialState : BattleState
{
    public override void Enter() {
        base.Enter();

        battleUI.ShowTutorial();
    }

    public override void Exit() {
        base.Exit();

        GameInformation.instance.viewedTutorial = true;
    }
}
