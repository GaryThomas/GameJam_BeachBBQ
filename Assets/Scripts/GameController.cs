using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    GameInit,
    GameInitComplete,
    GameOver,
    BeginLevelComplete,
    PlayLevelComplete,
    EndLevelComplete,
}

public class GameController : Singleton<GameController> {
    public event Action OnInitGame;
    public event Action OnBeginLevel;
    public event Action OnPlayLevel;
    public event Action OnEndLevel;

    private GameState _state;

    public override void Awake() {
        base.Awake();
        // DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        StartCoroutine(Game());
    }

    IEnumerator Game() {
        yield return StartCoroutine(InitGame());
        while (_state != GameState.GameOver) {
            yield return StartCoroutine(BeginLevel());
            yield return StartCoroutine(PlayLevel());
            yield return StartCoroutine(EndLevel());
        }
    }

    IEnumerator InitGame() {
        Debug.Log("GameController: InitGame");
        _state = GameState.GameInit;
        if (OnInitGame != null) {
            OnInitGame.Invoke();
        } else {
            _state = GameState.GameInitComplete;
        }
        while (_state != GameState.GameInitComplete) {
            yield return null;
        }
    }

    IEnumerator BeginLevel() {
        Debug.Log("GameController: BeginLevel");
        if (OnBeginLevel != null) {
            OnBeginLevel.Invoke();
        } else {
            _state = GameState.BeginLevelComplete;
        }
        while (_state != GameState.BeginLevelComplete) {
            yield return null;
        }
    }

    IEnumerator PlayLevel() {
        Debug.Log("GameController: PlayLevel");
        if (OnPlayLevel != null) {
            OnPlayLevel.Invoke();
        } else {
            _state = GameState.PlayLevelComplete;
        }
        while (_state != GameState.PlayLevelComplete) {
            yield return null;
        }
    }

    IEnumerator EndLevel() {
        Debug.Log("GameController: EndLevel");
        if (OnEndLevel != null) {
            OnEndLevel.Invoke();
        } else {
            _state = GameState.EndLevelComplete;
        }
        while (_state != GameState.EndLevelComplete) {
            yield return null;
        }
        // TEMP
        UpdateGameState(GameState.GameOver);
    }

    public void UpdateGameState(GameState newState) {
        _state = newState;
    }
}
