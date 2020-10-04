using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BallScript : MonoBehaviour
{
    //private readonly float SHOT_TIME = 2;
    private readonly float GRAVITY_PER_SECOND = 1500; // Because I didn't want to do math, this variable must be carefully balanced with SUCCESS_SWIPE_HEIGHT in BasketBallManagers

    public float vSpeed = 0;
    public float hSpeed = 0;
    bool gravity = false;
    Rigidbody2D rigidBody;
    float timer = -1;

    bool scoredGoal = false;

    private float scaleY;

    public UnityAction<bool> BallDead;

    private bool m_paused;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();

        GameObject canv = GameObject.FindGameObjectsWithTag("MainCanvas")[0];
        scaleY = canv.GetComponent<RectTransform>().localScale.y;

        this.transform.localScale *=  scaleY / 2.4f;
    }

    public void ShootMe(float shootHeight, Vector2 shootVector)
    {
        FindObjectOfType<SoundManager>().Play("Throw");
        var shootHComponent = shootVector.y;
        var factor = shootHeight / shootHComponent;
        Vector2 newVector = shootVector * factor;

        // vSpeed = newVector.y;
        //hSpeed = newVector.x;

        //rigidBody.AddForce(newVector * 70);
        newVector = new Vector2(newVector.x * 0.5f, newVector.y);

        rigidBody.velocity += newVector * (1.8f / 2.4f * scaleY);
        rigidBody.gravityScale = 300 / 2.4f * scaleY;
        timer = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_paused)
        {
            return;
        }

        if (timer >= 0)
        {
            timer += Time.fixedDeltaTime;
            if (rigidBody.velocity.y > 0)
            {
                var sizeFactor = Time.fixedDeltaTime * 0.3f;
                this.gameObject.transform.localScale -= new Vector3(sizeFactor, sizeFactor, 0);
            }
        }
        if (rigidBody.velocity.y < 0)
        {
            this.GetComponent<CircleCollider2D>().enabled = true;
        }

        if (rigidBody.position.y < 0 || timer > 4)
        {
            BallDead.Invoke(scoredGoal);
            Destroy(this.gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BasketballNet"))
        {
            scoredGoal = true;
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
