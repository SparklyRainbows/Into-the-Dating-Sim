using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : AttackPattern {
    private void Start() {
        ignoreGroupHead = true;
    }

    public override List<List<Tile>> GetTilesInRange(BoardCreator board, Point pos) {
        List<List<Tile>> tiles = new List<List<Tile>>();
        
        tiles.Add(GetTilesInDirection(board, pos, Point.North));
        tiles.Add(GetTilesInDirection(board, pos, Point.East));
        tiles.Add(GetTilesInDirection(board, pos, Point.South));
        tiles.Add(GetTilesInDirection(board, pos, Point.West));

        return tiles;
    }

    private List<Tile> GetTilesInDirection(BoardCreator board, Point start, Point dir) {
        List<Tile> tiles = new List<Tile>();

        Point next = start + dir;
        Tile nextTile = board.GetTile(next);

        while (nextTile != null) {
            Unit u = nextTile.content.GetComponent<Unit>();
            if (u.type != UnitType.TERRAIN) {
                break;
            }
            TerrainUnitInfo info = u.GetComponent<UnitStats>().terrainUnitInfo;
            if (info.BlocksProjectiles() &&
                !(u.GetComponent<UnitStats>().IsDead() && !info.BlocksProjectileWhenDead())) {
                break;
            }

            tiles.Add(nextTile);

            next += dir;
            nextTile = board.GetTile(next);
        }
        if (nextTile != null) {
            tiles.Add(nextTile);
        }

        return tiles;
    }
}
