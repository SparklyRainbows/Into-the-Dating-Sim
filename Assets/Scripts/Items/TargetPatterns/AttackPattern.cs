using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackPattern : MonoBehaviour {
    public ItemData data;
    protected Unit unit;
    protected bool ignoreGroupHead;

    protected virtual void Awake() {
        unit = GetComponent<Unit>();
    }

    public void SetData(ItemData data) {
        this.data = data;
        data.Reset();
    }

    #region Get tiles
    public abstract List<List<Tile>> GetTilesInRange(BoardCreator board, Point pos);

    public List<List<Tile>> GetTilesInRange(BoardCreator board) {
        return GetTilesInRange(board, unit.tile.pos);
    }

    public static List<Tile> SquashList(List<List<Tile>> allTiles) {
        List<Tile> tiles = new List<Tile>();
        
        foreach (List<Tile> group in allTiles) {
            foreach (Tile t in group) {
                if (t != null && !tiles.Contains(t)) {
                    tiles.Add(t);
                }
            }
        }

        return tiles;
    }

    public List<Tile> GetAllTilesInRange(BoardCreator board) {
        List<Tile> tiles = new List<Tile>();

        List<List<Tile>> temp = GetTilesInRange(board);
        foreach (List<Tile> group in temp) {
            foreach (Tile t in group) {
                if (!tiles.Contains(t)) {
                    tiles.Add(t);
                }
            }
        }

        return tiles;
    }

    public List<Tile> GetGroup(List<List<Tile>> tiles, Tile t) {
        List<List<Tile>> groups = new List<List<Tile>>();

        foreach (List<Tile> group in tiles) {
            if (group == null) {
                continue;
            }
            if (group.Contains(t) && ignoreGroupHead) {
                return group;
            }
            if (group != null && group.Count > 0 && group[0] != null && group[0].pos == t.pos) {
                return group;
            }
        }

        return null;
    }

    #endregion

    public IEnumerator Attack(Tile target) {
        string audioStr = data.useSFX;
        if (audioStr != null && audioStr.Length > 0) {
            GameInformation.instance.audio.Play(audioStr);
        }

        Animator a = GetComponent<Animator>();

        if (a != null && a.isActiveAndEnabled && GetComponent<Unit>().type != UnitType.ENEMY) {
            float length = GetComponent<Unit>().PlayAttack(target);
            yield return new WaitForSeconds(length + .3f);
        } else {
            Vector2 startPos = transform.position;
            Vector2 temp = (target.GetPosition() - startPos) * 2 / 3 + startPos;

            Vector2 diff = temp - startPos;

            Vector2 targetPos = new Vector2(Mathf.Clamp(diff.x, -2f / 3, 2f / 3), Mathf.Clamp(diff.y, -2f / 3, 2f / 3)) + startPos;

            while ((Vector2)transform.position != targetPos) {
                transform.position = Vector2.MoveTowards(transform.position, targetPos, 0.2f);
                yield return null;
            }
            while ((Vector2)transform.position != startPos) {
                transform.position = Vector2.MoveTowards(transform.position, startPos, 0.2f);
                yield return null;
            }
        }
    }

    #region Apply damage
    protected List<UnitStats> GetHitUnits(BoardCreator board, Tile tile, Tile source) {
        List<List<Tile>> tiles = GetTilesInRange(board);
        List<Tile> selectedGroup = GetGroup(tiles, tile);

        List<UnitStats> hit = new List<UnitStats>();

        if (data.isElectric && data.type == WeaponType.RANGED) {
            Tile hitTile = selectedGroup[selectedGroup.Count - 1];
            ExpandHitUnits(board, hit, hitTile.pos);

            return hit;
        }

        foreach (Tile t in selectedGroup) {
            if (t != null && t.content != null && !t.content.GetComponent<UnitStats>().IsDead()) {
                hit.Add(t.content.GetComponent<UnitStats>());
            }
        }

        return hit;
    }

    protected List<UnitStats> GetHitUnits(BoardCreator board, List<Tile> targets, Tile source) {
        List<UnitStats> hit = new List<UnitStats>();

        if (data.isElectric && data.type == WeaponType.RANGED) {
            Tile hitTile = targets[targets.Count - 1];
            ExpandHitUnits(board, hit, hitTile.pos);

            return hit;
        }

        foreach (Tile t in targets) {
            if (t != null && t.content != null && !t.content.GetComponent<UnitStats>().IsDead()) {
                hit.Add(t.content.GetComponent<UnitStats>());
            }
        }

        return hit;
    }

    public virtual IEnumerator HitUnits(BoardCreator board, List<Tile> targets, Tile source) {
        List<UnitStats> hit = GetHitUnits(board, targets, source);

        foreach (UnitStats unit in hit) {
            yield return StartCoroutine(Hit(board, unit, source));
        }

        if (data.hasLimitedUse) {
            data.usesLeft--;
        }

        unit.GetComponent<UnitStats>().IncreaseSupport();
    }

    public virtual IEnumerator HitUnits(BoardCreator board, Tile tile, Tile source) {
        List<UnitStats> hit = GetHitUnits(board, tile, source);
        
        foreach (UnitStats unit in hit) {
            yield return StartCoroutine(Hit(board, unit, source));
        }

        if (data.hasLimitedUse) {
            data.usesLeft--;
        }

        unit.GetComponent<UnitStats>().IncreaseSupport();
    }

    protected IEnumerator Hit(BoardCreator board, UnitStats target, Tile source) {
        if (data.Damage > 0) {
            target.TakeDamage(unit.GetComponent<UnitStats>().GetAttack(board, data));
        }
        if (data.Heal > 0) {
            target.Heal(data.Heal);
        }

        if (!target.IsDead()) {
            if (data.hasKnockback && target.GetComponent<Unit>().KnockbackAble()) {
                Point direction = source.pos - target.GetComponent<Unit>().tile.pos;
                yield return StartCoroutine(target.ApplyKnockback(board, direction));
            }

            if (data.status != StatusEffects.NONE) {
                target.AddStatus(data.status);
            }
        }
    }
    #endregion

    #region Electricity
    private void ExpandHitUnits(BoardCreator board, List<UnitStats> hit, Tile currentTile) {
        Point pos = currentTile.pos;

        ExpandHitUnits(board, hit, pos + Point.North);
        ExpandHitUnits(board, hit, pos + Point.South);
        ExpandHitUnits(board, hit, pos + Point.East);
        ExpandHitUnits(board, hit, pos + Point.West);
    }

    private void ExpandHitUnits(BoardCreator board, List<UnitStats> hit, Point next) {
        Tile t = board.GetTile(next);
        if (t != null && t.content != null) {
            UnitStats unit = t.content.GetComponent<UnitStats>();

            if (!unit.IsDead() && unit.ConductsElectricity() && !hit.Contains(unit)) {
                hit.Add(unit);

                ExpandHitUnits(board, hit, t);
            }
        }
    }
    #endregion
}
