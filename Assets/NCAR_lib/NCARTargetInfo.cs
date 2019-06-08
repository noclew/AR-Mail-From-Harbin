using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NCARTargetInfo : MonoBehaviour {

    /// new method
    public GameObject defaultModel;

    // initial target transfrom information
    public Vector3 posTargetInit { get; set; }
    public Vector3 scaleTargetInit { get; set; }
    public Quaternion rotTargetInit { get; set; }
    /// 

    // Use this for initialization
    void Start () {
        //set the default target for its default model
        if (defaultModel != null )
            defaultModel.GetComponent<NCARModelInfo>().defaultTarget = this;
        //initialize the initial transform information of this target
        posTargetInit = transform.position;
        rotTargetInit = transform.rotation;
        scaleTargetInit = transform.lossyScale;

    }
	
	// Update is called once per frame
	void Update () {

    }

    
    //public Vector3 CalcInitialLocalPosOfModeltoThisTarget(Vector3 posModelInit)
    //{
    //    //this part is inverse of local to global (inverse function of TransformPoint).
    //    Vector3 t = Quaternion.Inverse(rotTargetInit) * (posModelInit - posTargetInit);
    //    t = Vector3.Scale(new Vector3(1 / scaleTargetInit.x, 1 / scaleTargetInit.y, 1 / this.scaleTargetInit.z), t);
    //    return t;
    //}

    //public void MoveModelOnThis(NCARModelInfo model)
    //{
    //    Transform trans = model.transform;
    //    Vector3 initLocal = CalcInitialLocalPosOfModeltoThisTarget(model.posInit);
    //    trans.SetPositionAndRotation(transform.TransformPoint(initLocal), model.rotInit * this.transform.rotation);
    //    trans.SetParent (this.transform);
    //}


}
