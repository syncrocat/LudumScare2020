using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mole : MonoBehaviour
{
    public bool Popped;
    public Sprite PoppedSprite;
    public Sprite BuriedSprite;
    public int Index;

    private Image m_image;
    
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
        if (m_image == null) {
            m_image = GetComponent<Image>();
        }

        m_image.sprite = PoppedSprite;
        Popped = true;
    }

    public void SetSpriteBuried()
    {
        if (m_image == null) {
            m_image = GetComponent<Image>();
        }
        
        m_image.sprite = BuriedSprite;
        Popped = false;
    }

    public bool Tapped(int desiredMole)
    {
        if (Popped) // && Index == desiredMole
        {
            SetSpriteBuried();
            return true;
        }

        return false;
    }
}
