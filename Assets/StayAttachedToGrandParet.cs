using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayAttachedToGrandParet : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = this.gameObject.transform.parent.parent.position;
    }
}
