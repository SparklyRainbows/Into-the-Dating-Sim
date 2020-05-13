using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainUnit : Unit
{
    protected override void Start() {
        base.Start();
        type = UnitType.TERRAIN;
    }

    public override bool KnockbackAble() {
        return false;
    }

    public void ChangeSprite(int health, TerrainUnitInfo info) {
        try {
            renderer.sprite = info.unitSprites[health];
        } catch (Exception e) {

        }
    }

    public void SetRiverSprite(bool frozen, TerrainUnitInfo info) {
        if (frozen) {
            renderer.sprite = info.frozenSprite;
        } else {
            renderer.sprite = info.unitSprite;
        }
    }

    public void SetSortingLayer() {
        Point pos = tile.pos;
        GetComponent<SpriteRenderer>().sortingOrder = 100 - (pos.x + pos.y);

        /*Vector2 newPos = transform.position;
        newPos.y = (pos.x + pos.y) * 0.59f;
        transform.position = newPos;*/
    }
}