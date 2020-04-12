using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController> {
    private bool _ready = false;
    private bool _playing = false;

    private void Start() {
        StartCoroutine(Game());
    }

    IEnumerator Game() {
        yield return StartCoroutine(BeginLevel());
        yield return StartCoroutine(PlayLevel());
        yield return StartCoroutine(EndLevel());
    }

    IEnumerator BeginLevel() {
        yield return new WaitForSeconds(2f);
    }

    IEnumerator PlayLevel() {
        yield return new WaitForSeconds(2f);
    }

    IEnumerator EndLevel() {
        yield return new WaitForSeconds(2f);
    }
}
