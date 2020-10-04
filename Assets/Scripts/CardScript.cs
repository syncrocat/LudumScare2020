using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    public List<Sprite> CardSprites;
    public Sprite BackCardSprite;

    public int cardType;

    public bool killing = false;
    public bool hiding = false;

    public float internalTimer = 0;

    private bool m_paused;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void SetCard(int x)
    {
        cardType = x;
    }

    public void FlipCard()
    {
        if (killing)
            return;
        this.GetComponent<Image>().sprite = CardSprites[cardType];
    }

    public void HideCard()
    {
        if (killing)
            return;
        this.GetComponent<Image>().sprite = BackCardSprite;
    }

    public void Kill()
    {
        killing = true;
        internalTimer = -0.2f;
    }

    public void DelayThenHide()
    {
        hiding = true;
    }

    public void FixedUpdate()
    {
        if (m_paused)
        {
            return;
        }

        if (killing)
        {
            internalTimer += Time.fixedDeltaTime;
            if (internalTimer > 0 && internalTimer < 0.2)
            {
                this.GetComponent<Image>().enabled = false;
            } else if (internalTimer < 0.4)
            {
                this.GetComponent<Image>().enabled = true;
            } else
            {
                Destroy(this.gameObject);
            }
        }

        if (hiding)
        {
            internalTimer += Time.fixedDeltaTime;
            if (internalTimer >= 1)
            {
                this.HideCard();
                hiding = false;
                internalTimer = 0;
            }
        }

    }

    public void Pause()
    {
        m_paused = true;
    }

    public void Unpause()
    {
        m_paused = false;
    }
}
