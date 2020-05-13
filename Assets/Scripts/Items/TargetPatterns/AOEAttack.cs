using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEAttack : AttackPattern {
    public override List<List<Tile>> GetTilesInRange(BoardCreator board, Point pos) {
        int range = data.range;
        List<List<Tile>> tiles = new List<List<Tile>>();

        List<List<List<Tile>>> temp = new List<List<List<Tile>>>();
        temp.Add(GetTilesInDirection(board, pos + new Point(0, range), Point.North));
        temp.Add(GetTilesInDirection(board, pos + new Point(0, -range), Point.South));
        temp.Add(GetTilesInDirection(board, pos + new Point(range, 0), Point.East));
        temp.Add(GetTilesInDirection(board, pos + new Point(-range, 0), Point.West));

        foreach (List<List<Tile>> l in temp) {
            foreach (List<Tile> t in l) {
                if (t.Count > 0) {
                    tiles.Add(t);
                }
            }
        }

        return tiles;
    }

    private List<List<Tile>> GetTilesInDirection(BoardCreator board,  Point curr, Point direction) {
        List<List<Tile>> tiles = new List<List<Tile>>();

        Tile t = board.GetTile(curr);
        while (t != null) {
            tiles.Add(GetSurroundingTiles(board, t.pos));
            curr += direction;
            t = board.GetTile(curr);
        }

        return tiles;
    }

    private List<Tile> GetSurroundingTiles(BoardCreator board, Point t) {
        List<Tile> tiles = new List<Tile>();

        List<Tile> temp = new List<Tile>();
        temp.Add(board.GetTile(t));
        temp.Add(board.GetTile(t + Point.North));
        temp.Add(board.GetTile(t + Point.South));
        temp.Add(board.GetTile(t + Point.West));
        temp.Add(board.GetTile(t + Point.East));

        foreach (Tile tile in temp) {
            if (t != null) {
                tiles.Add(tile);
            }
        }

        return tiles;
    }
}
