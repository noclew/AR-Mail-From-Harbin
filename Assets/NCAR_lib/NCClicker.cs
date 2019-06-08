using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NCClicker : MonoBehaviour {
    public GameObject showHideObject;
	public GameObject showHideObject2;

    Button btn;
    // Use this for initialization
    void Start () {
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(toggleActive);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void toggleActive()
    {
        Debug.Log("toggle");
        if (showHideObject != null) showHideObject.SetActive(!showHideObject.activeSelf);
		if (showHideObject2 != null) showHideObject2.SetActive(!showHideObject2.activeSelf);
    }
}
