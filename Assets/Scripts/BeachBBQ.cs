using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] GameObject introScreen;
    [SerializeField] GameObject introFrame;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject winFrame;
    [SerializeField] TMP_Text winText;
    [SerializeField] int minCrowdSize = 10;
    [SerializeField] int maxCrowdSize = 40;
    [SerializeField] int minCrowdGathering = 4;
    [SerializeField] int maxCrowdGathering = 12;
    [SerializeField] float minTimeBetweenCrowds = 10.0f;
    [SerializeField] float maxTimeBetweenCrowds = 25.0f;
    [SerializeField] float crowdFieldSize = 2f;
    [SerializeField] GameObject[] crowdPlayers;
    [SerializeField] Transform[] crowdHotSpots;

    public bool Playing { get { return _playing; } }

    private GameController _gc;
    private bool _playing;
    private int _totalCrowdMembers = 0;
    private int _totalCrowdSize;
    private int _activeCrowdMembers;

    public override void Awake() {
        base.Awake();
        _gc = GameController.Instance;
        _gc.OnInitGame += InitGame;
        foreach (Stat stat in stats) {
            stat.container.SetActive(false);
        }
        introScreen.SetActive(true);
        introFrame.transform.localScale = Vector3.zero;
        winScreen.SetActive(false);
        winFrame.transform.localScale = Vector3.zero;
    }

    private void OnDestroy() {
        _gc.OnInitGame -= InitGame;
    }

    private void InitGame() {
        Debug.Log("BeachBBQ - Init Game");
        _totalCrowdSize = Random.Range(minCrowdSize, maxCrowdSize);
        LeanTween.scale(introFrame, Vector3.one, 0.5f).setDelay(1.0f).setEaseInExpo();
    }

    public void StartGame() {
        LeanTween.scale(introFrame, Vector3.zero, 0.5f).setOnComplete(() => {
            _gc.UpdateGameState(GameState.GameInitComplete);
            _playing = true;
            introScreen.SetActive(false);
            StartCoroutine(CrowdGenerator());
        });
    }

    private void Win() {
        _playing = false;
        winScreen.SetActive(true);
        winText.text = string.Format("{0} Made it to the beach!", GetStat(StatsType.Beached));
        LeanTween.scale(winFrame, Vector3.one, 0.5f).setDelay(1.0f).setEaseInExpo();

    }

    public void ReStartGame() {
        LeanTween.scale(winFrame, Vector3.zero, 0.5f).setOnComplete(() => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    IEnumerator CrowdGenerator() {
        while (_totalCrowdMembers < _totalCrowdSize) {
            Vector2 hotSpot = crowdHotSpots[Random.Range(0, crowdHotSpots.Length)].position;
            int size = Random.Range(minCrowdGathering, maxCrowdGathering + 1);
            for (int i = 0; i < size; i++) {
                Vector2 pos = hotSpot + Random.insideUnitCircle * crowdFieldSize;
                GameObject member = Instantiate(crowdPlayers[Random.Range(0, crowdPlayers.Length)], pos, Quaternion.identity);
            }
            _totalCrowdMembers += size;
            _activeCrowdMembers += size;
            yield return new WaitForSeconds(Random.Range(minTimeBetweenCrowds, maxTimeBetweenCrowds));
        }
    }

    public void CrowdMemberCaptured() {
        _activeCrowdMembers--;
        if (_activeCrowdMembers <= 0) {
            Debug.Log("You win!");
            Win();
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
        Debug.LogError("ChangeStat: Incorrect/Missing stat");
    }

    public int GetStat(StatsType type) {
        foreach (Stat stat in stats) {
            if (stat.type == type) {
                return stat.value;
            }
        }
        Debug.LogError("GetStat: Incorrect/Missing stat");
        return -1;
    }

    public int NumFollowers() {
        return GetStat(StatsType.Followers);
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
