﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBattleUI : MonoBehaviour
{
    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void EndBattleState() {
        Hide();
        GetComponentInParent<BattleUI>().readyToEnd = true;
    }
}
