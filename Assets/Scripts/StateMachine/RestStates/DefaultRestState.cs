using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultRestState : RestState
{
    public override void Enter() {
        base.Enter();
        owner.restUI.Show();
    }
}
