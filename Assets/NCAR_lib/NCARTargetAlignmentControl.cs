using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using NCEH = NCARTrackableEventHandler;

public class NCARTargetAlignmentControl : MonoBehaviour
{
    public Transform debugPanel;
    public GameObject goModelList;
    public List<GameObject> modelList;
    public float angleThreshold = 10;
    public float dispThreshold = 25f;
    public float m_looseness = 1f;
    public float distanceFactorScaler = 0.002f;

    private bool isRoofTargetPerpendicular = false;

    // modifying looseness from the camera distance.
    public float loosenessMod { get; set; }

    // target List
    [Header("AR Target List")]
    public NCEH tgBotLeft;
    public NCEH tgBotRight;
    public NCEH tgTopLeft;
    public NCEH tgTopRight;
    public NCEH tgRoof;

    // model List
    [Header("3D Model List")]
    public Transform topLeft_1;
    public Transform topRight_1;
    public Transform botLeft_1;
    public Transform botRight_1;

    public Transform topLeft_2;
    public Transform topRight_2;
    public Transform botLeft_2;
    public Transform botRight_2;
    public Transform context;

    public NCARDefaultTrackableHandlerExtended scissor;

    /// <summary>
    /// initiate models to have all the required scripts.
    /// 1) this part adds two scripts. 
    ///     - NCARModelInfo : saves initial position information of the model. 
    ///     - NCARTouchController : adds touch function to a model
    /// 2) it also adds a bo collider that encapsulate all the child models. 
    /// </summary>
    private void Awake()
    {
        //modelList = new GameObject[goModelList.transform.childCount];
        for (int i = 0; i < goModelList.transform.childCount; i++)
        {
            goModelList.transform.GetChild(i).gameObject.AddComponent<NCARModelInfo>(); //add component for the initial location of the model
            modelList.Add(goModelList.transform.GetChild(i).gameObject);

            if (goModelList.transform.GetChild(i).gameObject == context.gameObject)
            {
                continue;
            }
             
            NCARHelperTools.addBoxColliderEncapsulatingChildren(goModelList.transform.GetChild(i).gameObject); //add collider


        }
    }

    // Use this for initialization
    void Start()
    {

        //modelList = new GameObject[goModelList.transform.childCount];
        for (int i = 0; i < goModelList.transform.childCount; i++)
        {
            //initiate model information to store initial transform paprams
            goModelList.transform.GetChild(i).GetComponent<NCARModelInfo>().initiate();
        }
        //deactivate model database object
        this.goModelList.SetActive(false);
    }

    #region MOVE_MODELS_ON_TARGETS


    // this function calcuates the initial position of the model in relation to the target's local space. 
    // it assuems that the target's transform is in world coordination.
    public Vector3 CalcInitialLocalPosOfModelToTarget(NCARModelInfo modelInfo, global::NCARTargetInfo targetInfo)
    {
        //this part is inverse of local to global (inverse function of TransformPoint)
        Quaternion rotTargetInit = targetInfo.rotTargetInit;
        Vector3 scaleTargetInit = targetInfo.scaleTargetInit;
        Vector3 posTargetInit = targetInfo.posTargetInit;
        Vector3 posModelInit = modelInfo.posInit;

        Vector3 t = Quaternion.Inverse(rotTargetInit) * (posModelInit - posTargetInit);
        t = Vector3.Scale(new Vector3(1 / scaleTargetInit.x, 1 / scaleTargetInit.y, 1 / scaleTargetInit.z), t);
        return t;
    }

    //based on the function above, this function moves the model onto the target setting it as the model's parent
    public void MoveModelOnTarget(NCARModelInfo modelInfo, global::NCARTargetInfo targetInfo)
    {

        Transform modelTransfrom = modelInfo.transform;
        Vector3 initLocal = CalcInitialLocalPosOfModelToTarget(modelInfo, targetInfo);

        modelTransfrom.position = targetInfo.transform.TransformPoint(initLocal);
        modelTransfrom.rotation = (modelInfo.rotInit * targetInfo.transform.rotation) * Quaternion.Inverse(targetInfo.rotTargetInit);

        modelTransfrom.SetParent(targetInfo.transform);

        if (modelInfo.GetComponent<NCARTouchController>() != null)
        {
            modelInfo.GetComponent<NCARTouchController>().setInitialLocalPosition();
        }

        modelInfo.currentTarget = targetInfo;

        //show model in case it was hidden;
        modelInfo.ShowModel();

    }

    /// <summary>
    /// This function changes the current target of ModelInfo Class and set a flag to see if the model is combined to any other targets other than its default target
    /// </summary>
    /// <param name="modelInfo">Model info.</param>
    /// <param name="targetInfo">Target info.</param>
    public void ChangeModelTarget(NCARModelInfo modelInfo, NCARTargetInfo targetInfo)
    {
        modelInfo.currentTarget = targetInfo;
        modelInfo.isModelCombined = true;
    }


    #endregion //functions to move models on cards


    // Update is called once per frame
    void Update()
    {
        //testFlags
        bool s1 = false ;
        bool s2= false;
        bool s3= false;
        bool s4= false;
        bool sall = false;

        #region distance compensator
        //check the cam distanceloosenessMod = looseness;
        loosenessMod = m_looseness;
        Vector3 targetCenter = Vector3.zero;
        Vector3 cameroPosition = Camera.main.transform.position;

        if (tgTopLeft.isBeingTracked)
        {
            targetCenter = (tgTopLeft.wTopRight + tgTopLeft.wBottomLeft) / 2.0f;
            float camDist = Vector3.Distance(targetCenter, cameroPosition);

            float distFactor = Mathf.Clamp(camDist - 300.0f, 0, 1000f);
            loosenessMod = m_looseness + distFactor * distanceFactorScaler;
            //print(loosenessMod);
        }
        #endregion


        //first check if roof is perpendicular
        if (IsTargetPerpendicular(tgTopLeft, tgRoof, loosenessMod))
        {
            Debug.Log("perp!");
            isRoofTargetPerpendicular = true;
            ChangeModelTarget(topLeft_2.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());
            
        }


        //case 1 (2) top two cards
        if (IsTargetLeftRight(tgTopLeft, tgTopRight, loosenessMod))
        {
            ChangeModelTarget(topRight_1.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());
            if (isRoofTargetPerpendicular)
            {
                ChangeModelTarget(topRight_2.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());
            }

            s1 = true;

        }

        //case 2 (2) bot two cards
        if (IsTargetLeftRight(tgBotLeft, tgBotRight, loosenessMod))
        {
            ChangeModelTarget(botRight_1.GetComponent<NCARModelInfo>(), tgBotLeft.GetComponent<NCARTargetInfo>());

            s2 = true;
        }

        //case 3 (2) vertical left cards
        if (IsTargetsTopBottom(tgTopLeft, tgBotLeft, loosenessMod))
        {
            ChangeModelTarget(botLeft_1.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());

            if (isRoofTargetPerpendicular)
            {
                ChangeModelTarget(botLeft_2.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());
            }

            s3 = true;
        }

        // L case (3)
        if (IsTargetsTopBottom(tgTopLeft, tgBotLeft, loosenessMod) && IsTargetLeftRight(tgTopLeft, tgTopRight, loosenessMod))
        {
            ChangeModelTarget(topRight_1.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());
            ChangeModelTarget(botLeft_1.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());

            if (isRoofTargetPerpendicular)
            {
                ChangeModelTarget(topRight_2.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());
                ChangeModelTarget(botLeft_2.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());

                s4 = true;
            }

        }

        ////check case: all
        if (IsTargetsTopBottom(tgTopRight, tgBotRight, loosenessMod) 
            && IsTargetsTopBottom(tgTopLeft, tgBotLeft, loosenessMod) 
            && IsTargetLeftRight(tgTopLeft,tgTopRight, loosenessMod))
        {
            ChangeModelTarget(topRight_1.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());
            ChangeModelTarget(botLeft_1.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());
            ChangeModelTarget(botRight_1.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());


            if (isRoofTargetPerpendicular)
            {
                ChangeModelTarget(topRight_2.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());
                ChangeModelTarget(botLeft_2.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());
                ChangeModelTarget(botRight_2.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());
                ChangeModelTarget(context.GetComponent<NCARModelInfo>(), tgTopLeft.GetComponent<NCARTargetInfo>());
            }

                sall = true;
        }
        if (debugPanel != null)
        {
            string dt = "perp: "+ isRoofTargetPerpendicular + " / s1: " + s1 + " / s2: " + s2 + " / s3: " + s3 + " / s4: " + s4 + " / sall: " + sall;
            debugPanel.GetComponent<UnityEngine.UI.Text>().text = dt;
        }
        isRoofTargetPerpendicular = false;


        ////check Scissor
        //if (scissor.IsTracked && tgTopLeft.IsTracked )
        //{
        //    Debug.Log("cut!");
        //    tgTopLeft.GetComponent<NCARTargetInfo>().TurnOnChildFlag("all");
        //    topRight.GetComponent<NCARTargetInfo>().TurnOnChildFlag(null);
        //    tgBotRight.GetComponent<NCARTargetInfo>().TurnOnChildFlag(null);
        //    botLeft.GetComponent<NCARTargetInfo>().TurnOnChildFlag(null);

        //    //roof model control
        //    //if perpendicular, turn on roof of main


        //    tgTopLeft.GetComponent<NCARTargetInfo>().TurnOnChildFlag("roof_all", false);
        //}


        // disable moving function of the top-left model
        if (topLeft_2.GetComponent<NCARModelInfo>().isModelCombined == false)
        {
            topLeft_2.GetComponent<NCARTouchController>().enabled = false;
        }
        else if (topLeft_2.GetComponent<NCARModelInfo>().isModelCombined == true)
        {
            topLeft_2.GetComponent<NCARTouchController>().enabled = true;
        }
        UpdateModelStatus();
    

    }


    /// <summary>
    /// this function actually moves models on their associated targe using 'MoveModelOnTarget' function. 
    /// </summary>
    public void UpdateModelStatus()
    {
        foreach (GameObject model in modelList)
        {
            NCARModelInfo modelInfo = model.GetComponent<NCARModelInfo>();

            //if the model is combined to any other targets then its default target
            if (modelInfo.isModelCombined == true)
            {
                //and if the target has been changed
                if (modelInfo.previousTarget != modelInfo.currentTarget)
                {
                    //need this to exclude context model.
                    if (modelInfo.GetComponent<NCARTouchController>() != null)
                    {
                        modelInfo.GetComponent<NCARTouchController>().ResetAnimation();
                    }
                    MoveModelOnTarget(modelInfo, modelInfo.currentTarget);
                }
            }

            //if model is not combined to any other target
            else if (modelInfo.isModelCombined == false)
            {
                //and if the model does have the default target && target has been changed.
                if (modelInfo.defaultTarget != null && modelInfo.defaultTarget != modelInfo.currentTarget)
                {
                    //Move the model on its default taret
                    MoveModelOnTarget(modelInfo, model.GetComponent<NCARModelInfo>().defaultTarget);
                }

                // and if the model does not have the default target associated && target has been changed.
                if (modelInfo.defaultTarget == null && modelInfo.defaultTarget != modelInfo.currentTarget)
                {
                    model.transform.SetParent(goModelList.transform);
                    modelInfo.resetPosToDefault();
                }
                // set the target to its default
                modelInfo.currentTarget = modelInfo.defaultTarget;
            }

            if (modelInfo.currentTarget != null && modelInfo.currentTarget.GetComponent<NCARTrackableEventHandler>().IsTracked != true)
                modelInfo.HideModel();

            //set the preious target to the current one. 
            modelInfo.previousTarget = modelInfo.currentTarget;
            //reset the combination target to false;
            model.GetComponent<NCARModelInfo>().isModelCombined = false;
        }
    }

    bool IsTargetsTopBottom(NCEH mTop, NCEH mBottom, float looseness = 1.0f)
    {
        Vector3 vLeft1 = mTop.wBottomLeft - mTop.wTopLeft;
        Vector3 vLeft2 = mBottom.wBottomLeft - mBottom.wTopLeft;
        float angLeft = Vector3.Angle(vLeft1, vLeft2);

        Vector3 vRight1 = mTop.wBottomRight - mTop.wTopRight;
        Vector3 vRight2 = mBottom.wBottomRight - mBottom.wTopRight;
        float angRight = Vector3.Angle(vRight1, vRight2);

        Vector3 diffLeft = mTop.wBottomLeft - mBottom.wTopLeft;
        Vector3 diffRight = mTop.wBottomRight - mBottom.wTopRight;
        //Debug.Log("--" + angBottom + "--" + angTop + "--" + diffTop.magnitude + "--" + diffBot.magnitude);
        return (mTop.IsTracked && mBottom.IsTracked
            && angLeft < angleThreshold * looseness
            && angRight < angleThreshold * looseness
            && diffLeft.magnitude < dispThreshold * looseness
            && diffRight.magnitude < dispThreshold * looseness);
    }


    bool IsTargetLeftRight(NCEH mLeft, NCEH mRight, float looseness = 1.0f)
    {
        Vector3 vBottom1 = mLeft.wBottomLeft - mLeft.wBottomRight;
        Vector3 vBottom2 = mRight.wBottomLeft - mRight.wBottomRight;
        float angBottom = Vector3.Angle(vBottom1, vBottom2);

        Vector3 vTop1 = mLeft.wTopLeft - mLeft.wTopRight;
        Vector3 vTop2 = mRight.wTopLeft - mRight.wTopRight;
        float angTop = Vector3.Angle(vTop1, vTop2);

        Vector3 diffBot = mLeft.wBottomRight - mRight.wBottomLeft;
        Vector3 diffTop = mLeft.wTopRight - mRight.wTopLeft;
        //Debug.Log("--"+ angBottom + "--" + angTop + "--" + diffTop.magnitude +"--"+ diffBot.magnitude);
        return (mLeft.IsTracked && mRight.IsTracked
            && angBottom < angleThreshold * looseness
            && angTop < angleThreshold * looseness
            && diffBot.magnitude < dispThreshold * looseness
            && diffTop.magnitude < dispThreshold * looseness);

    }

    bool IsTargetPerpendicular(NCEH floor, NCEH wall, float looseness = 1.0f)
    {
        Vector3 vFloorBotEdge = floor.wBottomRight - floor.wBottomLeft;
        Vector3 vWallBotEdge = wall.wBottomLeft - wall.wBottomRight;
        float angBottom = Vector3.Angle(vFloorBotEdge, vWallBotEdge);

        Vector3 vFloorTopEdge = floor.wTopRight - floor.wTopLeft;
        Vector3 vWallTopEdge = wall.wTopLeft - wall.wTopRight;
        float angTop = Vector3.Angle(vFloorTopEdge, vWallTopEdge);


        Vector3 diffBotLeft = floor.wBottomLeft - wall.wBottomRight;
        Vector3 diffTopLeft = floor.wTopLeft - wall.wTopRight;

        //Debug.Log("--angle: [" + angBottom + "]--[" + angTop + "]--dist:[" + diffBotLeft.magnitude + "]--[" + diffTopLeft.magnitude);
        //Debug.Log(angleThreshold * looseness);
        return (floor.IsTracked && wall.IsTracked
            && angBottom < (90 + (angleThreshold * looseness))
            && angBottom > (90 - (angleThreshold * looseness))
            && angTop < (90 + (angleThreshold * looseness))
            && angTop > (90 - (angleThreshold * looseness))
            && diffBotLeft.magnitude < dispThreshold * looseness
            && diffTopLeft.magnitude < dispThreshold * looseness);
    }

    public void ExitApp()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
