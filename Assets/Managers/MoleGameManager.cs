using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoleGameManager : MiniGameManager
{
    [SerializeField] private List<GameObject> m_moleObjects;

    private List<Mole> m_moles;

    public override void StartGame(int side, float difficulty)
    {
        base.StartGame(side, difficulty);
        m_moles = new List<Mole>();
        foreach(var moleObject in m_moleObjects) {
            m_moles.Add(moleObject.GetComponent<Mole>());
        }
        
        foreach (var mole in m_moles)
        {
            mole.SetSpritePopped();
        }
    }

    protected void Update()
    {
        if (m_paused) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Clicked!");
            RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero);
            var moleHit = hit.collider?.gameObject?.GetComponent<Mole>();
            if (moleHit == null) {
                return;
            }

            Debug.Log("Clicked... A MOLE!");
            TapMole(moleHit);
        }

        // Check for touches on any moles
        foreach(Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("Touched!");
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                Vector2 touchPos2D = new Vector2(touchPos.x, touchPos.y);

                RaycastHit2D hit = Physics2D.Raycast(touchPos2D, Vector2.zero);
        
                var moleHit = hit.collider?.gameObject?.GetComponent<Mole>();
                if (moleHit == null) {
                    continue;
                }

                Debug.Log("Touched... a MOLE!");
                TapMole(moleHit);
            }
        }
    }

    private void TapMole(Mole tappedMole)
    {
        foreach(var mole in m_moles) {
            Debug.Log(mole.Popped);
        }

        tappedMole.Tapped();

        // Check to see if all moles are done
        foreach(Mole mole in m_moles)
        {
            if (mole.Popped)
            {
                return;
            }
        }

        DoneGame?.Invoke(m_side, 50);
    }
}
