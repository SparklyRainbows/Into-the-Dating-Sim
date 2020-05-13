using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static event EventHandler<InfoEventArgs<Point>> moveEvent;
    public static event EventHandler<EventArgs> selectEvent;
    public static event EventHandler<EventArgs> cancelEvent;
    public static event EventHandler<EventArgs> resetEvent;
    public static event EventHandler<EventArgs> autoLoseEvent;

    private Repeater _hor = new Repeater("Horizontal");
    private Repeater _ver = new Repeater("Vertical");

    private string select = "Select";
    private string cancel = "Cancel";

    private Cursor cursor;

    private void Start() {
        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
    }

    private void Update() {
        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");
        if (xMouse != 0 || yMouse != 0) {
            if (moveEvent != null) {
                moveEvent(this, new InfoEventArgs<Point>(cursor.pos));
            }
        }

        /*
        int x = _hor.Update();
        int y = _ver.Update();
        if (x != 0 || y != 0) {
            if (moveEvent != null) 
                moveEvent(this, new InfoEventArgs<Point>(cursor.pos + new Point(x, y)));
        }*/

        if (Input.GetButtonUp(select) || Input.GetMouseButtonDown(0)) {
            selectEvent(this, new EventArgs());
        }
        if (Input.GetButtonUp(cancel) || Input.GetMouseButtonDown(1)) {
            cancelEvent(this, new EventArgs());
        }
        if (Input.GetKeyUp(KeyCode.R)) {
            resetEvent(this, new EventArgs());
        }
        if (Input.GetKeyUp(KeyCode.F)) {
            autoLoseEvent(this, new EventArgs());
        }
    }

    private class Repeater {
        private const float threshold = 0.5f;
        private const float rate = 0.25f;
        private float _next;
        private bool _hold;
        private string _axis;

        public Repeater(string axisName) {
            _axis = axisName;
        }

        public int Update() {
            int retValue = 0;
            int value = Mathf.RoundToInt(Input.GetAxisRaw(_axis));

            if (value != 0) {
                if (Time.time > _next) {
                    retValue = value;
                    _next = Time.time + (_hold ? rate : threshold);
                    _hold = true;
                }
            } else {
                _hold = false;
                _next = 0;
            }

            return retValue;
        }
    }
}
