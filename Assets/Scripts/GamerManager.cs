using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class MiniGameManager :  MonoBehaviour
{
    
    protected int m_side;
    protected float m_timer = 0;
    protected bool m_paused = false;

    public UnityAction<int, int> DoneGame;
    protected GameObject m_gameArea;

    public virtual void StartGame(int side, float difficulty, GameObject gameArea)
    {
        m_side = side;
        m_gameArea = gameArea;
        m_gameArea.GetComponentInChildren<Text>().text = GameName();
    }

    public virtual void DestroySelf() { Destroy(this.gameObject); }

    public virtual void Pause()
    {
        m_paused = true;
    }
    public virtual void Unpause()
    {
        m_paused = false;
    }

    protected virtual void FixedUpdate()
    {
        if (m_paused)
        {
            return;
        }

        m_timer += 1 * Time.deltaTime;
    }

    protected virtual string GameName()
    {
        return "DEFAULT GAME";
    }
}

[RequireComponent(typeof(HealthManager))]
public class GamerManager : MonoBehaviour
{
    private int m_score;

    // Constants
    private readonly float DIFFICULTY_SCALE_SPEED = 0.01f; // arbitrary units per second

    // Variables
    [SerializeField] private List<GameObject> m_games;
    [SerializeField] private GameObject m_spinnerPrefab;
    
    private HealthManager m_healthManager;

    // 0 is left, 1 is right
    [SerializeField] private List<GameObject> m_gameArea;

    private List<GameObject> m_currentGame = new List<GameObject>() { null, null };

    private List<MiniGameManager> m_currentGameManager = new List<MiniGameManager>() { null, null };

    private List<int> m_currentGameIndex = new List<int>(){ -1, -1 };

    [SerializeField] private float m_difficultyModifier = 0; //Only serialized for editor viewing purposes

    private bool m_paused = false;

    public GameObject GameCanvas;
    public GameObject PauseCanvas;
    public GameObject DeathCanvas;

    public GameObject NotificationSystemObject;

    private NotificationSystem NotificationSystem;

    private bool waitingForNewGame = false;

    private float startNewGameTimer = -1f;

    private const float START_NEW_GAME_TIMER_INTERVAL = 1f;

    private int newGameSide;

    private const float DOUBLE_SPINNER_TIMER_INTERVAL = 30f;

    private float doubleSpinnerTimer = DOUBLE_SPINNER_TIMER_INTERVAL;

    private bool doingDoubleSpinner;

    private float leaveSpinnerTimer;

    private const float LEAVE_SPINNER_TIMER_INTERVAL = 5f;

    private float gameStartTime;

    public GameObject ScoreObj;

    private Text ScoreText;

    public GameObject DeathScoreObj;

    private Text DeathScoreText;

    private bool tutorialOver;

    private int previousGameIndex = -1;

    // TODO when you start a spinner, it needs a short grace period where it doesnt penalize you for not spinning

    // Start is called before the first frame update
    private void Awake()
    {
        NotificationSystem = NotificationSystemObject.GetComponent<NotificationSystem>();
        PauseCanvas.SetActive(false);
        GameCanvas.SetActive(true);
        DeathCanvas.SetActive(false);
        m_healthManager = GetComponent<HealthManager>();
        ScoreText = ScoreObj.GetComponent<Text>();
        DeathScoreText = DeathScoreObj.GetComponent<Text>();
    }

    void Start()
    {
        for (var i = 0; i < 2; i++)
        {
            m_gameArea[i].GetComponentInChildren<Text>().text = "";
        }
        tutorialOver = false;
        m_healthManager.Pause();
        StartSpinner(0);
        m_score = 0;

        CardManager.timesSeen = 0;
        ShapesManager.timesSeen = 0;
        BasketballManager.timesSeen = 0;
        MoleGameManager.timesSeen = 0;
    }

    // Left side is 0, right side is 1
    void StartMiniGame(int side, GameObject gameOverride = null)
    {
        // NotificationSystem.Alert(side, AlertLevel.Low);

        if (side < 0 || side > 1)
        {
            return;
        }

        if (gameOverride == null)
        {
            var gameIndex = PickNewGameIndex(side);
            m_currentGameIndex[side] = gameIndex;
            m_currentGame[side] = Instantiate(m_games[gameIndex], new Vector2(0, 0), Quaternion.identity, m_gameArea[side].transform);
        } else
        {
            m_currentGame[side] = Instantiate(gameOverride, new Vector2(0, 0), Quaternion.identity, m_gameArea[side].transform);
        }

        m_currentGame[side].transform.localPosition = new Vector2(0, 0);
        m_currentGameManager[side] = m_currentGame[side].GetComponent<MiniGameManager>();
        m_currentGameManager[side].DoneGame += FinishMiniGame;
        m_currentGameManager[side].StartGame(side, m_difficultyModifier, m_gameArea[side]);
    }

    void StartSpinner(int side)
    {
        StartMiniGame(side, m_spinnerPrefab);
    }

    void FinishMiniGame(int side, int reward)
    {
        FindObjectOfType<SoundManager>().Play("FinishedMinigame");
        m_gameArea[side].GetComponentInChildren<Text>().text = "NICE!";
        // End old game stuff
        m_healthManager.AddHP(reward);
        m_currentGameIndex[side] = -1;
        m_currentGameManager[side].DoneGame -= FinishMiniGame;
        m_currentGameManager[side].DestroySelf();

        // Set timer to start new minigame, or do spinner switching stuff here
        waitingForNewGame = true;
        startNewGameTimer = START_NEW_GAME_TIMER_INTERVAL;
        newGameSide = side;
        NotificationSystem.CancelAlert(side, AlertLevel.High);
    }

    public void Pause() {
        m_paused = true;
        m_healthManager.Pause();
        foreach(var gameManager in m_currentGameManager)
        {
            if (gameManager != null)
            {
                gameManager.Pause();
            }
        }

        NotificationSystem.Pause();
        PauseCanvas.SetActive(true);
        GameCanvas.SetActive(false);
    }

    public void Unpause() {
        m_paused = false;
        m_healthManager.Unpause();
        foreach(var gameManager in m_currentGameManager)
        {
            if (gameManager != null)
            {
                gameManager.Unpause();
            }
        }

        NotificationSystem.Unpause();
        PauseCanvas.SetActive(false);
        GameCanvas.SetActive(true);
    }

    private int PickNewGameIndex(int side)
    {
        /*
         * Pretty sure this is just wrong but maybe there's hidden functionality I didn't understand
        var maxIndex = m_games.Count;
        var excludeIndex = m_currentGameIndex[side];
        var excludeCount = m_currentGameIndex[side] == -1 ? 0 : 1;
        var gameIndex = Random.Range(0, maxIndex - excludeCount);
        if (gameIndex == excludeIndex) {
            gameIndex += 1;
        }
        */

        var maxIndex = m_games.Count;
        var excludeIndex = previousGameIndex;
        var excludeCount = (previousGameIndex == -1 || m_games.Count <= 1) ? 0 : 1;
        var gameIndex = Random.Range(0, maxIndex - excludeCount);
        if (gameIndex >= excludeIndex && excludeCount == 1)
        {
            gameIndex += 1;
        }

        previousGameIndex = gameIndex;

        return gameIndex;
    }

    void Update() {
        // Check if Back was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (m_paused) {
                Unpause();
            } else {
                Pause();
            }
            // Quit the application
            // SceneManager.LoadScene(0);
            // Quit the application
            // Application.Quit();
        }
    }

    int doggy = 1;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_paused)
            return;

        if (!tutorialOver)
        {
            var spinManager = m_currentGame[0].GetComponent<SpinManager>();
            if (spinManager.VelocityManager.GetCurrentVelocity() > 50f)
            {
                tutorialOver = true;
                m_healthManager.Unpause();
                gameStartTime = Time.time;
                StartMiniGame(1);
            }
        }

        if (tutorialOver)
        {
            m_score = (int)(Time.time - gameStartTime);
            var minutes = (int)(m_score / 60);
            var seconds = m_score % 60;
            ScoreText.text = $"{minutes}:{seconds:00}";

            if (m_healthManager.GetHP() < 0)
            {
                Die();
            }

            m_difficultyModifier += DIFFICULTY_SCALE_SPEED * Time.deltaTime;
            m_healthManager.CurrentDifficultyMultiplier = m_difficultyModifier;
            var states = new List<HealthState>();
            // Check any spinners for health updates
            for (var i = 0; i < m_currentGame.Count; i++)
            {
                var game = m_currentGame[i];
                if (game == null)
                {
                    continue;
                }

                var spinManager = game.GetComponent<SpinManager>();
                if (spinManager == null)
                {
                    continue;
                }

                switch (spinManager.SpinState)
                {
                    case SpinState.ReallyFine:
                        if (spinManager.VelocityManager.GetCurrentVelocity() > 50f)
                        {
                            NotificationSystem.CancelAlert(i, AlertLevel.High);
                        }
                        
                        break;
                    case SpinState.Fine:
                        if (spinManager.VelocityManager.GetCurrentVelocity() > 50f)
                        {
                            NotificationSystem.CancelAlert(i, AlertLevel.High);
                        }

                        break;
                    case SpinState.Bad:
                        if (spinManager.VelocityManager.GetCurrentVelocity() < 25f)
                        {
                            NotificationSystem.IndefiniteAlert(i, AlertLevel.High);
                        }

                        break;
                    case SpinState.ReallyBad:
                        if (spinManager.VelocityManager.GetCurrentVelocity() < 25f)
                        {
                            NotificationSystem.IndefiniteAlert(i, AlertLevel.High);
                        }

                        break;
                }

                states.Add(spinManager.GetHealthState());
            }

            // Assuming we want the worst performance of all current spinners to be what affects our hp
            var worstState = HealthState.Fine;
            /* for (var i = 0; i < states.Count; i++)
             {
                 var state = states[i];
                 switch (worstState)
                 {
                     case HealthState.Fine:
                         if (state == HealthState.NotFine || state == HealthState.Empty)
                         {
                             worstState = state;
                         }
                         break;
                     case HealthState.NotFine:
                         if (state == HealthState.Empty)
                         {
                             worstState = state;
                         }
                         break;
                     case HealthState.Empty:
                         break;
                 }
             }*/



            var current = doggy == 1 ? 0 : 1;
            var hello105 = states.Count == 1 ? 0 : current;

            m_healthManager.SetHealthState(states[hello105]);

            if (waitingForNewGame)
            {
                startNewGameTimer -= 1 * Time.fixedDeltaTime;
                if (startNewGameTimer < 0f)
                {
                    waitingForNewGame = false;
                    if (doubleSpinnerTimer < 0f)
                    {
                        StartSpinner(newGameSide);
                        leaveSpinnerTimer = LEAVE_SPINNER_TIMER_INTERVAL;
                        doingDoubleSpinner = true;
                        newGameSide = (newGameSide + 1) % 2;
                    }
                    else
                    {
                        StartMiniGame(newGameSide);
                    }
                }
            }

            if (doingDoubleSpinner)
            {
                leaveSpinnerTimer -= 1 * Time.fixedDeltaTime;
                if (leaveSpinnerTimer < 0f)
                {
                    doingDoubleSpinner = false;
                    doubleSpinnerTimer = DOUBLE_SPINNER_TIMER_INTERVAL;
                    FinishMiniGame(newGameSide, 50);
                    doggy = doggy * -1;
                }
            }

            doubleSpinnerTimer -= 1 * Time.fixedDeltaTime;
        }
    }

    private void Die()
    {
        m_paused = true;
        DeathCanvas.SetActive(true);
        GameCanvas.SetActive(false);
        PauseCanvas.SetActive(false); // God i hope i dont need you
        var topScoreManager = FindObjectOfType<TopScoreManager>();
        var score = new Score() { name = "YOU", score = m_score };
        topScoreManager.RegisterTopScore(score);
        topScoreManager.SaveTopScores();

        var minutes = (int)(m_score / 60);
        var seconds = m_score % 60;
        DeathScoreText.text = $"YOUR SCORE: {minutes}:{seconds:00}";
    }
}
