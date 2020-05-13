using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepAttack : AttackPattern {
    public override List<List<Tile>> GetTilesInRange(BoardCreator board, Point pos) {
        List<List<Tile>> tiles = new List<List<Tile>>();
        
        tiles.Add(GetTilesInDirection(board, pos + Point.North, Point.West));
        tiles.Add(GetTilesInDirection(board, pos + Point.East, Point.North));
        tiles.Add(GetTilesInDirection(board, pos + Point.South, Point.West));
        tiles.Add(GetTilesInDirection(board, pos + Point.West, Point.North));

        return tiles;
    }
    
    private List<Tile> GetTilesInDirection(BoardCreator board, Point start, Point dir) {
        List<Tile> tiles = new List<Tile>();

        Tile mid = board.GetTile(start);
        if (mid != null) {
            tiles.Add(mid);
        }
        Tile pos = board.GetTile(start + dir);
        if (pos != null) {
            tiles.Add(pos);
        }
        Tile neg = board.GetTile(start - dir);
        if (neg != null) {
            tiles.Add(neg);
        }

        return tiles;
    }
}
