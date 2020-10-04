using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShapesManager : MiniGameManager
{
    private readonly float SQUARE_STANDARD_DIST = 300;
    private readonly float RANDOM_VARIANCE = 50;

    public static int timesSeen = 0; 

    [SerializeField] private GameObject dot_prefab;
    [SerializeField] private GameObject line_prefab;
    [SerializeField] private GameObject trail_holder;
    [SerializeField] private GameObject trail_cursor;

    private List<GameObject> le_dottlers = new List<GameObject>();

    List<List<Vector2>> paths = new List<List<Vector2>>();

    Vector2 PolarToCartesian_Degrees(float angle, float magnitude)
    {
        float radians = angle * Mathf.PI / 180.0f;
        var x = Mathf.Cos(radians) * magnitude;
        var y = Mathf.Sin(radians) * magnitude;

        return new Vector2(x, y);
    }

    public Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    float scaleY;

    protected override string GameName()
    {
        return "CONNECT THE DOTS";
    }

    public override void StartGame(int side, float difficulty, GameObject gameArea)
    {
        base.StartGame(side, difficulty, gameArea);

        this.transform.position += new Vector3(0,-20, 0);

        float baseIdiotNumber = Mathf.Floor(Mathf.Min(timesSeen / 2 + 3, 8));
        //baseIdiotNumber = 20;

        timesSeen += 1;

        float degreesPerDoge = 365 / baseIdiotNumber;

        float tickingRotate = - degreesPerDoge / 2;

        GameObject canv = GameObject.FindGameObjectsWithTag("MainCanvas")[0];
         scaleY = canv.GetComponent<RectTransform>().localScale.y;

        var epicMagnitude = SQUARE_STANDARD_DIST * scaleY / 2.4f;

        Vector2 initialVector = new Vector2(epicMagnitude, 0);

        

        for (int i = 0; i < baseIdiotNumber; i++)
        {
            var nextVector = Rotate(initialVector, degreesPerDoge * i);

            var offSetVector = new Vector2(Random.Range(-RANDOM_VARIANCE, RANDOM_VARIANCE), Random.Range(-RANDOM_VARIANCE, RANDOM_VARIANCE));

            if (i == 0) offSetVector = new Vector2(0, 0);

            le_dottlers.Add(Instantiate(dot_prefab, (Vector2)gameArea.transform.position + offSetVector + nextVector, Quaternion.identity, this.gameObject.transform));
        }

        RandomizeDotOrder();

        le_dottlers[0].GetComponent<DotScript>().GlowMeUpScotty();
        //le_dottlers[1].GetComponent<DotScript>().GlowMeUpScotty();

        AddArrowMess();
    }

    public void RandomizeDotOrder()
    {
        var size = le_dottlers.Count;
        var hello = new List<GameObject>();
        hello.Add(le_dottlers[0]);
        for (int i = 0; i < size - 1; i++)
        {
            var randydoggy = Random.Range(1, le_dottlers.Count);
            hello.Add(le_dottlers[randydoggy]);
            le_dottlers.Remove(le_dottlers[randydoggy]);
        }
        le_dottlers = hello;
    }

    List<GameObject> joinMe;

    public void AddArrowMess()
    {
        joinMe = new List<GameObject>();
        for (int i = 0; i < le_dottlers.Count - 1; i++)
        {
            joinMe.Add(null);
            joinMe[i] = Instantiate(line_prefab, (Vector2)m_gameArea.transform.position, Quaternion.identity, trail_holder.transform);
            StretchGameObjOverPts(joinMe[i], le_dottlers[i + 1].transform.position, le_dottlers[i].transform.position);
        }
    }

    private float GetAngleRadians(Vector2 coordinate)
    {
        var delta_x = coordinate.x - 0;
        var delta_y = coordinate.y - 0;
        var theta_radians = Mathf.Atan2(delta_y, delta_x);

        return theta_radians;
    }

    Vector3 cylDefaultOrientation = new Vector3(0, 1, 0);

    public void StretchGameObjOverPts(GameObject cylinder, Vector2 endV, Vector2 startV)
    {
        cylinder.transform.position = startV;

        var targetDir = endV - startV;


        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        cylinder.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        var magnitude = targetDir.magnitude;

        cylinder.transform.localScale = new Vector2( magnitude / scaleY, cylinder.transform.localScale.y);

        cylinder.transform.position += (Vector3) (endV - startV) / 2;

        /*var angleVector = endV - startV;
        var angle = GetAngleRadians(angleVector);

        cylinder.transform.Rotate(new Vector3(0, 0, angle));*/

    }


    int currentIndex = 0;
    int OUTER_SPIN_RADIUS = 100;
    float fingerId = -1;

    protected void Update()
    {
        if (m_paused)
        {
            return;
        }

        if (currentIndex >= le_dottlers.Count)
            return;

        var tapCount = Input.touchCount;
        var fingerFlag = false;

        for (var i = 0; i < tapCount; i++)
        {
            
            Touch touch = Input.GetTouch(i);
            //position
            var touch_position = touch.position;

            Vector2 spinner_center = le_dottlers[currentIndex].transform.position;

            var dist_from_center = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(spinner_center.x - touch.position.x), 2) + Mathf.Pow(Mathf.Abs(spinner_center.y - touch.position.y), 2));

            if (dist_from_center < OUTER_SPIN_RADIUS * scaleY / 2.4f)
            {
                if (fingerId == -1)
                {
                    fingerId = touch.fingerId;
                }

                if (fingerId == touch.fingerId)
                {
                    AdvanceIndex(currentIndex);
                }
            }

            if (fingerId == touch.fingerId)
                fingerFlag = true;

                // Cnacelling the path early
            if (fingerId == touch.fingerId && touch.phase != TouchPhase.Moved && touch.phase != TouchPhase.Stationary && touch.phase != TouchPhase.Began)
            {
                CancelSwipePath();
            }
        }

        //If we lost our other swipe randomly for some reason (happens ???)
       /* if (fingerFlag == false)
            CancelSwipePath(); EDIT: doesnt happen */

    }

    private void CancelSwipePath()
    {
        fingerId = -1;
        for (int ii = 0; ii < le_dottlers.Count; ii++)
        {
            le_dottlers[ii].GetComponent<DotScript>().GlowMeDownScotty();
            if (ii < le_dottlers.Count - 1)
                joinMe[ii].GetComponent<DotScript>().GlowMeDownScotty();
        }
        currentIndex = 0;
        le_dottlers[0].GetComponent<DotScript>().GlowMeUpScotty();
    }

    private void AdvanceIndex(int index)
    {
        if (index != currentIndex)
            return;
        currentIndex += 1;

        if (currentIndex - 2 >= 0)
            joinMe[currentIndex - 2].GetComponent<DotScript>().GlowMeUpScotty();
        le_dottlers[currentIndex - 1].GetComponent<DotScript>().GlowMeUpISaid();

        FindObjectOfType<SoundManager>().Play("DotConnect");

        if (currentIndex >= le_dottlers.Count)
        {
            DoneGame?.Invoke(m_side, 50);
        } else
        {
            le_dottlers[currentIndex].GetComponent<DotScript>().GlowMeUpScotty();
        }
    }



}
