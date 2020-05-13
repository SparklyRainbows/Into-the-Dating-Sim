using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnitInfo/PlayerUnitInfo")]
public class PlayerUnitInfo : UnitInfo {
    public Sprite portrait;

    [Header("Dialogue Busts")]
    public Sprite defaultBust;
    public Sprite blushing;
    public Sprite angry;
    public Sprite happy;
    public Sprite sad;
    public Sprite shocked;

    public WeaponType weaponType;
}
