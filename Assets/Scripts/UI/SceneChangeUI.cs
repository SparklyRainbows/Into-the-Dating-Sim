using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeUI : MonoBehaviour
{
    public void ToRestScene() {
        GameInformation.instance.GoToRestScene();
    }

    public void Quit() {
        GameInformation.instance.Quit();
    }

    public void ToMenu() {
        GameInformation.instance.SetBGMVolume(1);
        GameInformation.instance.GoToMainMenu();
    }

    public void ToMenuReset() {
        GameInformation.instance.Init();
        GameInformation.instance.SetBGMVolume(1);
        GameInformation.instance.GoToMainMenu();
    }
}
