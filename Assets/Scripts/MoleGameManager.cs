using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoleGameManager : MiniGameManager
{
    private const int MIN_PREVIEW_MOLES = 1;
    private const int MAX_PREVIEW_MOLES = 9;
    private const int HEALTH_REWARD = 50;

    [SerializeField] private List<GameObject> m_moleObjects;

    private List<Mole> m_moles;

    private int m_currentMole;

    private int m_molesToPreview;

    private List<int> m_moleOrder;

    public static int timesSeen = 0;

    protected override string GameName()
    {
        return "WHACK THE MOLES";
    }

    public override void StartGame(int side, float difficulty, GameObject playArea)
    {
        base.StartGame(side, difficulty, playArea);
       // m_molesToPreview = Math.Max(MAX_PREVIEW_MOLES - (int)(difficulty * 10), MIN_PREVIEW_MOLES);


        m_molesToPreview = Math.Max(MIN_PREVIEW_MOLES, MAX_PREVIEW_MOLES - timesSeen);

        timesSeen += 1;

        m_moles = m_moleObjects.Select(obj =>
        {

            var mole = obj.GetComponent<Mole>();
            return mole;
        }).ToList();
        m_moles = new List<Mole>();
        foreach(var moleObject in m_moleObjects) {
            m_moles.Add(moleObject.GetComponent<Mole>());
        }

        m_moleOrder = RandomOrder(m_moles.Count);

        for (var i = 0; i < m_molesToPreview; i++)
        {
            m_moles[m_moleOrder[i]].SetSpritePopped();
            m_moles[m_moleOrder[i]].Index = i;
        }

        for (var i = m_molesToPreview; i < m_moles.Count; i++)
        {
            m_moles[m_moleOrder[i]].SetSpriteBuried();
            m_moles[m_moleOrder[i]].Index = i;
        }

    }

    protected void Update()
    {
        if (m_paused) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero);
            if (!(hit && hit.collider != null && hit.collider.gameObject != null))
            {
                return;
            }

            var moleHit = hit.collider.gameObject.GetComponent<Mole>();
            if (moleHit == null) {
                return;
            }

            TapMole(moleHit);
        }

        // Check for touches on any moles
        foreach(Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit2D hit = Physics2D.Raycast(touch.position, Vector2.zero);
                if (!(hit && hit.collider != null && hit.collider.gameObject != null))
                {
                    continue;
                }

                var moleHit = hit.collider.gameObject.GetComponent<Mole>();
                if (moleHit == null)
                {
                    continue;
                }

                TapMole(moleHit);
            }
        }
    }

    private void TapMole(Mole tappedMole)
    {
        if (tappedMole.Tapped(m_currentMole))
        {
            FindObjectOfType<SoundManager>().Play("MoleHit");
            // Show the next moles we need to
            m_currentMole += 1;
            if (m_currentMole >= m_moles.Count)
            {
                DoneGame?.Invoke(m_side, HEALTH_REWARD);
            }
            else if (m_currentMole + m_molesToPreview - 1 < m_moles.Count)
            {
                m_moles[m_moleOrder[m_currentMole + m_molesToPreview - 1]].SetSpritePopped();
            }
        }
    }

    private List<int> RandomOrder(int size)
    {
        var rnd = new System.Random();
        var choices = new List<int>();
        for (var i = 0; i < size; i++)
        {
            choices.Add(i);
        }

        var result = new List<int>();
        while (size > 0)
        {
            var choice = rnd.Next(0, size);
            result.Add(choices[choice]);
            choices.RemoveAt(choice);
            size -= 1;
        }

        return result;
    }
}
