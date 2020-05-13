using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiagonalAttack : AttackPattern
{
    public override List<List<Tile>> GetTilesInRange(BoardCreator board, Point pos) {
        List<List<Tile>> tiles = new List<List<Tile>>();

        tiles.Add(new List<Tile> { board.GetTile(pos + Point.North + Point.West) });
        tiles.Add(new List<Tile> { board.GetTile(pos + Point.South + Point.West) });
        tiles.Add(new List<Tile> { board.GetTile(pos + Point.North + Point.East) });
        tiles.Add(new List<Tile> { board.GetTile(pos + Point.South + Point.East) });

        return tiles;
    }
}
