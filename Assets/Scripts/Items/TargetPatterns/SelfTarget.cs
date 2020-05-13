using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfTarget : AttackPattern
{
    public override List<List<Tile>> GetTilesInRange(BoardCreator board, Point pos) {
        List<List<Tile>> tiles = new List<List<Tile>>();
        tiles.Add(new List<Tile> { board.GetTile(pos) });

        return tiles;
    }
}
