using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HealthState
{
    Fine,
    NotFine,
    Empty,
}

public class VelocityBarManager : MonoBehaviour
{
    private const float MAX_VELOCITY = 100f;
    private const float BAR_VELOCITY = 50f;
    [SerializeField] private float m_currentVelocity; // Serialized for debug purposes

    public GameObject water;

    private bool m_paused;

    private SpinState m_spinState;
    private readonly Dictionary<SpinState, float> m_velocityMapping = new Dictionary<SpinState, float>()
    {
        { SpinState.ReallyFine, 50 },
        { SpinState.Fine, 25 },
        { SpinState.Bad, -25 },
        { SpinState.ReallyBad, -50 },
    };

    public Text DebugText;

    float scaleY;

    // Start is called before the first frame update
    void Start()
    {
        defaultWaterPosition = water.transform.position;
        GameObject canv = GameObject.FindGameObjectsWithTag("MainCanvas")[0];
        scaleY = canv.GetComponent<RectTransform>().localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVelocity(float velocity)
    {
        m_currentVelocity = velocity;
        if (m_currentVelocity > MAX_VELOCITY)
        {
            m_currentVelocity = MAX_VELOCITY;
        } else if (m_currentVelocity < 0)
        {
            m_currentVelocity = 0;
        }
    }

    // Split add and sub functions to allow for different sounds/UI stuff
    public void AddVelocity(float extraVelocity)
    {
        if (extraVelocity < 0)
        {
            SubVelocity(Mathf.Abs(extraVelocity));
            return;
        }

        m_currentVelocity += extraVelocity;
        if (m_currentVelocity > MAX_VELOCITY)
        {
            m_currentVelocity = MAX_VELOCITY;
        }
    }

    public void SubVelocity(float lostVelocity)
    {
        m_currentVelocity -= lostVelocity;
        if (m_currentVelocity < 0)
        {
            m_currentVelocity = 0;
        }
    }

    public void SetSpinState(SpinState spinState)
    {
        m_spinState = spinState;
    }

    Vector2 defaultWaterPosition;

    public void FixedUpdate()
    {
        if (m_paused)
        {
            return;
        }

        AddVelocity(m_velocityMapping[m_spinState] * Time.fixedDeltaTime);
        Debug($"Velocity: {m_currentVelocity}%");

        var offBar = 300 * scaleY / 2.4f * ((100 - GetCurrentVelocity()) / 100);

        water.transform.position = new Vector3(water.transform.position.x,  defaultWaterPosition.y - offBar, 0);
    }

    public HealthState GetHealthState()
    {
        if (m_currentVelocity > BAR_VELOCITY)
        {
            return HealthState.Fine;
        }

        if (m_currentVelocity > 0)
        {
            return HealthState.NotFine;
        }

        return HealthState.Empty;
    }

    public float GetCurrentVelocity()
    {
        return m_currentVelocity;
    }

    private void Debug(string text)
    {
        if (DebugText == null)
        {
            return;
        }

        DebugText.text = text;
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
