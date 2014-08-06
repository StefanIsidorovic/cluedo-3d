using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{

    private CharacterController controller;
    private CollisionFlags collisionFlags;
    private BoardScript board;
    public int playerNum;
    private int numOfMoves;
    public int discesSum;
    public int onTurn = 1;

    //---- Network variables.
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 syncPosition = Vector3.zero;
        if (stream.isWriting)
        {
            syncPosition = rigidbody.position;
            stream.Serialize(ref syncPosition);
        }
        else
        {
            stream.Serialize(ref syncPosition);
            rigidbody.position = syncPosition;
        }
    }

    void Awake()
    {
        lastSynchronizationTime = Time.time;
    }


    // Use this for initialization
    void Start()
    {
        //#TODO  Spawning players in order.
        Vector3 p0 = new Vector3(-1, 0, 22);
        Vector3 p1 = new Vector3(5, 0, 15);
        Vector3 p2 = new Vector3(0, 0, 0);
        Vector3 p3 = new Vector3(-9, 0, 0);
        Vector3 p4 = new Vector3(-17, 0, 6);
        Vector3 p5 = new Vector3(-17, 0, 15);
        //playerNum = 0; 
        board = gameObject.GetComponent<BoardScript>();
        //numOfMoves = 0;
        //onTurn = 0;        
    }

    void Update()
    {
        if (networkView.isMine)
        {
            CharacterLogic();
        }
       
    }

 
    // Update is called once per frame
    void CharacterLogic()
    {

        if (playerNum == 0 && onTurn == playerNum)
        {
            int xPos, zPos;
            xPos = board.playersPosition[playerNum].X;
            zPos = board.playersPosition[playerNum].Z;
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos + 1, zPos))
                {
                    board.playersPosition[playerNum].X += 1;
                    transform.Translate(-1, 0, 0);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos + 1, zPos] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + (xPos + 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos - 1, zPos))
                {
                    board.playersPosition[playerNum].X -= 1;
                    transform.Translate(1, 0, 0);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos - 1, zPos] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + (xPos - 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos, zPos + 1))
                {
                    board.playersPosition[playerNum].Z += 1;
                    transform.Translate(0, 0, -1);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos + 1] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + xPos + " zPos: " + (zPos + 1) + "BROJ POTEZA: " + numOfMoves);
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos, zPos - 1))
                {
                    board.playersPosition[playerNum].Z -= 1;
                    transform.Translate(0, 0, 1);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos - 1] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + xPos + " zPos: " + (zPos - 1) + "BROJ POTEZA: " + numOfMoves);
                }
            }

        }
        else if (playerNum == 1 && onTurn == playerNum)
        {
            int xPos, zPos;
            xPos = board.playersPosition[playerNum].X;
            zPos = board.playersPosition[playerNum].Z;
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos, zPos - 1))
                {
                    board.playersPosition[playerNum].Z -= 1;
                    transform.Translate(0, 0, 1);
                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos - 1] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + xPos + " zPos: " + (zPos - 1) + "BROJ POTEZA: " + numOfMoves);
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos, zPos + 1))
                {
                    board.playersPosition[playerNum].Z += 1;
                    transform.Translate(0, 0, -1);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos + 1] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + xPos + " zPos: " + (zPos + 1) + "BROJ POTEZA: " + numOfMoves);
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos + 1, zPos))
                {
                    board.playersPosition[playerNum].X += 1;
                    transform.Translate(-1, 0, 0);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos + 1, zPos] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + (xPos + 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos - 1, zPos))
                {
                    board.playersPosition[playerNum].X -= 1;
                    transform.Translate(1, 0, 0);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos - 1, zPos] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + (xPos - 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
                }
            }

        }
        else if ((playerNum == 2 || playerNum == 3) && onTurn == playerNum)
        {
            int xPos, zPos;
            xPos = board.playersPosition[playerNum].X;
            zPos = board.playersPosition[playerNum].Z;
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos - 1, zPos))
                {
                    board.playersPosition[playerNum].X -= 1;
                    transform.Translate(1, 0, 0);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos - 1, zPos] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + (xPos - 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
                }

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos + 1, zPos))
                {
                    board.playersPosition[playerNum].X += 1;
                    transform.Translate(-1, 0, 0);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos + 1, zPos] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + (xPos + 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos, zPos - 1))
                {
                    board.playersPosition[playerNum].Z -= 1;
                    transform.Translate(0, 0, 1);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos - 1] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + xPos + " zPos: " + (zPos - 1) + "BROJ POTEZA: " + numOfMoves);
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos, zPos + 1))
                {
                    board.playersPosition[playerNum].Z += 1;
                    transform.Translate(0, 0, -1);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos + 1] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + xPos + " zPos: " + (zPos + 1) + "BROJ POTEZA: " + numOfMoves);
                }

            }

        }
        else if ((playerNum == 4 || playerNum == 5) && onTurn == playerNum)
        {
            int xPos, zPos;
            xPos = board.playersPosition[playerNum].X;
            zPos = board.playersPosition[playerNum].Z;
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos, zPos + 1))
                {
                    board.playersPosition[playerNum].Z += 1;
                    transform.Translate(0, 0, -1);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos + 1] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + xPos + " zPos: " + (zPos + 1) + "BROJ POTEZA: " + numOfMoves);
                }

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos, zPos - 1))
                {
                    board.playersPosition[playerNum].Z -= 1;
                    transform.Translate(0, 0, 1);
                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos - 1] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + xPos + " zPos: " + (zPos - 1) + "BROJ POTEZA: " + numOfMoves);
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos - 1, zPos))
                {
                    board.playersPosition[playerNum].X -= 1;
                    transform.Translate(1, 0, 0);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos - 1, zPos] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + (xPos - 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (board.isValid(playerNum, xPos, zPos, xPos + 1, zPos))
                {
                    board.playersPosition[playerNum].X += 1;
                    transform.Translate(-1, 0, 0);

                    if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos + 1, zPos] == (int)BoardScript.Rooms.Hallway)
                        numOfMoves++;

                    Debug.Log("xPos: " + (xPos + 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
                }

            }

        }

    }

}
