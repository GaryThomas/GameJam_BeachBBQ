using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMember : MonoBehaviour {

    [SerializeField] float followSpeed = 2f;
    [SerializeField] float fleeSpeed = 6f;
    [SerializeField] float minDistance = 2f;  // Separation from player
    [SerializeField] float zombieTime = 15f; // Time keep following

    private bool _following;
    private bool _beached;
    private bool _fleeing;
    private bool _captured;
    private GameObject _player;
    private Rigidbody2D _rb2d;
    private SpriteRenderer _sr;
    private BeachBBQ _game;
    private float _zombieTimer;
    private Vector3 _startPos;

    private void Awake() {
        _rb2d = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _game = BeachBBQ.Instance;
    }

    private void FixedUpdate() {
        _zombieTimer -= Time.fixedDeltaTime;
        if (_following) {
            if (_zombieTimer <= 0) {
                _following = false;
                _game.ChangeStat(StatsType.Followers, -1);
                _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 1.0f);
            } else {
                float dist = Vector3.Distance(transform.position, _player.transform.position);
                if (dist > minDistance) {
                    Vector3 newPos = Vector3.MoveTowards(transform.position, _player.transform.position, followSpeed * Time.fixedDeltaTime);
                    _rb2d.MovePosition(newPos);
                }
            }
        } else if (_fleeing) {
            float dist = Vector3.Distance(transform.position, _startPos);
            if (dist > 0.01f) {
                Vector3 newPos = Vector3.MoveTowards(transform.position, _startPos, fleeSpeed * Time.fixedDeltaTime);
                _rb2d.MovePosition(newPos);
            } else {
                _fleeing = false;
            }
        } else {
            _rb2d.velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (_captured) {
            return;
        }
        if (other.gameObject.tag == "Player") {
            if (!_following && !_beached) {
                _following = true;
                _player = other.gameObject;
                _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0.75f);
                _zombieTimer = zombieTime;
                _game.ChangeStat(StatsType.Followers, +1);
                Debug.Log("Touched by player!");
            }
        } else if (other.gameObject.tag == "Beach") {
            _following = false;
            _beached = true;
            _fleeing = false;
            _rb2d.velocity = Vector3.zero;
            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 1.0f);
            _zombieTimer = zombieTime;
            _game.ChangeStat(StatsType.Followers, -1);
            _game.ChangeStat(StatsType.Beached, +1);
            Debug.Log("Made it to the beach!");
        } else if (other.gameObject.tag == "Police") {
            // Collected by police riot van
            if (_following) {
                _game.ChangeStat(StatsType.Followers, -1);
            }
            if (_beached) {
                _game.ChangeStat(StatsType.Beached, -1);
            }
            _captured = true;
            _game.ChangeStat(StatsType.Captured, +1);
            _game.CrowdMemberCaptured();
            Destroy(gameObject);
            Debug.Log("Picked up by the Police!");
        } else if (other.gameObject.tag == "Patrol") {
            if (_following) {
                _game.ChangeStat(StatsType.Followers, -1);
            }
            if (_beached) {
                _game.ChangeStat(StatsType.Beached, -1);
            }
            _fleeing = true;
            _following = false;
            _beached = false;
            _rb2d.velocity = Vector3.zero;
            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 1.0f);
            Debug.Log("Scared off by the police patrol!");
        }
    }
}
