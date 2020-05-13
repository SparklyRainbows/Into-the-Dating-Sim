using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit
{
    public static List<Tile> enemyAttackTiles = new List<Tile>();

    private List<Tile> attackTarget;
    private Tile nonNullAttackTarget;
    private Tile moveTarget;

    private int damage;

    protected override void Start() {
        base.Start();
        type = UnitType.ENEMY;

        damage = GetComponent<UnitStats>().enemyUnitInfo.WeaponMain.Damage;
    }

    public virtual IEnumerator TakeTurn(BoardCreator board) {
        if (!CanMove()) {
            ResetMove();
        } else {
            if (nonNullAttackTarget != null) {
                AttackPattern pattern = GetComponent<AttackPattern>();
                yield return StartCoroutine(pattern.Attack(nonNullAttackTarget));

                pattern.SetData(GetComponent<UnitStats>().unitInfo.WeaponMain);
                yield return StartCoroutine(pattern.HitUnits(board, attackTarget, moveTarget));

                attackTarget = null;
                nonNullAttackTarget = null;
            } else {
                SetNextMove(board);

                Movement m = GetComponent<Movement>();
                yield return StartCoroutine(m.Traverse(moveTarget));
            }
        }
    }

    public void SetNextMove(BoardCreator board) {
        Movement m = GetComponent<Movement>();
        List<Tile> movementTiles = m.GetTilesInRange(board);

        GetBestTiles(board, movementTiles);

        if (moveTarget == null) { 
            movementTiles.Remove(tile);
            moveTarget = movementTiles[Random.Range(0, movementTiles.Count)];
        }

        if (attackTarget != null) {
            foreach (Tile t in attackTarget) {
                enemyAttackTiles.Add(t);
            }
        }
    }

    public void MoveAttackTarget(BoardCreator board, Point dir) {
        if (attackTarget == null) {
            return;
        }

        board.DeselectForecast(attackTarget);
        board.DeselectForecastWithoutColor(attackTarget);

        List<Tile> newAttack = new List<Tile>();
        
        for (int i = 0; i < attackTarget.Count; i++) {
            Tile curr = attackTarget[i];
            curr = board.GetTile(curr.pos - dir);

            if (curr != null) {
                board.Forecast(curr);
                newAttack.Add(curr);
            }
        }

        attackTarget = newAttack;
    }

    public List<Tile> GetNextMove() {
        return attackTarget;
    }

    private int GetTileValue(Tile t) {
        if (t == null || t.content == null || t.content.GetComponent<UnitStats>().IsDead()) {
            return 0;
        }

        if (enemyAttackTiles.Contains(t)) {
            return 0;
        }

        Unit unit = t.content.GetComponent<Unit>();
        UnitStats stats = unit.GetComponent<UnitStats>();
        if (unit.type == UnitType.ENEMY) {
            if (stats.HasStatus(StatusEffects.FREEZE)) {
                return 1;
            }

            return 0;
        }
        if (unit.type == UnitType.TERRAIN) {
            if (stats.HasStatus(StatusEffects.BURN)) {
                return 0;
            }
            
            return stats.terrainUnitInfo.GetValue();
        }

        return 1;
    }

    private void GetBestTiles(BoardCreator board, List<Tile> movementTiles) {
        moveTarget = null;

        int leastMoveDist = 100;
        int bestValue = 0;

        AttackPattern pattern = GetComponent<AttackPattern>();

        foreach (Tile t in movementTiles) {
            List<List<Tile>> inRange = pattern.GetTilesInRange(board, t.pos);

            foreach (List<Tile> targets in inRange) {
                int value = GetTileGroupValue(targets);
                if (value >= bestValue) {
                    if (value > bestValue ||
                        (GetMoveDist(t) < leastMoveDist)) {
                        attackTarget = targets;
                        bestValue = value;

                        moveTarget = t;
                        leastMoveDist = GetMoveDist(t);
                    }
                }
            }
        }
    }

    private int GetMoveDist(Tile t) {
        return Mathf.Abs((t.pos.x + t.pos.y) - (tile.pos.x + tile.pos.y));
    }

    private int GetTileGroupValue(List<Tile> tiles) {
        int value = 0;

        foreach (Tile t in tiles) {
            int tVal = GetTileValue(t);
            value += tVal;

            if (tVal > 0) {
                nonNullAttackTarget = t;
            }
        }

        return value;
    }

    public void ResetMove() {
        attackTarget = null;
        moveTarget = null;
    }

    public int GetDamage() {
        return damage;
    }

    #region Animations
    public override void InitAnimator(UnitInfo info) {
        if (animator == null) {
            animator = GetComponent<Animator>();
        }

        if (info.attackAnims == null || info.attackAnims.Length < 1) {
            Debug.LogWarning("Animation not set up correctly for " + info.name);
            animator.enabled = false;

            return;
        }

        AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        foreach (AnimationClip clip in aoc.animationClips) {
            ReplaceAnimation(clip, info, anims);
        }

        aoc.ApplyOverrides(anims);
        animator.runtimeAnimatorController = aoc;
    }

    protected override void ReplaceAnimation(AnimationClip oldClip, UnitInfo info, List<KeyValuePair<AnimationClip, AnimationClip>> anims) {
        if (oldClip.name.Contains("attack")) {
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldClip, info.attackAnims[0]));
        } else if (oldClip.name.Contains("idle")) {
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldClip, info.idleAnim));
        }
    }

    public override float PlayAttack(Tile target) {
        animator.SetTrigger("attack");
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }
    #endregion

    private bool CanMove() {
        return !GetComponent<UnitStats>().HasStatus(StatusEffects.FREEZE);
    }
}
