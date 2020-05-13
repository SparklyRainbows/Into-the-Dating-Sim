using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
    public RelationshipUI fromUI;

    private RestController restController;
    private BattleController battleController;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public Image left;
    public Image right;
    public Queue<Sprite> leftSprites = new Queue<Sprite>();
    public Queue<Sprite> rightSprites = new Queue<Sprite>();
    private string leftUnit;
    private string rightUnit;

    private Queue<string> sentences = new Queue<string>();
    private Queue<string> names = new Queue<string>();

    private AudioSource audio;
    private int lineNum;
    private UnitPair pair;
    private RelationshipRank rank;

    private void Start() {
        restController = GetComponentInParent<RestController>();
        battleController = GetComponentInParent<BattleController>();

        audio = GetComponent<AudioSource>();

        gameObject.SetActive(false);
    }

    public void StartDialogue(List<DialogueLine> dialogue, RelationshipRank rank, UnitPair pair) {
        gameObject.SetActive(true);

        sentences.Clear();
        names.Clear();

        this.rank = rank;
        this.pair = pair;

        if (rank != RelationshipRank.SPICY) {
            GameInformation.instance.SetBGMVolume(.3f);
        } else {
            GameInformation.instance.PlayLove();
        }
        
        SetDialogueUnits(dialogue);
        
        foreach (DialogueLine line in dialogue) {
            sentences.Enqueue(line.sentence);
            names.Enqueue(line.name);
        }

        lineNum = 1;
        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (sentences.Count == 0) {
            EndDialogue();
            return;
        }
        
        string sentence = sentences.Dequeue();
        string name = names.Dequeue();

        if (audio != null) {
            audio.Stop();
        }
        if (!name.Equals("")) {
            PlayVA();
            lineNum++;
        }
        
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

        nameText.text = name;

        left.color = Color.gray;
        right.color = Color.gray;

        if (name.Equals("Phynne")) {
            nameText.font = GameInformation.instance.edgyFont;
            dialogueText.font = GameInformation.instance.edgyFont;
        } else {
            nameText.font = GameInformation.instance.defaultFont;
            dialogueText.font = GameInformation.instance.defaultFont;
        }

        if (name.Equals(leftUnit)) {
            left.sprite = leftSprites.Dequeue();
            left.color = Color.white;
        } else if (name.Equals(rightUnit)) {
            right.sprite = rightSprites.Dequeue();
            right.color = Color.white;
        }
    }
    
    IEnumerator TypeSentence(string sentence) {
        bool italics = false;
        bool foundOpen = false;
        bool edgy = false;
        bool normal = false;

        dialogueText.text = "";
        
        foreach (char letter in sentence.ToCharArray()) {
            if (letter == '<') {
                foundOpen = true;
                continue;
            }
            if (letter == '/') {
                italics = false;
                edgy = false;
                normal = false;
                continue;
            }
            if (letter == '>') {
                foundOpen = false;
                continue;
            }

            if (foundOpen) {
                if (letter == 'i') {
                    italics = true;
                } else if (letter == 'e') {
                    edgy = true;
                } else if (letter == 'n') {
                    normal = true;
                }
                continue;
            }

            string newLetter = letter.ToString();

            if (italics) {
                newLetter = "<i>" + newLetter + "</i>";
            }
            if (normal) {
                newLetter = "<font=\"pixel SDF\">" + newLetter + "</font>";
            }
            if (edgy) {
                newLetter = "<font=\"pain SDF\">" + newLetter + "</font>";
            }

            dialogueText.text += newLetter;
            yield return null;
        }
    }

    void EndDialogue() {
        if (fromUI != null) {
            fromUI.FinishSupportConversation();
        }

        gameObject.SetActive(false);

        if (restController != null) {
            restController.ChangeState<MenuState>();
        } else if (battleController != null) {
            battleController.ChangeState<SelectUnitState>();
        }

        if (fromUI != null) {
            fromUI.UpdateRelationshipUI();
        }
        fromUI = null;

        if (rank != RelationshipRank.SPICY) {
            GameInformation.instance.SetBGMVolume(1f);
        } else {
            GameInformation.instance.PlayMenu();
        }
    }

    private void SetDialogueUnits(List<DialogueLine> lines) {
        leftSprites.Clear();
        rightSprites.Clear();

        List<PlayerUnitInfo> units = new List<PlayerUnitInfo>();

        foreach (DialogueLine line in lines) {
            if (line.info == null) {
                continue;
            }

            if (!units.Contains(line.info)) {
                units.Add(line.info);
            }
        }

        if (units.Count != 2) {
            Debug.LogWarning("Dialogue system doesn't support this number of speaking units: " + units.Count);
        }

        leftUnit = units[0].name;
        rightUnit = units[1].name;
        foreach (DialogueLine line in lines) {
            if (line.info == null) {
                continue;
            }

            if (line.info.name.Equals(leftUnit)) {
                leftSprites.Enqueue(line.GetPortrait());
            } else if (line.info.name.Equals(rightUnit)) {
                rightSprites.Enqueue(line.GetPortrait());
            }
        }

        left.sprite = units[0].defaultBust;
        right.sprite = units[1].defaultBust;
    }

    private void PlayVA() {
        if (audio == null) {
            return;
        }

        AudioClip clip = DatingSoundManager.GetClip(pair, RelationshipManager.AsString(rank), lineNum);

        if (clip != null) {
            audio.clip = clip;
            audio.Play();
        }
    }
}
