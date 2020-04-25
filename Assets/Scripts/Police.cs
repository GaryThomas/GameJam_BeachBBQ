using System.Collections;
using UnityEngine;

public class Police : MonoBehaviour {
    [SerializeField] Transform[] wayPoints;
    [SerializeField] float patrolTimePerLeg = 1.5f;
    [SerializeField] float wayPointPauseTime = 0.5f;
    [SerializeField] float wayPointFuzz = 0.1f;

    private int _currentWP = 0;
    private GameController _gc;
    private Vector3 _oldPos;

    private void Start() {
        _gc = GameController.Instance;
        _gc.OnBeginLevel += BeginLevel;
    }

    private void BeginLevel() {
        _currentWP = 0;
        transform.position = wayPoints[_currentWP].transform.position;
        _oldPos = transform.position;
        StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine() {
        bool atWP = true;
        while (true) {
            if (atWP) {
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
                        transform.rotation = Quaternion.Euler(0, 90f, 0);
                    } else {
                        transform.rotation = Quaternion.Euler(0, -90f, 0);
                    }
                } else {
                    if (Mathf.Abs(delta.x) <= wayPointFuzz) {
                        // Moving left/right
                        if (delta.y >= 0) {
                            transform.rotation = Quaternion.Euler(-90f, 0, 90f);
                        } else {
                            transform.rotation = Quaternion.Euler(90f, 0, -90f);
                        }
                    }
                }
                atWP = false;
                LeanTween.move(gameObject, wayPoints[nextWP].position, patrolTimePerLeg).setOnComplete(() => { atWP = true; });
                _currentWP = nextWP;
            } else {
                yield return null;
            }
        }
    }
}
