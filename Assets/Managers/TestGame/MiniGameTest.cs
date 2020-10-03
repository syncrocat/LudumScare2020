using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Example implementation of a MiniGameManager
public class MiniGameTest : MiniGameManager
{
    float m_timer = 0;

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

    private void FixedUpdate()
    {
        m_timer += 1 * Time.deltaTime;

        if (m_timer > 10)
        {
            DoneGame.Invoke(50);
        }
    }

    
}
