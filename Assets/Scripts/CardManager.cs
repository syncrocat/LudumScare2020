using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum CardType
{
    Ogmo,
    Stroll,
    Doge,
    Reki
}

public class CardManager : MiniGameManager
{

    public List<GameObject> cards;

    private int numCards;

    private bool startTurned = true;

    private CardScript previousUp = null;

    private float timer = 0;

    protected override string GameName()
    {
        return "MATCH THE CARDS";
    }

    public static int timesSeen = 0;

    public override void StartGame(int side, float difficulty, GameObject gameArea)
    {
        base.StartGame(side, difficulty, gameArea);

        // todo scale this with difficulty
        numCards = 8;

        if (timesSeen < 3)
        {
            numCards = 4;
        }
        else if (timesSeen < 7)
        {
            numCards = 6;
        }
        else
        {
            numCards = 8;
        }

        timesSeen += 1;

        for (int i = numCards; i < cards.Count; i++)
        {
            cards[i].SetActive(false);
        }

        InitializeCards();

    }

    public void InitializeCards ()
    {
        var numMatches = numCards / 2;

        var remainingCards = new List<GameObject>();
        for (int i = 0; i < numCards; i ++)
        {
            remainingCards.Add(cards[i]);
        }

        for (int i = 0; i < numMatches; i++)
        {
            int selection1 = Random.Range(0, remainingCards.Count - 1);
            remainingCards[selection1].GetComponent<CardScript>().SetCard(i);
            remainingCards.Remove(remainingCards[selection1]);

            int selection2 = remainingCards.Count == 1 ? 0 : Random.Range(0, remainingCards.Count - 1);
            remainingCards[selection2].GetComponent<CardScript>().SetCard(i);
            remainingCards.Remove(remainingCards[selection2]);
        }


        // Flip up all cards to start
        for (int i = 0; i < numCards; i++)
        {
            cards[i].GetComponent<CardScript>().FlipCard();
        }


    }

    public GameObject spinner;
    private bool startTheClock  = false;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (m_paused)
        {
            return;
        }

        if (startTheClock)
        {
            timer += Time.fixedDeltaTime;
            if (timer > 0.6f)
            {
                DoneGame?.Invoke(m_side, 50);
                startTheClock = false;
            }
        }
        
    }

    protected void Update()
    {
        if (m_paused)
        {
            return;
        }

        // Check for touches on any moles
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                    
                RaycastHit2D hit = Physics2D.Raycast(touch.position, Vector2.zero);
                if (!(hit && hit.collider != null && hit.collider.gameObject != null))
                {
                    continue;
                }

                var moleHit = hit.collider.gameObject.GetComponent<CardScript>();
                if (moleHit == null)
                {
                    continue;
                }

                // Turn all the cards  over to start
                if (startTurned)
                {
                    startTurned = false;
                    for (int i = 0; i < numCards; i++)
                    {
                        cards[i].GetComponent<CardScript>().HideCard();
                    }

                    continue;
                }

                // After this just  joiny doggy I guess
                if (moleHit.killing == false && moleHit != previousUp)
                {
                    

                    // Hot face them all down if some were in the hiding animation
                    for (int i = 0; i < numCards; i++)
                    {
                        var thing = cards[i].GetComponent<CardScript>();
                        if (thing.hiding)
                        {
                            thing.HideCard();
                            thing.hiding = false;
                            thing.internalTimer = 0;
                        }
                    }

                    moleHit.FlipCard();

                    // If theres one thats already up to check against
                    if (previousUp != null)
                    {
                        // If they match
                        if (previousUp.cardType == moleHit.cardType)
                        {
                            FindObjectOfType<SoundManager>().Play("Good");
                            cards.Remove(previousUp.gameObject);
                            cards.Remove(moleHit.gameObject);
                            numCards -= 2;

                            previousUp.Kill();
                            moleHit.Kill();
                            previousUp = null;

                            if (numCards == 0)
                            {
                                startTheClock = true;
                            }
                        } else
                        {
                            FindObjectOfType<SoundManager>().Play("CardMatchFail");
                            previousUp.DelayThenHide();
                            moleHit.DelayThenHide();
                            previousUp = null;
                        }
                    } else
                    {
                        previousUp = moleHit;
                    }
                }
                
            }
        }
    }

    public override void Pause()
    {
        base.Pause();
        for (int i = 0; i < numCards; i++)
        {
            cards[i].GetComponent<CardScript>().Pause();
        }
    }

    public override void Unpause()
    {
        base.Unpause();
        for (int i = 0; i < numCards; i++)
        {
            cards[i].GetComponent<CardScript>().Unpause();
        }
    }
}
