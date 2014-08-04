using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour {

	private CharacterController controller;
	private CollisionFlags collisionFlags;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		/*
		bool right = Input.GetKeyDown(KeyCode.RightArrow);
		bool left = Input.GetKeyDown(KeyCode.LeftArrow);
		bool  
		Vector3 v3_forward = new Vector3 (0, 0, 1);
		Vector3 v3_right = new Vector3 (1, 0, 0);

		Vector3 movement = hor * v3_right + ver * v3_forward;
		//controller = GetComponent(CharacterController);
		//collisionFlags = controller.Move(movement);
		//gameObject.transform.position.x = gameObject.transform.position.x + ver;
		//gameObject.transform.position.z = gameObject.transform.position.z + hor;
		transform.Translate (hor, 0, ver);
		*/

		//		controller = gameObject.GetComponent<CharacterController> ();

		if (Input.GetKeyDown (KeyCode.RightArrow))
			transform.Translate (1, 0, 0);
		else if (Input.GetKeyDown (KeyCode.LeftArrow))
			transform.Translate (-1, 0, 0);
		else if (Input.GetKeyDown (KeyCode.UpArrow))
			transform.Translate (0, 0, 1);
		else if (Input.GetKeyDown (KeyCode.DownArrow))
			transform.Translate (0, 0, -1);


	}
}
