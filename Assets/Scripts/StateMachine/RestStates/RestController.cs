using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestController : StateMachine
{
    public RestUI restUI;
    public DialogueManager dialogueManager;

    public void Start() {
        GameInformation.instance.PlayMenu();
        ChangeState<DefaultRestState>();
    }
}
