using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Police : MonoBehaviour {
    [SerializeField] Transform[] wayPoints;
    [SerializeField] float patrolTimePerLeg = 1.5f;
    [SerializeField] float wayPointPauseTime = 0.5f;
    [SerializeField] float wayPointFuzz = 0.1f;
    [SerializeField] AudioClip warningClip;

    private int _currentWP = 0;
    private GameController _gc;
    private BeachBBQ _game;
    private AudioSource _audio;

    private void Start() {
        _gc = GameController.Instance;
        _gc.OnBeginLevel += BeginLevel;
        _game = BeachBBQ.Instance;
        _audio = GetComponent<AudioSource>();
    }

    private void OnDestroy() {
        _gc.OnBeginLevel -= BeginLevel;
    }

    private void BeginLevel() {
        _currentWP = 0;
        transform.position = wayPoints[_currentWP].transform.position;
        StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine() {
        bool atWP = true;
        while (true) {
            if (_game.Playing && atWP) {
                yield return new WaitForSeconds(wayPointPauseTime);
                int nextWP = _currentWP + 1;
                if (nextWP >= wayPoints.Length) {
                    nextWP = 0;
                }
                // Make sure vehicle is pointing correctly
                Vector3 delta = wayPoints[nextWP].position - transform.position;
                // Assumes that movement is on a rectangle
                if (Mathf.Abs(delta.y) <= wayPointFuzz) {
                    // Moving left/right
                    if (delta.x >= 0) {
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                    } else {
                        transform.rotation = Quaternion.Euler(-180f, 0, 180);
                    }
                } else {
                    if (Mathf.Abs(delta.x) <= wayPointFuzz) {
                        // Moving left/right
                        if (delta.y >= 0) {
                            transform.rotation = Quaternion.Euler(0, 0, 90f);
                        } else {
                            transform.rotation = Quaternion.Euler(180f, 0, 90f);
                        }
                    }
                }
                atWP = false;
                LeanTween.move(gameObject, wayPoints[nextWP].position, patrolTimePerLeg).setOnComplete(() => { atWP = true; });
                _currentWP = nextWP;
                if (warningClip != null) {
                    _audio.PlayOneShot(warningClip);
                }
            } else {
                yield return null;
            }
        }
    }
}
