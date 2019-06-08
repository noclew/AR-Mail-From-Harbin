using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NCARTouchController : MonoBehaviour
{
    enum ModelState { ATSTART, ATFINISH, GOINGUP, GOINGDOWN, ONPAUSE }

    private ModelState m_animState;
    private Transform trans;
    private Vector3 posInit;

    private Vector3 vecMove = new Vector3(0, 0.75f, 0);
    private Vector3 posFin;
    public float pauseTime = 2.0f;
    // Use this for initialization
    void Start()
    {
        setInitialLocalPosition();
    }

    public void setInitialLocalPosition()
    {
        m_animState = ModelState.ATSTART;
        this.trans = GetComponent<Transform>();
        this.posInit = trans.localPosition;
        this.posFin = posInit + vecMove;
    }

    void PrintPos()
    {
        Debug.Log("---currentPos: " + trans.localPosition);
    }


    IEnumerator PauseForSecond()
    {
        print(">> I am on pause");
        yield return new WaitForSecondsRealtime(pauseTime);
        if (m_animState == ModelState.ONPAUSE)
        {
            m_animState = ModelState.GOINGDOWN;
        }
    }

    //coroutine to move the target
    public void MoveModel()
    {
        Debug.Log(m_animState);


        //move down
        if (m_animState == ModelState.GOINGDOWN)
        {
            Vector3 dist = posInit - trans.localPosition;
            trans.Translate(dist * Time.deltaTime * 500);
            if (trans.localPosition != posInit && Vector3.Distance(trans.localPosition, posInit) < 0.01f)
            {
                trans.localPosition = posInit;
                m_animState = ModelState.ATSTART;
                Debug.Log(">> translation finished at the starting point");

            }
        }
        //move up
        else if (m_animState == ModelState.GOINGUP)
        {
            Vector3 dist = posFin - trans.localPosition;
            trans.Translate(dist * Time.deltaTime * 500);
            if (trans.localPosition != posFin && Vector3.Distance(trans.localPosition, posFin) < 0.01f)
            {
                trans.localPosition = posFin;
                m_animState = ModelState.ATFINISH;
                Debug.Log(">> translation finished at the fin point");
            }
            //trans.localPosition = posFin;
        }


    }

    public void ResetAnimation()
    {
        print(">> aminReset");
        transform.localPosition = posInit;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_animState == ModelState.GOINGUP || m_animState == ModelState.GOINGDOWN)
        {
            MoveModel();
        }

        else if (m_animState == ModelState.ATFINISH)
        {
            m_animState = ModelState.ONPAUSE;
            StartCoroutine(PauseForSecond());
        }
    }



    void OnMouseDown()
    {
        if (Input.touchSupported && Input.touchCount != 1)
            return;

        Debug.Log("------------------This Object is Pressed!");;

        switch (m_animState)
        {
            case ModelState.ATSTART:
                m_animState = ModelState.GOINGUP;
                break;

            case ModelState.GOINGUP:
                m_animState = ModelState.GOINGDOWN;
                break;

            case ModelState.GOINGDOWN:
                m_animState = ModelState.GOINGUP;
                break;

            case ModelState.ONPAUSE:
                m_animState = ModelState.GOINGDOWN;
                break;

        }


        //isMoving = true;
        //isGoingUp = !isGoingUp;
        return;
    }
}