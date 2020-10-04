using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class copyanimationscript : MonoBehaviour
{

    public SpriteRenderer copy_dark;
    public Image paste;
    public SpriteRenderer copy_light;
    public VelocityBarManager VBM;

    public bool lightOn;

    // Start is called before the first frame update
    void Start()
    {
        paste.enabled = true;

        copy_light.gameObject.GetComponent<Animator>().speed = 2;
        copy_dark.gameObject.GetComponent<Animator>().speed = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (VBM.GetCurrentVelocity() < 50f)
        {
            paste.sprite = copy_dark.sprite;
        } else
        {
            if (lightOn)
            {
                paste.sprite = copy_light.sprite;
            } else
            {
                paste.sprite = copy_dark.sprite;
            }
           
            //
        }
        
    }
}
