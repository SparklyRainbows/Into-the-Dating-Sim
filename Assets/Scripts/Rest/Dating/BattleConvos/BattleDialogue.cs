using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class BattleDialogue : MonoBehaviour {
    private string path = "BattleDialogue/";

    private static List<List<DialogueLine>> minkeePhynneDialogueBattle;
    private static List<List<DialogueLine>> phynneVhallDialogueBattle;
    private static List<List<DialogueLine>> vhallMinkeeDialogueBattle;

    private void Start() {
        InitMinkeePhynneDialogue();
        InitPhynneVhallDialogue();
        InitVhallMinkeeDialogue();
    }

    #region Dialogue Init
    void InitMinkeePhynneDialogue() {
        minkeePhynneDialogueBattle = new List<List<DialogueLine>>();
        string filePath = path + "minkeePhynne";

        ConvertToDialogue(filePath, minkeePhynneDialogueBattle);
    }
    
    void InitPhynneVhallDialogue() {
        phynneVhallDialogueBattle = new List<List<DialogueLine>>();
        string filePath = path + "phynneVhall";
        
        ConvertToDialogue(filePath, phynneVhallDialogueBattle);
    }
    
    void InitVhallMinkeeDialogue() {
        vhallMinkeeDialogueBattle = new List<List<DialogueLine>>();
        string filePath = path + "vhallMinkee";

        ConvertToDialogue(filePath, vhallMinkeeDialogueBattle);
    }
    #endregion

    #region Convert to Dialogue
    private void ConvertToDialogue(string filePath, List<List<DialogueLine>> allLines) {
        List<DialogueLine> lines = new List<DialogueLine>();

        string textFile = Resources.Load<TextAsset>(filePath).text;
        string[] fileLines = Regex.Split(textFile, "\n|\r|\r\n");

        foreach (string line in fileLines) {
            if (line.Equals("-")) {
                allLines.Add(lines);
                lines = new List<DialogueLine>();
                continue;
            }

            if (!line.Equals("")) {
                DialogueLine l = ConvertLine(line);
                lines.Add(l);
            }
        }

        allLines.Add(lines);
    }

    private DialogueLine ConvertLine(string line) {
        string[] arr = line.Split(':');
        string words = arr[1];

        string[] first = arr[0].Split(' ');
        string name = first[0];

        if (name.Equals("Narrator")) {
            return new DialogueLine(null, words);
        }

        if (first.Length == 1) {
            return new DialogueLine(GameInformation.instance.GetPlayerInfo(name), words);
        }

        DialoguePortrait portrait = ConvertToPortrait(first[1]);
        return new DialogueLine(GameInformation.instance.GetPlayerInfo(name), portrait, words);
    }

    private DialoguePortrait ConvertToPortrait(string portrait) {
        switch (portrait) {
            case "blushing":
                return DialoguePortrait.BLUSHING;
            case "DTF":
                return DialoguePortrait.DTF;
            case "angry":
                return DialoguePortrait.ANGRY;
            case "sad":
                return DialoguePortrait.SAD;
            case "happy":
                return DialoguePortrait.HAPPY;
            case "shock":
                return DialoguePortrait.SHOCK;
            case "wink":
                return DialoguePortrait.WINK;
            case "annoyed":
                return DialoguePortrait.ANNOYED;
            case "confused":
                return DialoguePortrait.CONFUSED;
            default:
                Debug.LogWarning("Couldn't find portrait: " + portrait);
                return DialoguePortrait.NEUTRAL;
        }
    }
    #endregion

    public static List<DialogueLine> GetBattleDialogueBetween(UnitID a, UnitID b) {
        UnitPair pair = RelationshipManager.GetRelationshipPair(a, b);

        switch (pair) {
            case UnitPair.MINKEEVHALL:
                return GetVhallMinkee();
            case UnitPair.PHYNNEMINKEE:
                return GetMinkeePhynne();
            case UnitPair.VHALLPHYNNE:
                return GetPhynneVhall();
        }

        Debug.LogError("Couldn't find battle dialogue between " + a + " and " + b);
        return null;
    }

    private static List<DialogueLine> GetMinkeePhynne() {
        int index = Random.Range(0, minkeePhynneDialogueBattle.Count);
        List<DialogueLine> lines = minkeePhynneDialogueBattle[index];
        minkeePhynneDialogueBattle.RemoveAt(index);

        return lines;
    }

    private static List<DialogueLine> GetPhynneVhall() {
        int index = Random.Range(0, phynneVhallDialogueBattle.Count);
        List<DialogueLine> lines = phynneVhallDialogueBattle[index];
        phynneVhallDialogueBattle.RemoveAt(index);

        return lines;
    }

    private static List<DialogueLine> GetVhallMinkee() {
        int index = Random.Range(0, vhallMinkeeDialogueBattle.Count);
        List<DialogueLine> lines = vhallMinkeeDialogueBattle[index];
        vhallMinkeeDialogueBattle.RemoveAt(index);

        return lines;
    }

    public static bool StillHasDialogue(UnitID a, UnitID b) {
        UnitPair pair = RelationshipManager.GetRelationshipPair(a, b);

        switch (pair) {
            case UnitPair.MINKEEVHALL:
                return vhallMinkeeDialogueBattle.Count > 0;
            case UnitPair.PHYNNEMINKEE:
                return minkeePhynneDialogueBattle.Count > 0;
            case UnitPair.VHALLPHYNNE:
                return phynneVhallDialogueBattle.Count > 0;
        }

        return false;
    }
}