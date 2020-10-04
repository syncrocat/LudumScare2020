using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{

    // Constants 
    readonly float MAX_HEALTH = 100;
    readonly float FINE_DRAIN_RATE = 5;
    readonly float NOT_FINE_DRAIN_RATE = 10;
    readonly float EMPTY_DRAIN_RATE = 20;

    public Text DebugText;

    // Variables
    [SerializeField] private float m_health; //Only serialized for editor viewing purposes
    [SerializeField] private RectTransform m_healthbarMask;
    private float m_healthbarMaskHeight;
    private float m_drainRate;

    private bool m_paused;

    public float CurrentDifficultyMultiplier;

    // Start is called before the first frame update
    void Awake()
    {
        m_paused = false;
        m_health = MAX_HEALTH;
        m_drainRate = FINE_DRAIN_RATE;
    }

    private void Start()
    {
        m_healthbarMaskHeight = m_healthbarMask.rect.height;
    }

    void FixedUpdate()
    {
        if (m_paused)
        {
            return;
        }

        float tt = Time.deltaTime;


        // CurrentDifficultyMultiplier starts at 0 and goes up by 0.01 per second
        // After 200 seconds we'd like the drain rate to be as it stands now
        // At the beginning there should be maybe 1/10th the drain rate
        // So take the max of multiplier/2 and 1/10th?
        var difficultyMod = Mathf.Max(2/10, CurrentDifficultyMultiplier) == 2/10 ? 1/10 : CurrentDifficultyMultiplier;
        m_health -= m_drainRate * tt * difficultyMod;

        //m_healthbarMask.rect.height = m_healthbarMaskHeight * m_health / 100;

        m_healthbarMask.sizeDelta = new Vector2(m_healthbarMask.rect.width, m_healthbarMaskHeight * m_health / 100);

        Debug($"HP: {GetHP()}");
    }

    public void AddHP(float x)
    {
        m_health += x;
        if (m_health > MAX_HEALTH)
            m_health = MAX_HEALTH;
    }

    public void RemoveHP(float x)
    {
        m_health -= x;
    }

    public float GetHP()
    {
        return m_health;
    }

    public void Pause() {
        m_paused = true;
    }

    public void Unpause() {
        m_paused = false;
    }

    public void SetHealthState(HealthState healthState)
    {
        switch (healthState)
        {
            case HealthState.Fine:
                m_drainRate = FINE_DRAIN_RATE;
                break;
            case HealthState.NotFine:
                m_drainRate = NOT_FINE_DRAIN_RATE;
                break;
            case HealthState.Empty:
                m_drainRate = EMPTY_DRAIN_RATE;
                break;
        }
    }

    private void Debug(string text)
    {
        if (DebugText == null)
        {
            return;
        }

        DebugText.text = text;
    }
}
