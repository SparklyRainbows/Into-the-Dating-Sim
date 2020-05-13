using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAndSwimMovement : WalkMovement
{
    public override bool CanTraverse(Tile tile) {
        Unit u = tile.content.GetComponent<Unit>();

        if (u.type != UnitType.TERRAIN) {
            return false;
        }

        TerrainUnitInfo info = u.GetComponent<UnitStats>().terrainUnitInfo;

        if (info.terrain != Terrain.RIVER && !info.IsPassable(u.GetComponent<UnitStats>().HasStatus(StatusEffects.FREEZE),
            !u.GetComponent<UnitStats>().IsDead())) {
            return false;
        }

        return true;
    }
}
