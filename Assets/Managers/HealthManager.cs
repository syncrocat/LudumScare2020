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
    private float m_drainRate;

    private bool m_paused;

    // Start is called before the first frame update
    void Start()
    {
        m_paused = false;
        m_health = MAX_HEALTH;
        m_drainRate = FINE_DRAIN_RATE;
    }

    void FixedUpdate()
    {
        if (m_paused)
        {
            return;
        }

        float tt = Time.deltaTime;

        m_health -= m_drainRate * tt;
        if (m_health < 0)
            Die();

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
        if (m_health < 0)
            Die();
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

    private void Die()
    {
        // todo Death goes here
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
