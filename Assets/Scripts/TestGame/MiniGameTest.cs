using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Example implementation of a MiniGameManager
public class MiniGameTest : MiniGameManager
{
    public override void DestroySelf()
    {
        // Any additional cleanup behaviour first
        // Call basic shared destroy self 
        base.DestroySelf();
    }
    public override void StartGame(int side, float difficulty, GameObject playArea)
    {
        base.StartGame(side, difficulty, playArea);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (m_paused)
        {
            return;
        }

        if (m_timer > 10)
        {
            DoneGame.Invoke(m_side, 50);
        }
    }
}
