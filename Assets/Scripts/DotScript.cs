using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotScript : MonoBehaviour
{

    public Sprite glowed_up;
    public Sprite very_glowed_up;
    public Sprite non_glowed_up;
    public bool expand_on_glowup;

    public GameObject optional_propagate_glow = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GlowMeUpScotty()
    {
        gameObject.GetComponent<Image>().sprite = glowed_up;
        if (expand_on_glowup)
            gameObject.transform.localScale = new Vector3(2, 2, 1);

        if (optional_propagate_glow != null)
        {
            optional_propagate_glow?.GetComponent<MemeArrowScript>().GlowMeUpScotty();

        }
            
    }

    public void GlowMeUpISaid()
    {
        gameObject.GetComponent<Image>().sprite = very_glowed_up;
        if (expand_on_glowup)
            gameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    public void GlowMeDownScotty()
    {
        gameObject.GetComponent<Image>().sprite = non_glowed_up;
        if (expand_on_glowup)
            gameObject.transform.localScale = new Vector3(1, 1, 1);

        if (optional_propagate_glow != null)
            optional_propagate_glow?.GetComponent<MemeArrowScript>().GlowMeDownScotty();
    }
}
