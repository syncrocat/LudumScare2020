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
    private const float BAR_VELOCITY = 70f;
    [SerializeField] private float m_currentVelocity; // Serialized for debug purposes
    private SpinState m_spinState;
    private readonly Dictionary<SpinState, float> m_velocityMapping = new Dictionary<SpinState, float>()
    {
        { SpinState.ReallyFine, 50 },
        { SpinState.Fine, 25 },
        { SpinState.Bad, -25 },
        { SpinState.ReallyBad, -50 },
    };

    public Text DebugText;

    // Start is called before the first frame update
    void Start()
    {
        
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

    public void FixedUpdate()
    {
        AddVelocity(m_velocityMapping[m_spinState] * Time.fixedDeltaTime);
        Debug($"Velocity: {m_currentVelocity}%");
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

    private void Debug(string text)
    {
        if (DebugText == null)
        {
            return;
        }

        DebugText.text = text;
    }
}
