using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnitInfo/TerrainUnitInfo")]
public class TerrainUnitInfo : UnitInfo {
    public Terrain terrain;

    public Sprite[] unitSprites;
    public Sprite frozenSprite;

    public bool IsDestructible() {
        switch (terrain) {
            case Terrain.RESTAURANT:
                return true;
            case Terrain.MOUNTAIN:
                return true;
        }

        return false;
    }

    public bool IsPassable(bool frozen, bool alive) {
        switch (terrain) {
            case Terrain.RESTAURANT:
                return !alive;
            case Terrain.MOUNTAIN:
                return !alive;
            case Terrain.RIVER:
                return frozen;
        }

        return true;
    }

    public bool BlocksProjectiles() {
        switch (terrain) {
            case Terrain.RESTAURANT:
                return true;
            case Terrain.MOUNTAIN:
                return true;
        }

        return false;
    }

    public bool BlocksProjectileWhenDead() {
        return false;
    }

    #region Status
    private bool IsFlammable() {
        switch (terrain) {
            case Terrain.RESTAURANT:
                return true;
            case Terrain.FOREST:
                return true;
        }

        return false;
    }

    private bool IsFreezable() {
        switch (terrain) {
            case Terrain.GRASS:
                return false;
            case Terrain.MOUNTAIN:
                return false;
            case Terrain.FOREST:
                return false;
        }

        return true;
    }

    public bool CanApplyStatus(StatusEffects status) {
        switch (status) {
            case StatusEffects.BURN:
                return IsFlammable();
            case StatusEffects.FREEZE:
                return IsFreezable();
            default:
                Debug.LogWarning("Don't know if status can be applied to " + terrain + ": " + status);
                return false;
        }
    }
    #endregion

    public bool ConductsElectricity() {
        switch (terrain) {
            case Terrain.RESTAURANT:
                return true;
            case Terrain.MOUNTAIN:
                return true;
        }
        return false;
    }

    public int GetValue() {
        switch (terrain) {
            case Terrain.GRASS:
                return 0;
            case Terrain.RESTAURANT:
                return 2;
            case Terrain.MOUNTAIN:
                return 0;
            case Terrain.FOREST:
                return 0;
            case Terrain.RIVER:
                return 0;
        }

        return 0;
    }
}

public enum Terrain {
    GRASS,
    RESTAURANT,
    MOUNTAIN,
    FOREST,
    RIVER
}