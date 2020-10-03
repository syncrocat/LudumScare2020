using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mole : MonoBehaviour
{
    public bool Popped;
    public Sprite PoppedSprite;
    public Sprite BuriedSprite;
    private Sprite m_activeSprite;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpritePopped()
    {
        m_activeSprite = PoppedSprite;
        Popped = true;
    }

    public void SetSpriteBuried()
    {
        m_activeSprite = BuriedSprite;
        Popped = false;
    }

    public void Tapped()
    {
        if (Popped)
        {
            SetSpriteBuried();
        }
    }
}
