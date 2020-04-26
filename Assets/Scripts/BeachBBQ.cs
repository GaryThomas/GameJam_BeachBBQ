using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private GameController _gc;

    public override void Awake() {
        base.Awake();
        _gc = GameController.Instance;
        foreach (Stat stat in stats) {
            stat.container.SetActive(false);
        }
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
