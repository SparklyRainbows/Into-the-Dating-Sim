using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamyBuns : AttackPattern {
    private Relationship relationship = RelationshipManager.GetRelationshipBetween(UnitID.MINKEE, UnitID.VHALL);

    public override IEnumerator HitUnits(BoardCreator board, Tile tile, Tile source) {
        List<UnitStats> hit = GetHitUnits(board, tile, source);

        foreach (UnitStats unit in hit) {
            if (Random.Range(0, 2) == 0) {
                yield return StartCoroutine(Hit(board, unit, source));
            }
        }

        unit.GetComponent<UnitStats>().IncreaseSupport();
        relationship.IncreaseSupport();
    }

    public override List<List<Tile>> GetTilesInRange(BoardCreator board, Point pos) {
        List<List<Tile>> tiles = new List<List<Tile>>();

        int range = data.range + (int)relationship.GetRank();

        foreach (Point p in board.tiles.Keys) {
            tiles.Add(GetTilesAround(board, p, range));
        }

        return tiles;
    }

    private List<Tile> GetTilesAround(BoardCreator board, Point pos, int range) {
        List<Tile> tiles = new List<Tile>();
        
        tiles.Add(board.GetTile(pos));
        AddTileLine(board, tiles, pos, range);

        AddTilesInDirection(board, tiles, pos, Point.West, range);
        AddTilesInDirection(board, tiles, pos, Point.East, range);

        return tiles;
    }

    private void AddTilesInDirection(BoardCreator board, List<Tile> tiles, Point pos, Point dir, int range) {
        Point nextPos = pos + dir;

        int rangeLeft = range;

        while (rangeLeft > 0 && board.GetTile(nextPos) != null) {
            AddTileLine(board, tiles, nextPos, range);

            rangeLeft--;
            nextPos += dir;
        }
    }

    private void AddTileLine(BoardCreator board, List<Tile> tiles, Point pos, int range) {
        Tile t = board.GetTile(pos);
        if (!tiles.Contains(t)) {
            tiles.Add(t);
        }

        AddTile(board, tiles, pos, Point.North, range);
        AddTile(board, tiles, pos, Point.South, range);
    }

    private void AddTile(BoardCreator board, List<Tile> tiles, Point pos, Point dir, int range) {
        Tile tile = board.GetTile(pos);
        Point next = pos + dir;
        Tile nextTile = board.GetTile(next);

        while (nextTile != null && range > 0) {
            if (!tiles.Contains(nextTile)) {
                tiles.Add(nextTile);
            }

            next += dir;
            range--;
            nextTile = board.GetTile(next);
        }
    }
}
