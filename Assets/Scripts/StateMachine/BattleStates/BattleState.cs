using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleState : State {
    protected BattleController owner;
    public BattleUI battleUI { get { return owner.battleUI; } }
    public CameraRig cameraRig { get { return owner.cameraRig; } }
    public BoardCreator board { get { return owner.board; } }
    public Transform tileSelectionIndicator { get { return owner.tileSelectionIndicator; } }
    public Point pos { get { return owner.pos; } set { owner.pos = value; } }
    public List<PlayerUnit> playerUnits { get { return owner.playerUnits; } }
    public List<EnemyUnit> enemyUnits { get { return owner.enemyUnits; } }

    protected virtual void Awake() {
        owner = GetComponent<BattleController>();
    }

    protected override void AddListeners() {
        InputController.moveEvent += OnMove;
        InputController.selectEvent += OnSelect;
        InputController.cancelEvent += OnCancel;
        InputController.resetEvent += OnReset;
        InputController.autoLoseEvent += AutoLose;
    }

    protected override void RemoveListeners() {
        InputController.moveEvent -= OnMove;
        InputController.selectEvent -= OnSelect;
        InputController.cancelEvent -= OnCancel;
        InputController.resetEvent -= OnReset;
        InputController.autoLoseEvent -= AutoLose;
    }

    protected virtual void OnMove(object sender, InfoEventArgs<Point> e) {

    }

    protected virtual void OnSelect(object sender, EventArgs e) {

    }

    protected virtual void OnCancel(object sender, EventArgs e) {

    }

    private void OnReset(object sender, EventArgs e) {
        owner.ResetTurn();
    }

    private void AutoLose(object sender, EventArgs e) {
        owner.EndBattle(false);
    }

    protected virtual void SelectTile(Point p) {
        if (pos == p || !board.tiles.ContainsKey(p))
            return;

        pos = p;
        tileSelectionIndicator.localPosition = board.tiles[p].GetPosition();
    }
}