using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkMovement : Movement
{
    protected override bool ExpandSearch(Tile from, Tile to) {
        if (!CanTraverse(to)) {
            return false;
        }

        return base.ExpandSearch(from, to);
    }

    public override IEnumerator Traverse(Tile tile) {
        unit.Place(tile);
        
        List<Tile> targets = new List<Tile>();
        while (tile != null) {
            targets.Insert(0, tile);
            tile = tile.prev;
        }
        
        for (int i = 1; i < targets.Count; ++i) {
            Tile from = targets[i - 1];
            Tile to = targets[i];

            yield return StartCoroutine(Walk(to));
        }

        yield return null;
    }

    IEnumerator Walk(Tile target) {
        while ((Vector2)transform.position != target.GetPosition()) {
            transform.position = Vector2.MoveTowards(transform.position, target.GetPosition(), 0.2f);
            yield return null;
        }
    }
}
