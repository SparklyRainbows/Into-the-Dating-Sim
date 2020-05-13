using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemData")]
public class ItemData : ScriptableObject
{
    public string name;
    [TextArea]
    public string description;

    [SerializeField] private Sprite _sprite;
    [HideInInspector] public Sprite sprite;

    public TextAsset attackPattern;
    public WeaponType type;

    public bool isSword; //yikers

    [SerializeField] private int baseDamage;
    public int Damage { get; set; }
    [SerializeField] private int baseHeal;
    public int Heal { get; set; }

    public int value;
    
    [HideInInspector] public bool owned;
    public string useSFX;

    [Header("Other")]
    public bool hasLimitedUse;
    public int usesPerBattle;
    public int range;
    public bool hasKnockback;
    public bool consumed;
    public bool isElectric;
    
    public StatusEffects status;

    private int _usesLeft;
    [HideInInspector] public int usesLeft {
        get {
            return _usesLeft;
        }

        set {
            _usesLeft = value;
            if (_usesLeft < 0) {
                _usesLeft = 0;
            }
        }
    }

    [HideInInspector] public bool equipped;
    [HideInInspector] public WeaponRank rank;

    #region Init
    public void OnStart() {
        owned = false;
        equipped = false;

        sprite = _sprite;

        Reset();

        rank = WeaponRank.WOODEN;
        Damage = baseDamage;
        Heal = baseHeal;
    }

    public void SetDamage() {
        Damage = baseDamage;
        Heal = baseHeal;
    }

    public void Reset() {
        usesLeft = usesPerBattle;
    }
    #endregion

    public int GetSellValue() {
        return value / 2;
    }

    public bool Consumed() {
        return consumed && _usesLeft <= 0;
    }

    public bool NoMoreUses() {
        return hasLimitedUse && !consumed && _usesLeft <= 0;
    }

    #region Upgrade
    public void Upgrade() {
        switch (rank) {
            case WeaponRank.WOODEN:
                Bronze();
                break;
            case WeaponRank.BRONZE:
                Steel();
                break;
            case WeaponRank.STEEL:
                Gold();
                break;
            case WeaponRank.GOLD:
                Debug.LogError("Cannot upgrade above silver");
                break;
        }

        sprite = GameInformation.instance.GetWeaponSprite(this, rank);
    }

    #region Getters
    public string GetNextUpgrade() {
        switch (rank) {
            case WeaponRank.WOODEN:
                return BronzeString();
            case WeaponRank.BRONZE:
                return SteelString();
            case WeaponRank.STEEL:
                return GoldString();
            case WeaponRank.GOLD:
                return "No more upgrades";
        }

        return "";
    }

    public int GetUpgradeCost() {
        switch (rank) {
            case WeaponRank.WOODEN:
                return 100;
            case WeaponRank.BRONZE:
                return 200;
            case WeaponRank.STEEL:
                return 400;
            case WeaponRank.GOLD:
                return 0;
        }
        return -1;
    }

    public string GetRank() {
        switch (rank) {
            case WeaponRank.WOODEN:
                return "Wooden";
            case WeaponRank.BRONZE:
                return "Bronze";
            case WeaponRank.STEEL:
                return "Steel";
            case WeaponRank.GOLD:
                return "Gold";
        }
        return "";
    }

    public WeaponRank GetNextWeaponRank() {
        switch (rank) {
            case WeaponRank.WOODEN:
                return WeaponRank.BRONZE;
            case WeaponRank.BRONZE:
                return WeaponRank.STEEL;
            case WeaponRank.STEEL:
                return WeaponRank.GOLD;
            case WeaponRank.GOLD:
                return WeaponRank.GOLD;
        }
        return WeaponRank.WOODEN;
    }

    public string GetNextRank() {
        switch (rank) {
            case WeaponRank.WOODEN:
                return "Bronze";
            case WeaponRank.BRONZE:
                return "Steel";
            case WeaponRank.STEEL:
                return "Gold";
            case WeaponRank.GOLD:
                return "";
        }
        return "";
    }
    #endregion

    #region Upgrades
    private void Bronze() {
        rank = WeaponRank.BRONZE;

        IncreaseDamage();
    }

    private string BronzeString() {
        return IncreaseDamageString();
    }

    private void Steel() {
        rank = WeaponRank.STEEL;

        if (hasLimitedUse) {
            usesPerBattle++;
        } else {
            IncreaseDamage();
        }
    }

    private string SteelString() {
        if (hasLimitedUse) {
            return "+1 Use";
        } else {
            return IncreaseDamageString();
        }
    }

    private void Gold() {
        rank = WeaponRank.GOLD;

        IncreaseDamage();
    }

    private string GoldString() {
        return IncreaseDamageString();
    }

    private void IncreaseDamage() {
        if (Heal == 0) {
            Damage++;
        } else {
            Heal++;
        }
    }

    private string IncreaseDamageString() {
        if (Heal == 0) {
            return "+1 Damage";
        } else {
            return "+1 Heal";
        }
    }
    #endregion
    #endregion
}

public enum StatusEffects {
    NONE,
    BURN,
    FREEZE,
    REGEN
}

public enum WeaponType {
    NONE,
    MELEE,
    RANGED,
    STAFF
}

public enum WeaponRank {
    WOODEN,
    BRONZE,
    STEEL,
    GOLD
}