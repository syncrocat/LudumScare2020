using Boo.Lang;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

enum Direction
{
    Clockwise,
    CounterClockwise
}

public enum SpinState
{
    ReallyFine,
    Fine,
    Bad,
    ReallyBad,
}

[RequireComponent(typeof (RectTransform))]
public class SpinManager : MiniGameManager
{
    //Constants
    private readonly float INNER_SPIN_RADIUS = 150;
    private readonly float OUTER_SPIN_RADIUS = 450;
    private readonly Direction DEFAULT_SPIN_DIRECTION = Direction.CounterClockwise;

    // Vars
    private Direction mode;
    private Vector2 touch_position;
    private Vector2 spinner_center;
    private float turnSinceLast = 0;
    public float speed;
    private List<float> speedRollingAverage = new List<float>() { 0, 0, 0, 0, 0 };
    private int rollingAverageIndex = 0;
    public VelocityBarManager VelocityManager;

    public Text DebugText;

    public SpinState SpinState;

    [SerializeField] private GameObject spinnerImg = null;

    // Start is called before the first frame update
    void Start()
    {
        mode = DEFAULT_SPIN_DIRECTION;
        spinner_center = GetComponent<RectTransform>().position;
    }

    protected override string GameName()
    {
        return "SPIN!";
    }

    private float GetAngleRadians(Vector2 coordinate)
    {
        var delta_x = coordinate.x - spinner_center.x;
        var delta_y = coordinate.y - spinner_center.y;
        var theta_radians = Mathf.Atan2(delta_y, delta_x);

        return theta_radians;
    }

    private float DistanceBetweenRadians(float rad1, float rad2)
    {
        rad1 = rad1 * Mathf.Rad2Deg;
        rad2 = rad2 * Mathf.Rad2Deg;

        return Mathf.DeltaAngle(rad1, rad2);
    }

    // NOTE: This needs to be Update, not FixedUpdate. We need to access the deltaAngle from the previous frame
    // for touch rotations
    void Update()
    {
        if (m_paused)
        {
            return;
        }

        float turnAngle = GetTurnDegrees();
        turnSinceLast += turnAngle;   
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (m_paused)
        {
            return;
        }

        spinnerImg.transform.Rotate(new Vector3(0, 0, 4f));

        /*if ((turnSinceLast > 0 && mode == Direction.CounterClockwise) || (turnSinceLast < 0 && mode == Direction.Clockwise))
        {
            
        }*/

        speed = (turnSinceLast / Time.deltaTime);
        if (speed < 0)
        {
            speed = 0;
        }

        speedRollingAverage[rollingAverageIndex] = speed;
        rollingAverageIndex += 1;
        if (rollingAverageIndex > 4)
        {
            rollingAverageIndex = 0;
        }

        var rollingAverage = speedRollingAverage.Average();

        Debug($"Speed: {rollingAverage}");
        // Calculate state to send to velocitymanager
        if (speed > 250)
        {
            SpinState = SpinState.ReallyFine;
        } else if (speed > 150)
        {
            SpinState = SpinState.Fine;
        } else if (speed > 75)
        {
            SpinState = SpinState.Bad;
        } else
        {
            SpinState = SpinState.ReallyBad;
        }

        VelocityManager.SetSpinState(SpinState);

        turnSinceLast = 0;
    }

    public float GetTurnDegrees()
    {
        GameObject canv = GameObject.FindGameObjectsWithTag("MainCanvas")[0];
        var scaleY = canv.GetComponent<RectTransform>().localScale.y;

        var tapCount = Input.touchCount;
        for (var i = 0; i < tapCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            //position
            touch_position = touch.position;

            var dist_from_center = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(spinner_center.x - touch.position.x), 2) + Mathf.Pow(Mathf.Abs(spinner_center.y - touch.position.y), 2));

            if (dist_from_center > INNER_SPIN_RADIUS * scaleY / 2.4f && dist_from_center < OUTER_SPIN_RADIUS * scaleY / 2.4f)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended)
                {

                    Vector2 line_pos_2 = touch_position;
                    Vector2 line_pos_1 = touch_position - touch.deltaPosition;

                    /*
                     * Ideally we should have some code here 
                     * that trims the line_pos_2 to the point in which it entered the circle
                     * otherwise the spinner can theoretically be cheated
                     * 
                     * But I suspect this won't actually be a big issue, so we'll ignore it for now
                     * and see if it remotely affects gameplay
                     * 
                     * Finding out where a line intersects a circle turns out to be a fucking nightmare
                     * */

                    float angle1 = GetAngleRadians(line_pos_1);
                    float angle2 = GetAngleRadians(line_pos_2);

                    return DistanceBetweenRadians(angle1, angle2);

                }
            }
        }
        return 0;
    }

    public HealthState GetHealthState()
    {
        return VelocityManager.GetHealthState();
    }

    private void Debug(string text)
    {
        if (DebugText == null)
        {
            return;
        }

        DebugText.text = text;
    }

    public override void Pause()
    {
        base.Pause();
        VelocityManager.Pause();
    }

    public override void Unpause()
    {
        base.Unpause();
        VelocityManager.Unpause();
    }
}
