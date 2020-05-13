using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {

    public Point pos;
    public bool canMove;

    private SpriteRenderer renderer;
    private Color defaultColor = Color.white;
    private Color selectColor = Color.green;

    private void Start() {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void SetPosition(Vector2 position, Point point) {
        if (!canMove) {
            return;
        }

        transform.position = position;
        pos = point;
    }

    public Vector2 GetPosition() {
        return transform.position;
    }

    public void Selecting() {
        renderer.color = selectColor;
    }

    public void NotSelecting() {
        renderer.color = defaultColor;
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}

