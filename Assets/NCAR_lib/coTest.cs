using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coTest : MonoBehaviour {
    int a = 0;
	// Use this for initialization
	void Start () {
        StartCoroutine( dothis() );

    }
	
	// Update is called once per frame
	void Update () {
        a++;
	}

    IEnumerator dothis()
    {
        while (true)
        {
            print(a);
            yield return null;

        }
    }
}
