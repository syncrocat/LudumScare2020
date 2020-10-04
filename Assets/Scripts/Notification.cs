using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    private float m_timer;

    private Image m_image;

    private bool m_isGoing;

    private bool m_paused = false;
    // Start is called before the first frame update
    void Awake()
    {
        m_image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_paused)
        {
            return;
        }

        if (m_timer > 0f)
        {
            m_timer -= 1 * Time.deltaTime;
        }

        if (m_timer <= 0f)
        {
            // make sure im hidden
            gameObject.SetActive(false);
        } else if (m_timer % 0.5 > 0.25f)
        {
            m_image.CrossFadeAlpha(1, 0.25f, false);
        } else
        {
            m_image.CrossFadeAlpha(0, 0.25f, false);
        }
    }

    // This is designed for warning about spinners and stuff
    public void Begin()
    {
        m_timer += 2.5f;
        m_image.canvasRenderer.SetAlpha(0f);
        gameObject.SetActive(true);
    }

    // Best for SPIN FASTER warnings
    public void Indefinite()
    {
        if (!m_isGoing)
        {
            m_isGoing = true;
            m_timer = 1000f;
            m_image.canvasRenderer.SetAlpha(0f);
            gameObject.SetActive(true);
        }
    }

    public void End()
    {
        m_timer = 0f;
        m_isGoing = false;
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
