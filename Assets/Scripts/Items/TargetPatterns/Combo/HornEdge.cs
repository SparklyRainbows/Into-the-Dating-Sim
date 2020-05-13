using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornEdge : AttackPattern {
    private Relationship relationship = RelationshipManager.GetRelationshipBetween(UnitID.PHYNNE, UnitID.VHALL);

    public override IEnumerator HitUnits(BoardCreator board, Tile tile, Tile source) {
        yield return base.HitUnits(board, tile, source);

        relationship.IncreaseSupport();
    }

    public override List<List<Tile>> GetTilesInRange(BoardCreator board, Point pos) {
        List<List<Tile>> tiles = new List<List<Tile>>();

        tiles.Add(GetTilesInDirection(board, pos, Point.West, Point.North));
        tiles.Add(GetTilesInDirection(board, pos, Point.North, Point.West));
        tiles.Add(GetTilesInDirection(board, pos, Point.East, Point.North));
        tiles.Add(GetTilesInDirection(board, pos, Point.South, Point.West));

        return tiles;
    }

    private List<Tile> GetTilesInDirection(BoardCreator board, Point start, Point dir, Point oppDir) {
        List<Tile> tiles = new List<Tile>();

        start += dir;
        GetThreeTiles(board, tiles, start, start + oppDir, start - oppDir);
        start += dir;
        GetThreeTiles(board, tiles, start, start + oppDir, start - oppDir);

        GetStraightLine(board, tiles, start, dir);

        return tiles;
    }

    private void GetStraightLine(BoardCreator board, List<Tile> tiles, Point start, Point dir) {
        Point next = start + dir;
        Tile nextTile = board.GetTile(next);

        int rangeLeft = data.range + (int)relationship.GetRank();

        while (nextTile != null && rangeLeft > 0) {
            tiles.Add(nextTile);

            next += dir;
            rangeLeft--;
            nextTile = board.GetTile(next);
        }
    }

    private void GetThreeTiles(BoardCreator board, List<Tile> tiles, Point center, Point left, Point right) {
        Tile mid = board.GetTile(center);
        if (mid != null) {
            tiles.Add(mid);
        }
        Tile pos = board.GetTile(left);
        if (pos != null) {
            tiles.Add(pos);
        }
        Tile neg = board.GetTile(right);
        if (neg != null) {
            tiles.Add(neg);
        }
    }
}
