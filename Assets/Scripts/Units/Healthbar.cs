using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour {
    public void SetHealth(int health, int maxHealth) {
        float percentHealth = health * 1.0f / maxHealth;
        percentHealth = Mathf.Clamp(percentHealth, 0, 1);

        Vector3 scale = transform.localScale;
        scale.x = percentHealth;
        StartCoroutine(ChangeHealthbar(scale));
    }

    private IEnumerator ChangeHealthbar(Vector3 scale) {
        float lerp = 0;

        while (lerp < 1) {
            Vector3 temp = transform.localScale;
            temp.x = Mathf.Lerp(temp.x, scale.x, lerp);
            temp.y = Mathf.Lerp(temp.y, scale.y, lerp);
            transform.localScale = temp;

            lerp += .1f;

            yield return null;
        }
    }
}
