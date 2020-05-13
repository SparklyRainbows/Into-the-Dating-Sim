using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DawnAndDusk : AttackPattern {
    private Relationship relationship = RelationshipManager.GetRelationshipBetween(UnitID.MINKEE, UnitID.PHYNNE);

    public override IEnumerator HitUnits(BoardCreator board, Tile tile, Tile source) {
        data.status = GameInformation.instance.GetPlayerInfo(UnitID.PHYNNE).WeaponMain.status;

        yield return base.HitUnits(board, tile, source);

        relationship.IncreaseSupport();
    }

    public override List<List<Tile>> GetTilesInRange(BoardCreator board, Point pos) {
        List<List<Tile>> tiles = new List<List<Tile>>();

        int range = data.range + (int)relationship.GetRank();
        while (range > 1) {
            tiles.Add(GetTilesInDirection(board, pos, Point.North, range));
            tiles.Add(GetTilesInDirection(board, pos, Point.South, range));
            tiles.Add(GetTilesInDirection(board, pos, Point.West, range));
            tiles.Add(GetTilesInDirection(board, pos, Point.East, range));

            range--;
        }

        return tiles;
    }

    private List<Tile> GetTilesInDirection(BoardCreator board, Point pos, Point dir, int range) {
        List<Tile> tiles = new List<Tile>();

        Point next = pos + dir;
        Tile nextTile = board.GetTile(next);

        while (nextTile != null && range > 0) {
            tiles.Add(nextTile);

            next += dir;
            range--;
            nextTile = board.GetTile(next);
        }

        Point finalPoint = next - dir;
        Tile finalTile = board.GetTile(finalPoint);

        if (finalTile == null) {
            return null;
        }

        if (tiles.Count == 0) {
            return tiles;
        }

        Tile temp = tiles[0];
        tiles[0] = finalTile;
        tiles[tiles.Count - 1] = temp;

        GetTilesAround(board, tiles, finalPoint);

        return tiles;
    }

    private void GetTilesAround(BoardCreator board, List<Tile> tiles, Point pos) {
        AddTile(board, tiles, pos + Point.North);
        AddTile(board, tiles, pos + Point.South);
        AddTile(board, tiles, pos + Point.East);
        AddTile(board, tiles, pos + Point.West);
        AddTile(board, tiles, pos + Point.North + Point.West);
        AddTile(board, tiles, pos + Point.North + Point.East);
        AddTile(board, tiles, pos + Point.South + Point.West);
        AddTile(board, tiles, pos + Point.South + Point.East);
    }

    private void AddTile(BoardCreator board, List<Tile> tiles, Point pos) {
        Tile tile = board.GetTile(pos);

        if (tile != null && !tiles.Contains(tile)) {
            tiles.Add(tile);
        }
    }
}
