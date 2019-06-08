using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NCARSectionController : MonoBehaviour {
    public Slider slider;
    

	// Use this for initialization

	void Start () {
		
	}
    void Awaek()
    {
        slider.onValueChanged.AddListener((value) => {
            ChangeDistance(value);
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeDistance(float num)
    {
        transform.localPosition = new Vector3(0, 0, num);
        
    }
}
