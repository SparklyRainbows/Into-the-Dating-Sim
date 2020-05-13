using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private GameObject _content;
    private GameObject _prevContent;
    public GameObject content {
        get {
            return _content;
        }

        set {
            if (value == null) {
                _content = _prevContent;
            } else {
                _prevContent = _content;
                _content = value;
            }
        }
    }

    public Point pos;

    public Tile prev;
    [HideInInspector] public int distance;

    private Vector2 tileExtents = new Vector2(1f, .5f);

    public bool isEnemyForecast;

    public Vector2 GetPosition() {
        Vector2 scenePos = Vector2.zero;

        scenePos.x = tileExtents.x * (pos.x - pos.y);
        scenePos.y = tileExtents.y * (pos.x + pos.y);

        return scenePos;
    }

    public void Match() {
        transform.localPosition = GetPosition();
    }

    public void SetPosition(Point p) {
        pos = p;
        Match();
    }

    public void SetPosition(Vector2 v) {
        int x = (int)((v.x / tileExtents.x + v.y / tileExtents.y) * 0.5f);
        int y = (int)((v.y / tileExtents.y - v.x / tileExtents.x) * 0.5f);
        SetPosition(new Point(x, y));
    }

    private void OnMouseEnter() {
        FindObjectOfType<Cursor>().SetPosition(GetPosition(), pos);
    }
}
