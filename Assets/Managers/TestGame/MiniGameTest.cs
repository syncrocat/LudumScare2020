using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Example implementation of a MiniGameManager
public class MiniGameTest : MiniGameManager
{
    float m_timer = 0;
    private bool m_paused;

    public override void DestroySelf()
    {
        // Any additional cleanup behaviour first
        Debug.Log("Finishing minigame!");

        // Call basic shared destroy self 
        base.DestroySelf();
    }
    public override void StartGame(float difficulty)
    {
        Debug.Log("Starting minigame!");

        base.StartGame(difficulty);
    }

    public override void Pause()
    {
        m_paused = true;
    }

    public override void Unpause()
    {
        m_paused = false;
    }

    private void FixedUpdate()
    {
        if (m_paused)
        {
            return;
        }
        
        m_timer += 1 * Time.deltaTime;

        if (m_timer > 10)
        {
            DoneGame.Invoke(50);
        }
    }
}
