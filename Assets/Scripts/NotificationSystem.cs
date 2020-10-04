using System;
using System.Collections.Generic;
using UnityEngine;

public enum AlertLevel
{
    Low,
    High,
}

public class NotificationSystem : MonoBehaviour
{
    public GameObject LeftLow;
    public GameObject LeftHigh;
    public GameObject RightLow;
    public GameObject RightHigh;

    private Dictionary<Tuple<int, AlertLevel>, Notification> ObjectMapping;
    private Dictionary<Tuple<int, AlertLevel>, float> TimerMapping;

    private float m_leftLowTimer = 0;
    private float m_rightLowTimer = 0;
    private float m_leftHighTimer = 0;
    private float m_rightHighTimer = 0;
    // Start is called before the first frame update
    void Awake()
    {
        ObjectMapping = new Dictionary<Tuple<int, AlertLevel>, Notification>()
        {
            { new Tuple<int, AlertLevel>(0, AlertLevel.Low), LeftLow.GetComponent<Notification>() },
            { new Tuple<int, AlertLevel>(0, AlertLevel.High), LeftHigh.GetComponent<Notification>() },
            { new Tuple<int, AlertLevel>(1, AlertLevel.Low), RightLow.GetComponent<Notification>() },
            { new Tuple<int, AlertLevel>(1, AlertLevel.High), RightHigh.GetComponent<Notification>() },
        };

        TimerMapping = new Dictionary<Tuple<int, AlertLevel>, float>()
        {
            { new Tuple<int, AlertLevel>(0, AlertLevel.Low), m_leftLowTimer },
            { new Tuple<int, AlertLevel>(0, AlertLevel.High), m_leftHighTimer },
            { new Tuple<int, AlertLevel>(1, AlertLevel.Low), m_rightLowTimer },
            { new Tuple<int, AlertLevel>(1, AlertLevel.High), m_rightHighTimer },
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Alert(int side, AlertLevel alertLevel)
    {
        // 3 flashes of 0.5s with 0.5s between them
        var x = ObjectMapping[new Tuple<int, AlertLevel>(side, alertLevel)];
        if (x != null)
            x.Begin();
    }

    public void IndefiniteAlert(int side, AlertLevel alertLevel)
    {
        ObjectMapping[new Tuple<int, AlertLevel>(side, alertLevel)].Indefinite();
    }

    public void CancelAlert(int side, AlertLevel alertLevel)
    {
        ObjectMapping[new Tuple<int, AlertLevel>(side, alertLevel)].End();
    }

    public void Pause()
    {
        foreach(var obj in ObjectMapping)
        {
            obj.Value.Pause();
        }
    }

    public void Unpause()
    {
        foreach (var obj in ObjectMapping)
        {
            obj.Value.Unpause();
        }
    }
}
