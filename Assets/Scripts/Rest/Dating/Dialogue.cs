using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class Dialogue : MonoBehaviour {
    private string path = "Dialogue/";

    private static PlayerUnitInfo minkee;
    private static PlayerUnitInfo phynne;
    private static PlayerUnitInfo vhall;

    private static Dictionary<RelationshipRank, List<DialogueLine>> minkeePhynneDialogue;
    private static Dictionary<RelationshipRank, List<DialogueLine>> phynneVhallDialogue;
    private static Dictionary<RelationshipRank, List<DialogueLine>> vhallMinkeeDialogue;

    private void Start() {
        RestController controller = GetComponent<RestController>();
        minkee = GameInformation.instance.GetPlayerInfo(UnitID.MINKEE);
        phynne = GameInformation.instance.GetPlayerInfo(UnitID.PHYNNE);
        vhall = GameInformation.instance.GetPlayerInfo(UnitID.VHALL);

        InitMinkeePhynneDialogue();
        InitPhynneVhallDialogue();
        InitVhallMinkeeDialogue();
    }

    #region Dialogue Init
    void InitMinkeePhynneDialogue() {
        minkeePhynneDialogue = new Dictionary<RelationshipRank, List<DialogueLine>>();

        string filePath = path + "minkeePhynne/";

        List<DialogueLine> c = ConvertToDialogue(filePath + "C");
        List<DialogueLine> b = ConvertToDialogue(filePath + "B");
        List<DialogueLine> a = ConvertToDialogue(filePath + "A");
        List<DialogueLine> s = ConvertToDialogue(filePath + "S");

        minkeePhynneDialogue.Add(RelationshipRank.CRUDE, c);
        minkeePhynneDialogue.Add(RelationshipRank.BLAND, b);
        minkeePhynneDialogue.Add(RelationshipRank.APPETIZING, a);
        minkeePhynneDialogue.Add(RelationshipRank.SPICY, s);
    }
    
    void InitPhynneVhallDialogue() {
        phynneVhallDialogue = new Dictionary<RelationshipRank, List<DialogueLine>>();

        string filePath = path + "phynneVhall/";

        List<DialogueLine> c = ConvertToDialogue(filePath + "C");
        List<DialogueLine> b = ConvertToDialogue(filePath + "B");
        List<DialogueLine> a = ConvertToDialogue(filePath + "A");
        List<DialogueLine> s = ConvertToDialogue(filePath + "S");

        phynneVhallDialogue.Add(RelationshipRank.CRUDE, c);
        phynneVhallDialogue.Add(RelationshipRank.BLAND, b);
        phynneVhallDialogue.Add(RelationshipRank.APPETIZING, a);
        phynneVhallDialogue.Add(RelationshipRank.SPICY, s);
    }
    
    void InitVhallMinkeeDialogue() {
        vhallMinkeeDialogue = new Dictionary<RelationshipRank, List<DialogueLine>>();

        string filePath = path + "vhallMinkee/";

        List<DialogueLine> c = ConvertToDialogue(filePath + "C");
        List<DialogueLine> b = ConvertToDialogue(filePath + "B");
        List<DialogueLine> a = ConvertToDialogue(filePath + "A");
        List<DialogueLine> s = ConvertToDialogue(filePath + "S");

        vhallMinkeeDialogue.Add(RelationshipRank.CRUDE, c);
        vhallMinkeeDialogue.Add(RelationshipRank.BLAND, b);
        vhallMinkeeDialogue.Add(RelationshipRank.APPETIZING, a);
        vhallMinkeeDialogue.Add(RelationshipRank.SPICY, s);
    }
    #endregion

    #region Convert to Dialogue
    private List<DialogueLine> ConvertToDialogue(string filePath) {
        List<DialogueLine> lines = new List<DialogueLine>();

        string textFile = Resources.Load<TextAsset>(filePath).text;
        string[] fileLines = Regex.Split(textFile, "\n|\r|\r\n");

        foreach(string line in fileLines) {
            if (!line.Equals("")) {
                DialogueLine l = ConvertLine(line);
                lines.Add(l);
            }
        }

        return lines;
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

    public static List<DialogueLine> GetDialogueBetween(UnitID a, UnitID b) {
        RelationshipRank rank = RelationshipManager.GetRelationshipBetween(a, b).GetNextRank();
        UnitPair pair = RelationshipManager.GetRelationshipPair(a, b);

        switch (pair) {
            case UnitPair.MINKEEVHALL:
                return vhallMinkeeDialogue[rank];
            case UnitPair.PHYNNEMINKEE:
                return minkeePhynneDialogue[rank];
            case UnitPair.VHALLPHYNNE:
                return phynneVhallDialogue[rank];
        }

        Debug.LogError("Couldn't find relationship between " + a + " and " + b);
        return null;
    }
}

[System.Serializable]
public class DialogueLine {
    public PlayerUnitInfo info;

    public DialoguePortrait portrait;
    public string name {
        get {
            if (info != null) {
                return info.name;
            }
            return "";
        }
    }
    public string sentence;

    public DialogueLine(PlayerUnitInfo info, string sentence) {
        this.info = info;
        this.sentence = sentence;
    }

    public DialogueLine(PlayerUnitInfo info, DialoguePortrait portrait, string sentence) : this(info, sentence) {
        this.portrait = portrait;
    }

    public Sprite GetPortrait() {
        switch(portrait) {
            case DialoguePortrait.NEUTRAL:
                return info.defaultBust;
            case DialoguePortrait.BLUSHING:
                return info.blushing;
            case DialoguePortrait.ANGRY:
                return info.angry;
            case DialoguePortrait.HAPPY:
                return info.happy;
            case DialoguePortrait.SAD:
                return info.sad;
            case DialoguePortrait.SHOCK:
                return info.shocked;
            default:
                Debug.LogWarning("Bust not found: " + portrait);
                return info.defaultBust;
        }
    }
}

public enum DialoguePortrait {
    NEUTRAL,
    BLUSHING,
    DTF,
    ANGRY,
    SAD,
    HAPPY,
    SHOCK,
    WINK,
    ANNOYED,
    CONFUSED
}
 