using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfTargetWithRange : AttackPattern {
    public override List<List<Tile>> GetTilesInRange(BoardCreator board, Point pos) {
        List<List<Tile>> tiles = new List<List<Tile>>();

        List<Tile> allTiles = GetAllTilesInRange(board, pos);
        foreach (Tile t in allTiles) {
            tiles.Add(new List<Tile> { t });
        }

        return tiles;
    }

    private List<Tile> GetAllTilesInRange(BoardCreator board, Point pos) {
        List<Tile> tiles = board.Search(board.GetTile(pos), ExpandSearch);
        tiles.Add(board.GetTile(pos));
        return tiles;
    }

    private bool ExpandSearch(Tile from, Tile to) {
        return (from.distance + 1) <= data.range;
    }
}
