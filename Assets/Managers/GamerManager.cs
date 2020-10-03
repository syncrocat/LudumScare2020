using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class MiniGameManager :  MonoBehaviour
{
    public UnityAction<int> DoneGame;
    public virtual void StartGame(float difficulty) { }
    public virtual void DestroySelf() { Destroy(this.gameObject); }

    public virtual void Pause() { }
    public virtual void Unpause() { }
}

[RequireComponent(typeof(HealthManager))]
public class GamerManager : MonoBehaviour
{
    // Constants
    private readonly float DIFFICULTY_SCALE_SPEED = 0.01f; // arbitrary units per second

    // Variables
    [SerializeField] private List<GameObject> m_games;
    private HealthManager m_healthManager;

    private GameObject m_currentGame;
    private MiniGameManager m_currentGameManager;
    private int m_currentGameIndex = -1;
    [SerializeField] private float m_difficultyModifier = 0; //Only serialized for editor viewing purposes

    private bool m_paused = false;

    // Start is called before the first frame update
    void Start()
    {
        m_healthManager = GetComponent<HealthManager>();
        StartMiniGame();
    }

    void StartMiniGame()
    {
        //  Start new game stuff
        m_currentGameIndex = PickNewGameIndex();
        m_currentGame = Instantiate(m_games[m_currentGameIndex], new Vector3(0, 0, 0), Quaternion.identity);
        m_currentGameManager = m_currentGame.GetComponent<MiniGameManager>();
        m_currentGameManager.DoneGame += FinishMiniGame;
        m_currentGameManager.StartGame(m_difficultyModifier);
    }

    void FinishMiniGame(int reward)
    {
        // End old game stuff
        m_healthManager.AddHP(reward);
        m_currentGameManager.DoneGame -= FinishMiniGame;
        m_currentGameManager.DestroySelf();

        // Start new
        StartMiniGame();
    }

    public void Pause() {
        m_paused = true;
        m_currentGameManager.Pause();
    }

    public void Unpause() {
        m_paused = false;
        m_currentGameManager.Unpause();
    }

    private int PickNewGameIndex()
    {
        int maxIndex = m_games.Count;
        if (m_currentGameIndex != -1 && m_games.Count > 1) maxIndex -= 1;
        int gameIndex = Random.Range(0, maxIndex);

        if  (m_currentGameIndex != -1 && m_games.Count > 1 && gameIndex >= m_currentGameIndex)
        {
            gameIndex += 1;
        }

        return gameIndex;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_paused)
        {
            m_difficultyModifier += DIFFICULTY_SCALE_SPEED * Time.deltaTime; 
        }
    }
}
