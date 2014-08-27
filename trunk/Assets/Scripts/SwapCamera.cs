using UnityEngine;
using System.Collections;

public class SwapCamera : MonoBehaviour {

    /// <summary>
    /// Main Camera
    /// </summary>
    public Camera cam1;

    /// <summary>
    /// TopView Camera
    /// </summary>
    public Camera cam2;

	void Start () {
        cam1.enabled = true;
        cam2.enabled = false;
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cam1.enabled = !cam1.enabled;
            cam2.enabled = !cam2.enabled;
        }
	}
}
