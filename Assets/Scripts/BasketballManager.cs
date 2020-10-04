using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasketballManager : MiniGameManager
{
    // Constants
    private readonly float SWIPE_RADIUS = 200;
    private readonly float MINIMUM_SWIPE_MAGNITUDE = 50;
    private readonly float MAXIMUM_SWIPE_DURATION = 0.5f;
    private readonly float FAILED_SWIPE_HEIGHT = 500;
    //private readonly float SUCCESS_SWIPE_HEIGHT_MODIFIER = 50; // Successful swipe will be the basketball net height plus this value
    private float SUCCESS_SWIPE_HEIGHT = 1000; // Because I didn't want to do math, this variable must be carefully balanced with GRAVITY_PER_SECOND in BallScript

    [SerializeField] private GameObject basketball_position;
    [SerializeField] private GameObject basketball_prefab;
    [SerializeField] private GameObject basketball_net;
    public GameObject ballholder2;
    private GameObject current_basketball;
    private Vector2 basketball_center;
    private float startTime;

    private Vector2 swipe_start;
    private Vector2 swipe_end;
    private int fingerId = -1;
    private float swipeDuration;
    private bool ballReadyToShoot;

    int movespeed = 0;
    public GameObject m_movingComponent;

    // Start is called before the first frame update
    void Start()
    {
        basketball_center = (Vector2)basketball_position.transform.position;
        SpawnNewBall();
        //SUCCESS_SWIPE_HEIGHT = basketball_net.transform.position.y + SUCCESS_SWIPE_HEIGHT_MODIFIER;
    }

    protected override string GameName()
    {
        return "SHOOT A BASKET";
    }

    public static int timesSeen = 0;
    public override void StartGame(int side, float difficulty, GameObject playArea)
    {
        base.StartGame(side, difficulty, playArea);
        
        if (timesSeen < 5)
        {
            movespeed = 0;
        } else if (timesSeen < 10)
        {
            movespeed = 100;
        } else if (timesSeen < 15)
        {
            movespeed = 250;
        } else
        {
            movespeed = 500;
        }

        timesSeen += 1;

    }

        void SpawnNewBall(bool ScoredGoal = false)
    {
        if (ScoredGoal)
        {
            DoneGame?.Invoke(m_side, 50);
        }

        current_basketball = Instantiate(basketball_prefab, basketball_center, Quaternion.identity);
        current_basketball.transform.parent = gameObject.transform;
        current_basketball.GetComponent<BallScript>().BallDead += SpawnNewBall;
        ballReadyToShoot = true;
    }

    int direction = 1;

    private void FixedUpdate()
    {
        if (m_paused)
            return;


        Vector3 newPosition = m_movingComponent.transform.position + new Vector3(movespeed * direction * Time.fixedDeltaTime, 0, 0);

        GameObject canv = GameObject.FindGameObjectsWithTag("MainCanvas")[0];
        var scaleY = canv.GetComponent<RectTransform>().localScale.y;

        if  ((newPosition.x > m_gameArea.transform.position.x + 300 * scaleY / 2.4f && direction == 1) || (newPosition.x < m_gameArea.transform.position.x - 300 * scaleY / 2.4f && direction == -1)) {
            direction *= -1;
           // newPosition += new Vector3(Mathf.Abs(m_gameArea.transform.position.x - newPosition.x) * direction * 2, 0, 0);
        }

        m_movingComponent.transform.position = newPosition;

    }

    // Update is called once per frame
    void Update()
    {
        if (m_paused)
        {
            return;
        }

        // Go behind the net
        //if (current_basketball.transform.position.y > basketball_net.transform.position.y + current_basketball.transform.GetComponent<CircleCollider2D>().radius)
        if (current_basketball.GetComponent<Rigidbody2D>().velocity.y < 0)
        {
            current_basketball.transform.parent = ballholder2.transform;
        }

        GameObject canv = GameObject.FindGameObjectsWithTag("MainCanvas")[0];
        var scaleY = canv.GetComponent<RectTransform>().localScale.y;

        var tapCount = Input.touchCount;
        for (var i = 0; i < tapCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            var dist_from_center = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(basketball_center.x - touch.position.x), 2) + Mathf.Pow(Mathf.Abs(basketball_center.y - touch.position.y), 2));

            // Start swipe
            if (dist_from_center > 0 && dist_from_center < SWIPE_RADIUS * scaleY / 2.4f)
            {
                
                if (ballReadyToShoot && (touch.phase == TouchPhase.Began || fingerId == -1))
                {
                    swipe_start = touch.position;
                    fingerId = touch.fingerId;
                    startTime = Time.time;
                }
            }

            // End swipe
            if (fingerId == touch.fingerId && touch.phase == TouchPhase.Ended)
            {
                swipe_end = touch.position;
                swipeDuration = Time.time - startTime;
                TryShoot();
            }
            
            // Reset fingerId if youre not still doing the stroke (can they be cancelled prematurely? who knows, lets be safe)
            if (fingerId == touch.fingerId && touch.phase != TouchPhase.Moved && touch.phase != TouchPhase.Stationary && touch.phase != TouchPhase.Began)
            {
                fingerId = -1; 
            }
        }
    }

    void TryShoot()
    {
        var shootHeight = GetShootHeight();
        var shootVector = swipe_end - swipe_start;

        current_basketball.GetComponent<BallScript>().ShootMe(shootHeight, shootVector);
        ballReadyToShoot = false;
    }

    private float GetShootHeight()
    {
        if (swipeDuration > MAXIMUM_SWIPE_DURATION)
            return FAILED_SWIPE_HEIGHT;

        var swipeMagnitude = Vector2.Distance(swipe_start, swipe_end);
        if (swipeMagnitude < MINIMUM_SWIPE_MAGNITUDE)
            return FAILED_SWIPE_HEIGHT;

        return SUCCESS_SWIPE_HEIGHT;
    }

    public override void Pause()
    {
        base.Pause();
        current_basketball.GetComponent<BallScript>().Pause();
    }

    public override void Unpause()
    {
        base.Unpause();
        current_basketball.GetComponent<BallScript>().Unpause();
    }
}
