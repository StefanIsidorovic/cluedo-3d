using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour {

	private CharacterController controller;
	private CollisionFlags collisionFlags;
	private BoardScript board;
	public int playerNum;
	private int numOfMoves;
	public int discesSum;

	// Use this for initialization
	void Start () {
		board = gameObject.GetComponent<BoardScript> ();
		numOfMoves = 0;
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

		if (playerNum == 0)
		{
		int xPos, zPos;
		xPos = board.playersPosition [playerNum].X;
		zPos = board.playersPosition [playerNum].Z;
		if (Input.GetKeyDown (KeyCode.RightArrow)) 
		{
			if (board.isValid (playerNum, xPos, zPos, xPos + 1, zPos)) 
			{
				board.playersPosition [playerNum].X += 1;
				transform.Translate (-1, 0, 0);
				//FIX ME
				if(board.board[xPos,zPos] ==  10 || board.board[xPos+1,zPos] ==  10)
					numOfMoves++;

				Debug.Log("xPos: " + (xPos+1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
			}
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) 
		{
			if (board.isValid (playerNum, xPos, zPos, xPos - 1, zPos)) 
			{
				board.playersPosition [playerNum].X -= 1;
				transform.Translate (1, 0, 0);
				//FIX ME
				if(board.board[xPos,zPos] ==  10 || board.board[xPos-1,zPos] ==  10)
					numOfMoves++;

				Debug.Log("xPos: " + (xPos-1) + " zPos: " + zPos+ "BROJ POTEZA: " + numOfMoves);
			}
		}
		else if (Input.GetKeyDown (KeyCode.UpArrow))
		{			
			if (board.isValid (playerNum, xPos, zPos, xPos, zPos + 1)) 
			{
				board.playersPosition [playerNum].Z += 1;
				transform.Translate (0, 0, -1);
				//FIX ME
				if(board.board[xPos,zPos] ==  10 || board.board[xPos,zPos + 1] ==  10)
					numOfMoves++;

				Debug.Log("xPos: " + xPos + " zPos: " + (zPos + 1)+ "BROJ POTEZA: " + numOfMoves);
			}
		}
		else if (Input.GetKeyDown (KeyCode.DownArrow))
		{
			if (board.isValid (playerNum, xPos, zPos, xPos, zPos - 1)) 
			{
				board.playersPosition [playerNum].Z -= 1;
				transform.Translate (0, 0, 1);
				//FIX ME
				if(board.board[xPos,zPos] ==  10 || board.board[xPos,zPos-1] ==  10)
					numOfMoves++;

				Debug.Log("xPos: " + xPos + " zPos: " + (zPos - 1)+ "BROJ POTEZA: " + numOfMoves);
			}
		}

		}
	}
}
