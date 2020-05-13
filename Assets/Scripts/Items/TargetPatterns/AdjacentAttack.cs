using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjacentAttack : AttackPattern {

    public override List<List<Tile>> GetTilesInRange(BoardCreator board, Point pos) {
        List<List<Tile>> tiles = new List<List<Tile>>();
        
        tiles.Add(new List<Tile> { board.GetTile(pos + Point.North) });
        tiles.Add(new List<Tile> { board.GetTile(pos + Point.South) });
        tiles.Add(new List<Tile> { board.GetTile(pos + Point.West) });
        tiles.Add(new List<Tile> { board.GetTile(pos + Point.East) });

        return tiles;
    }
}
