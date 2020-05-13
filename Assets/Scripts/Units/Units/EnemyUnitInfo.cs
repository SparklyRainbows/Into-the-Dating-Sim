using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnitInfo/EnemyUnitInfo")]
public class EnemyUnitInfo : UnitInfo {
    public int difficulty;

    public void InitWeapons() {
        Init();

        WeaponMain.SetDamage();
    }
}