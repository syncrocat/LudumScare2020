using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GameObject current_basketball;
    private Vector2 basketball_center;
    private float startTime;

    private Vector2 swipe_start;
    private Vector2 swipe_end;
    private int fingerId = -1;
    private float swipeDuration;
    private bool ballReadyToShoot;

    // Start is called before the first frame update
    void Start()
    {
        basketball_center = (Vector2)basketball_position.transform.position;
        SpawnNewBall();
        //SUCCESS_SWIPE_HEIGHT = basketball_net.transform.position.y + SUCCESS_SWIPE_HEIGHT_MODIFIER;
    }

    void SpawnNewBall(bool ScoredGoal = false)
    {
        if (ScoredGoal)
        {
            DoneGame?.Invoke(m_side, 50);
            Debug.Log("hello we won");
        }

        current_basketball = Instantiate(basketball_prefab, basketball_center, Quaternion.identity);
        current_basketball.transform.parent = gameObject.transform;
        current_basketball.GetComponent<BallScript>().BallDead += SpawnNewBall;
        ballReadyToShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_paused)
        {
            return;
        }

        var tapCount = Input.touchCount;
        for (var i = 0; i < tapCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            var dist_from_center = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(basketball_center.x - touch.position.x), 2) + Mathf.Pow(Mathf.Abs(basketball_center.y - touch.position.y), 2));

            // Start swipe
            if (dist_from_center > 0 && dist_from_center < SWIPE_RADIUS)
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

       // Debug.Log("Successful swipe");
        return SUCCESS_SWIPE_HEIGHT;
    }
}
