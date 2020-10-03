using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoleGameManager : MiniGameManager
{
    [SerializeField] private List<GameObject> m_moleObjects;

    private IList<Mole> m_moles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void StartGame(int side, float difficulty)
    {
        base.StartGame(side, difficulty);

        m_moles = m_moleObjects.Select(moleObject => moleObject.GetComponent<Mole>()).ToList();
        foreach (var mole in m_moles)
        {
            mole.SetSpritePopped();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Input.GetMouseButtonDown(0)) {
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPos2D = new Vector2(touchPos.x, touchPos.y);

            RaycastHit2D hit = Physics2D.Raycast(touchPos2D, Vector2.zero);
    
            var moleHit = hit.collider?.gameObject?.GetComponent<Mole>();
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
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                Vector2 touchPos2D = new Vector2(touchPos.x, touchPos.y);

                RaycastHit2D hit = Physics2D.Raycast(touchPos2D, Vector2.zero);
        
                var moleHit = hit.collider?.gameObject?.GetComponent<Mole>();
                if (moleHit == null) {
                    continue;
                }

                TapMole(moleHit);
            }
        }
    }

    private void TapMole(Mole tappedMole)
    {
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
