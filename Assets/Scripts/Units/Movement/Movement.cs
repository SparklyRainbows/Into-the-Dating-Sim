using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour {
    protected Unit unit;

    protected virtual void Awake() {
        unit = GetComponent<Unit>();
    }

    public abstract IEnumerator Traverse(Tile tile);

    public virtual List<Tile> GetTilesInRange(BoardCreator board) {
        List<Tile> retValue = board.Search(unit.tile, ExpandSearch);
        Filter(retValue);
        retValue.Add(unit.tile);
        return retValue;
    }

    protected virtual bool ExpandSearch(Tile from, Tile to) {
        return (from.distance + 1) <= GetComponent<UnitStats>().Move;
    }

    private bool ExpandSearchWithRange(Tile from, Tile to, int range) {
        return (from.distance + 1) <= range;
    }

    protected virtual void Filter(List<Tile> tiles) {
        for (int i = tiles.Count - 1; i >= 0; --i) {
            if (!CanTraverse(tiles[i])) {
                tiles.RemoveAt(i);
            }
        }
    }

    public List<PlayerUnit> GetNearbyUnits(BoardCreator board, int range) {
        List<PlayerUnit> units = new List<PlayerUnit>();

        List<Tile> tiles = board.Search(unit.tile, ExpandSearchWithRange, 2);
        tiles.Remove(unit.tile);

        foreach (Tile t in tiles) {
            if (t.content.GetComponent<Unit>().type == UnitType.PLAYER) {;
                units.Add(t.content.GetComponent<PlayerUnit>());
            }
        }

        return units;
    }

    public virtual bool CanTraverse(Tile tile) {
        Unit u = tile.content.GetComponent<Unit>();

        if (u.type != UnitType.TERRAIN) {
            return false;
        }

        TerrainUnitInfo info = u.GetComponent<UnitStats>().terrainUnitInfo;

        if (!info.IsPassable(u.GetComponent<UnitStats>().HasStatus(StatusEffects.FREEZE),
            !u.GetComponent<UnitStats>().IsDead())) {
            return false;
        }

        return true;
    }
}