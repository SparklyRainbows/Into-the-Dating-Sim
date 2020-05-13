using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour {
    public string stateName { get { return this.GetType().Name; } }

    public virtual void Enter() {
        AddListeners();
    }

    public virtual void Exit() {
        RemoveListeners();
    }

    protected virtual void OnDestroy() {
        RemoveListeners();
    }

    protected virtual void AddListeners() {

    }

    protected virtual void RemoveListeners() {

    }
}