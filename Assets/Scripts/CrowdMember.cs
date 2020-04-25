using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMember : MonoBehaviour {

    [SerializeField] float followSpeed = 2f;
    [SerializeField] float minDistance = 2f;  // Separation from player
    [SerializeField] float zombieTime = 15f; // Time keep following

    private bool _following;
    private bool _beached;
    private GameObject _player;
    private Rigidbody2D _rb2d;
    private SpriteRenderer _sr;
    private float _zombieTimer;

    private void Awake() {
        _rb2d = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        _zombieTimer -= Time.fixedDeltaTime;
        if (_zombieTimer <= 0) {
            _following = false;
            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 1.0f);
        }
        if (_following) {
            float dist = Vector3.Distance(transform.position, _player.transform.position);
            if (dist > minDistance) {
                Vector3 newPos = Vector3.MoveTowards(transform.position, _player.transform.position, followSpeed * Time.fixedDeltaTime);
                _rb2d.MovePosition(newPos);
            }
        } else {
            _rb2d.velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            if (!_following && !_beached) {
                _following = true;
                _player = other.gameObject;
                _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0.75f);
                _zombieTimer = zombieTime;
                Debug.Log("Touched by player!");
            }
        } else if (other.gameObject.tag == "Beach") {
            _following = false;
            _beached = true;
            _rb2d.velocity = Vector3.zero;
            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 1.0f);
            _zombieTimer = zombieTime;
            Debug.Log("Made it to the beach!");
        }
    }
}
