using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DirectionsExtensions {
    public static string left = "attack_left";
    public static string right = "attack_right";
    public static string backLeft = "attack_back_left";
    public static string backRight = "attack_back_right";
    public static string idle = "idle";

    public static Directions GetDirection(this Tile t1, Tile t2) {
        if (t1.pos.y < t2.pos.y)
            return Directions.NORTH;
        if (t1.pos.x < t2.pos.x)
            return Directions.EAST;
        if (t1.pos.y > t2.pos.y)
            return Directions.SOUTH;
        return Directions.WEST;
    }

    public static Vector3 ToEuler(this Directions d) {
        return new Vector3(0, (int)d * 90, 0);
    }

    public static string GetAttackDirection(this Tile t1, Tile t2) {
        int b = t1.pos.y - t1.pos.x;

        //Above the y=x+b line
        if (t2.pos.y >= t2.pos.x + b) {
            if (t2.pos.x >= t1.pos.x) {
                return backLeft;
            }
            return left;
        } else {
            if (t2.pos.x > t1.pos.x) {
                return backRight;
            }
            return right;
        }
    }

    public static PlayerUnit GetAdjacentPlayer(BoardCreator board, Point pos) {
        Tile t = board.GetTile(pos + Point.North);
        if (t != null && t.content != null && t.content.GetComponent<Unit>().type == UnitType.PLAYER) {
            return t.content.GetComponent<PlayerUnit>();
        }
        t = board.GetTile(pos + Point.South);
        if (t != null && t.content != null && t.content.GetComponent<Unit>().type == UnitType.PLAYER) {
            return t.content.GetComponent<PlayerUnit>();
        }
        t = board.GetTile(pos + Point.West);
        if (t != null && t.content != null && t.content.GetComponent<Unit>().type == UnitType.PLAYER) {
            return t.content.GetComponent<PlayerUnit>();
        }
        t = board.GetTile(pos + Point.East);
        if (t != null && t.content != null && t.content.GetComponent<Unit>().type == UnitType.PLAYER) {
            return t.content.GetComponent<PlayerUnit>();
        }

        return null;
    }
}

public enum Directions {
    NORTH,
    EAST,
    SOUTH,
    WEST
}