using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestState : State
{
    protected RestController owner;
    public RestUI restUI { get { return owner.restUI; } }

    protected virtual void Awake() {
        owner = GetComponent<RestController>();
    }
}
