using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ncURL : MonoBehaviour {

	Button btn;
	public string UrlToOpen;

	// Use this for initialization
	void Start () {
		btn = this.GetComponent<Button>();
		btn.onClick.AddListener(openURL);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void openURL(){
		Application.OpenURL (UrlToOpen);
	}
}
