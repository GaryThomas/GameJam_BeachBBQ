﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] AudioClip invitation;

    private Vector3 _movement;
    private Rigidbody2D _rb2d;
    private AudioSource _audio;
    private BeachBBQ _game;

    private void Awake() {
        _rb2d = GetComponent<Rigidbody2D>();
        _game = BeachBBQ.Instance;
        _audio = GetComponent<AudioSource>();
    }

    private void Update() {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        _movement = new Vector3(horiz, vert, 0);
    }

    private void FixedUpdate() {
        _rb2d.MovePosition(transform.position + _movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Player touched " + other.gameObject.tag + ", followers: " + _game.NumFollowers().ToString());
        if (other.gameObject.tag == "CrowdMember") {
            if (!_audio.isPlaying) {
                _audio.PlayOneShot(invitation);
            }
        }
    }
}
