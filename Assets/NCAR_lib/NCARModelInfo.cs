using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NCARModelInfo : MonoBehaviour {

    public NCARTargetInfo defaultTarget { get; set; }
    public NCARTargetInfo currentTarget { get; set; }
    public NCARTargetInfo previousTarget { get; set; }

    public Vector3 posInit { get; set; }
    public Quaternion rotInit { get; set; }
    public Vector3 scaleInit { get; set; }
    private Vector3 scaleLocalInit;
    private Transform trParent;
    
    public bool isModelCombined { get; set; }
    public bool isModelVisible { get; set; }

    public GameObject targetAttached { get; set; }
    // Use this for initialization

    void Start () {
    }

    public void initiate()
    {
        posInit = transform.position;
        rotInit = transform.rotation;
        scaleInit = transform.lossyScale;
        scaleLocalInit = transform.localScale;
        //print(this.transform.name + "--" + scaleLocalInit);


        this.trParent = this.transform.parent;
        //HideModel();
    }
	public void resetPosToDefault()
    {
        this.transform.position = posInit;
        this.transform.rotation = rotInit;
        this.transform.localScale = scaleLocalInit;
    }
	// Update is called once per frame
	void Update () {
		
	}


    public void HideModel()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;
    }

    public void ShowModel()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;
    }
}
