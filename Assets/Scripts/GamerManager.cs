﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
}

[RequireComponent(typeof(HealthManager))]
public class GamerManager : MonoBehaviour
{
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

    public GameObject NotificationSystemObject;

    private NotificationSystem NotificationSystem;

    // Start is called before the first frame update
    void Start()
    {
        NotificationSystem = NotificationSystemObject.GetComponent<NotificationSystem>();
        PauseCanvas.SetActive(false);
        GameCanvas.SetActive(true);
        m_healthManager = GetComponent<HealthManager>();

        //StartMiniGame(0);
        StartSpinner(0);
        StartMiniGame(1);
    }

    // Left side is 0, right side is 1
    void StartMiniGame(int side, GameObject gameOverride = null)
    {
        NotificationSystem.Alert(side, AlertLevel.Low);

        if (side < 0 || side > 1)
        {
            Debug.Log("No");
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
        // End old game stuff
        m_healthManager.AddHP(reward);
        m_currentGameManager[side].DoneGame -= FinishMiniGame;
        m_currentGameManager[side].DestroySelf();

        // Start new
        // Spinner flow goes here
        StartMiniGame(side);
    }

    public void Pause() {
        m_paused = true;
        m_healthManager.Pause();
        foreach(var gameManager in m_currentGameManager)
        {
            gameManager.Pause();
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
            gameManager.Unpause();
        }

        NotificationSystem.Unpause();
        PauseCanvas.SetActive(false);
        GameCanvas.SetActive(true);
    }

    private int PickNewGameIndex(int side)
    {
        var maxIndex = m_games.Count;
        var excludeIndex = m_currentGameIndex[side];
        var excludeCount = m_currentGameIndex[side] == -1 ? 0 : 1;
        var gameIndex = Random.Range(0, maxIndex - excludeCount);
        if (gameIndex == excludeIndex) {
            gameIndex += 1;
        }

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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_paused)
            return;

        m_difficultyModifier += DIFFICULTY_SCALE_SPEED * Time.deltaTime;
        var states = new List<HealthState>();
        // Check any spinners for health updates
        for(var i = 0; i < m_currentGame.Count; i++)
        {
            var game = m_currentGame[i];
            var spinManager = game.GetComponent<SpinManager>();
            if (spinManager == null)
            {
                continue;
            }

            switch(spinManager.SpinState)
            {
                case SpinState.ReallyFine:
                    NotificationSystem.CancelAlert(i, AlertLevel.High);
                    break;
                case SpinState.Fine:
                    NotificationSystem.CancelAlert(i, AlertLevel.High);
                    break;
                case SpinState.Bad:
                    NotificationSystem.IndefiniteAlert(i, AlertLevel.High);
                    break;
                case SpinState.ReallyBad:
                    NotificationSystem.IndefiniteAlert(i, AlertLevel.High);
                    break;
            }

            states.Add(spinManager.GetHealthState());
        }

        // Assuming we want the worst performance of all current spinners to be what affects our hp
        var worstState = HealthState.Fine;
        for (var i = 0; i < states.Count; i++)
        {
            var state = states[i];
            switch(worstState)
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
        }

        m_healthManager.SetHealthState(worstState);
    }
}
