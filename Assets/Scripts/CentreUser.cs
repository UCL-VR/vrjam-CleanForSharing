using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentreUser : MonoBehaviour
{

    public GameObject userHead;
    public GameObject cameraRig;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("centre");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator centre()
    {
        yield return new WaitForEndOfFrame();
        // Get head position

        if (cameraRig!=null && userHead!=null) // Check it is an environment where we can retarget the camera, i.e. position tracking is on
        {
            //            Debug.Log("Start up (" + -userHead.transform.localPosition.x + "," + -userHead.transform.localPosition.z + ")");
            cameraRig.transform.Rotate(new Vector3(0, -userHead.transform.localEulerAngles.y, 0));
            cameraRig.transform.Translate(new Vector3(-userHead.transform.localPosition.x, 0, -userHead.transform.localPosition.z));
        }
    }
}
