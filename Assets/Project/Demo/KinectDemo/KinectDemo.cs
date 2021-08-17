using com.rfilkov.kinect;
using InteractionFramework.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinectDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MyGestureListener.Instance.GestureCompletedCallback += GestureCompletedHandler;
    }

    private void GestureCompletedHandler(GestureType obj)
    {
        if (obj == GestureType.RaiseRightHand)
        {
            Debug.Log("举起手来");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
