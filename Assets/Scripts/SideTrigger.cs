using UnityEngine;
using System.Collections;

public class SideTrigger : MonoBehaviour {

	public int faceValue = 0;
	
	void OnTriggerEnter(Collider other ) {
		
		var dieGameObject = GameObject.Find("Dice1");
		
		var dieValueComponent = dieGameObject.GetComponent<DieValue>();
		
		dieValueComponent.currentValue = faceValue;
		
		Debug.Log(faceValue);
		
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
