using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NCARARCamCopied : MonoBehaviour {

    public Camera arCam;
    Camera cam;
    // Use this for initialization

    float maxFov;
    float minFov = 10f;
    bool isFovInitiated = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    void Start () {
        cam.fieldOfView = arCam.fieldOfView;   
    }
	
	// Update is called once per frame
	void Update () {

        maxFov = arCam.fieldOfView;

        float newFov = cam.fieldOfView;
        //if (Input.GetAxis("Mouse ScrollWheel") < 0)
        //{
        //    newFov = GetComponent<Camera>().fieldOfView + 2;
        //}
        //if (Input.GetAxis("Mouse ScrollWheel") > 0)
        //{
        //    newFov = GetComponent<Camera>().fieldOfView - 2;
        //}

        newFov = Mathf.Clamp(newFov, minFov, maxFov);

        GetComponent<Camera>().fieldOfView = newFov;


    }
}
