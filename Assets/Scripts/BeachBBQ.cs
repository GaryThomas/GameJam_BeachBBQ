using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum StatsType {
    Followers,
    Captured,
    Beached,
}

[System.Serializable]
public struct Stat {
    public StatsType type;
    public int value;
    public GameObject container;
    public TMP_Text textField;
}

public class BeachBBQ : Singleton<BeachBBQ> {
    [SerializeField] Stat[] stats;
    [SerializeField] GameObject initScreen;
    [SerializeField] GameObject initFrame;

    public bool Playing { get { return _playing; } }

    private GameController _gc;
    private bool _playing;

    public override void Awake() {
        base.Awake();
        _gc = GameController.Instance;
        _gc.OnInitGame += InitGame;
        foreach (Stat stat in stats) {
            stat.container.SetActive(false);
        }
        initScreen.SetActive(true);
        initFrame.transform.localScale = Vector3.zero;
    }

    private void InitGame() {
        Debug.Log("BeachBBQ - Init Game");
        LeanTween.delayedCall(1.0f, () => {
            LeanTween.scale(initFrame, Vector3.one, 0.5f);
        });
    }

    public void StartGame() {
        LeanTween.scale(initFrame, Vector3.zero, 0.5f).setOnComplete(() => {
            _gc.UpdateGameState(GameState.GameInitComplete);
            _playing = true;
            initScreen.SetActive(false);
        });
    }

    public void ChangeStat(StatsType type, int delta) {
        for (int i = 0; i < stats.Length; i++) {
            if (stats[i].type == type) {
                stats[i].value += delta;
                UpdateUI();
                return;
            }
        }
        Debug.LogError("Incorrect/Missing stat");
    }

    private void UpdateUI() {
        foreach (Stat stat in stats) {
            if ((stat.value == 0) && stat.container.activeInHierarchy) {
                stat.container.SetActive(false);
            } else {
                if (!stat.container.activeInHierarchy) {
                    stat.container.SetActive(true);
                }
                stat.textField.text = stat.value.ToString();
            }
        }
    }
}
