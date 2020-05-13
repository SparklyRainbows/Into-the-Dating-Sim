using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : ScriptableObject
{
    public UnitID id;
    public string name;

    public Sprite unitSprite;

    public MovementType movementType;
    [SerializeField] private ItemData weaponMain;
    [SerializeField] private ItemData weaponSecondary;

    public int maxHealth;
    public int baseMove;

    public AnimatorOverrideController controller;

    [Header("Left, Right, BackLeft, BackRight")]
    public AnimationClip[] attackAnims;
    public AnimationClip idleAnim;

    public string hurtSFX;

    public void Init() {
        WeaponMain = weaponMain;
        WeaponSecondary = weaponSecondary;
    }

    private ItemData _weaponMain;
    public ItemData WeaponMain {
        get {
            return _weaponMain;
        }
        set {
            if (_weaponMain != null) {
                _weaponMain.equipped = false;
            }
            _weaponMain = value;
            if (_weaponMain != null) {
                _weaponMain.equipped = true;
            }
        }
    }

    private ItemData _weaponSecondary;
    public ItemData WeaponSecondary {
        get {
            return _weaponSecondary;
        }
        set {
            if (_weaponSecondary != null) {
                _weaponSecondary.equipped = false;
            }
            _weaponSecondary = value;
            if (_weaponSecondary != null) {
                _weaponSecondary.equipped = true;
            }
        }
    }
}

public enum UnitID {
    TERRAIN,
    MINKEE,
    PHYNNE,
    VHALL,
    GREENSLIME,
    REDSLIME,
    BLUESLIME,
    ORANGESLIME,
    YELLOWSLIME,
    PURPLESLIME
}