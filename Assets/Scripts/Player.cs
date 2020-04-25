using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] float moveSpeed = 4f;

    private Vector3 _movement;
    private Rigidbody2D _rb2d;

    private void Awake() {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        _movement = new Vector3(horiz, vert, 0);
    }

    private void FixedUpdate() {
        _rb2d.MovePosition(transform.position + _movement * Time.fixedDeltaTime);
    }
}
