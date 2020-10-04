using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemeArrowScript : DotScript
{
    // Start is called before the first frame update
    void Start()
    {
        //this.transform.Rotate(new Vector3(0,0,180 * Mathf.Deg2Rad));
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale = new Vector3(1/ this.transform.parent.localScale.x, 1 / this.transform.parent.localScale.y, 1);
    }
}
