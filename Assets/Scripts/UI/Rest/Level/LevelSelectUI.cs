using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
    public GameObject levelPrefab;

    private void Start() {
        for (int i = 0; i < GameInformation.instance.totalLevels; i++) {
            GameObject temp = Instantiate(levelPrefab, transform);
            temp.GetComponent<LevelUI>().SetLevel(i + 1);
        }
    }
}
