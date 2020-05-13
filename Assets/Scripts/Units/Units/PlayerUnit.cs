using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
    public Animator heartAnimator;

    private List<PlayerUnit> nearbyUnits = new List<PlayerUnit>();
    private bool isDirty;

    private Color finishedTurnColor = Color.black;
    private Color defaultColor = Color.white;

    private bool _moved;
    public bool moved { get { return _moved; }
        set {
            _moved = value;
            isDirty = true;
        }
    }

    private bool _finishedTurn;
    public bool finishedTurn { get { return _finishedTurn; }
        set {
            _finishedTurn = value;
            prevTile = null;

            if (_finishedTurn) {
                renderer.color = finishedTurnColor;
            } else {
                renderer.color = defaultColor;
                moved = false;
                talked = false;
            }
        }
    }

    private BoardCreator board;

    private bool _talked;
    public bool talked {
        get {
            return _talked;
        }
        set {
            _talked = value;
            if (_talked) {
                prevTile = null;
            }
        }
    }

    public void UpdatePrevTile() {
        prevTile = tile;
    }

    protected override void Start() {
        base.Start();
        type = UnitType.PLAYER;
    }

    public void SetBoard(BoardCreator board) {
        this.board = board;
    }

    public List<PlayerUnit> GetNearbyUnits() {
        if (isDirty) {
            nearbyUnits = GetComponent<Movement>().GetNearbyUnits(board, 2);
            isDirty = false;
        }
        return nearbyUnits;
    }

    public bool CanMove() {
        return !_moved && !_finishedTurn && !GetComponent<UnitStats>().HasStatus(StatusEffects.FREEZE);
    }

    #region Animations
    public void PlayHeart() {
        heartAnimator.SetTrigger("play");
    }

    public override void InitAnimator(UnitInfo info) {
        if (animator == null) {
            animator = GetComponent<Animator>();
        }

        if (info.id != UnitID.MINKEE) {
            animator.runtimeAnimatorController = info.controller;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters) {
            animator.ResetTrigger(parameter.name);
        }

        /*if (info.attackAnims == null || info.attackAnims.Length < 4) {
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
        animator.runtimeAnimatorController = aoc;*/
    }

    protected override void ReplaceAnimation(AnimationClip oldClip, UnitInfo info, List<KeyValuePair<AnimationClip, AnimationClip>> anims) {
        int index = 0;
        string name = oldClip.name;

        if (name.Contains(DirectionsExtensions.left)) {
            index = 0;
        } else if (name.Contains(DirectionsExtensions.backLeft)) {
            index = 1;
        } else if (name.Contains(DirectionsExtensions.right)) {
            index = 2;
        } else if (name.Contains(DirectionsExtensions.backRight)) {
            index = 3;
        } else if (name.Contains(DirectionsExtensions.idle)) {
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldClip, info.idleAnim));
            return;
        } else {
            return;
        }

        anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldClip, info.attackAnims[index]));
    }

    public override float PlayAttack(Tile target) {
        if (animator == null || !animator.isActiveAndEnabled) {
            return 0;
        }

        string trigger = DirectionsExtensions.GetAttackDirection(tile, target);
        animator.SetTrigger(trigger);

        return animator.GetCurrentAnimatorStateInfo(0).length;
    }
    #endregion
}
