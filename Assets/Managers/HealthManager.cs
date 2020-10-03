﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    // Constants 
    readonly float MAX_HEALTH = 100;
    readonly float STANDARD_DRAIN_RATE = 5;  //in units per second
    readonly float FEVER_DRAIN_RATE = 10;  //in units per second

    // Variables
    [SerializeField] private float m_health; //Only serialized for editor viewing purposes
    private float m_drainRate;

    private bool m_paused;

    // Start is called before the first frame update
    void Start()
    {
        m_paused = false;
        m_health = MAX_HEALTH;
        m_drainRate = STANDARD_DRAIN_RATE;
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

    private void Die()
    {
        // todo Death goes here
    }
}