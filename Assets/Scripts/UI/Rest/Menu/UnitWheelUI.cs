using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitWheelUI : MonoBehaviour
{
    public Image[] unitImages;
    public Text nameText;

    private Sprite[] sprites;
    private Animator animator;

    private MenuUI menuUI;

    private void Start() {
        sprites = new Sprite[GameInformation.instance.playerInfo.Count];

        for (int i = 0; i < sprites.Length; i++) {
            sprites[i] = GameInformation.instance.playerInfo[i].unitSprite;
        }

        animator = GetComponent<Animator>();
        menuUI = GetComponentInParent<MenuUI>();

        animator.enabled = false;
    }

    public void SpinLeft(int currentUnit) {
        StartCoroutine(Spin("SpinLeft", currentUnit, true));
    }

    public void SpinRight(int currentUnit) {
        StartCoroutine(Spin("SpinRight", currentUnit, true));
    }

    private IEnumerator Spin(string trigger, int currentUnit, bool left) {
        //animator.SetTrigger(trigger);

        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - .2f);
        yield return null;

        unitImages[0].sprite = sprites[currentUnit];
        if (!left) {
            unitImages[1].sprite = sprites[menuUI.GetLeft()];
            unitImages[2].sprite = sprites[menuUI.GetRight()];
        } else {
            unitImages[1].sprite = sprites[menuUI.GetRight()];
            unitImages[2].sprite = sprites[menuUI.GetLeft()];
        }

        string name = "";
        switch (currentUnit) {
            case 0:
                name = "Empress Min Kee";
                break;
            case 1:
                name = "Phynne";
                break;
            case 2:
                name = "Count von Vhall";
                break;
        }

        nameText.text = name;
    }
}
