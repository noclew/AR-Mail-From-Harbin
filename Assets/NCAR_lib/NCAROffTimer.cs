using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NCAROffTimer : MonoBehaviour
{
    public float offTimeInSec = 300f;
    public float offCountdown = 30f;
    bool isExtending = false;


    public GameObject autoExitScreen;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(StartTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartTimer()
    {
        yield return new WaitForSecondsRealtime(offTimeInSec);
        autoExitScreen.SetActive(true);
        StartCoroutine(StartTimer());
        StartCoroutine(ExitCountDown());
    }

    IEnumerator ExitCountDown()
    {
        yield return new WaitForSecondsRealtime(offCountdown);

        if (!isExtending)
        {
            print(">>>Ding!!");
            Application.Quit();
        }
        isExtending = false;
    }

    public void ExtendTimer()
    {
        isExtending = true;
    }

}
