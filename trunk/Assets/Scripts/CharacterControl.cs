using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{

    private CharacterController controller;
    private CollisionFlags collisionFlags;
    private BoardScript board;
    public int playerNum=0;
    private int numOfMoves;
    public int discesSum;
    public int onTurn = 1;


    public void setNumNet(int num)
    {
        networkView.RPC("setNum", RPCMode.AllBuffered, num);
    }

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



    // Use this for initialization
    void Start()
    {
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

    [RPC]
    void setNum(int num)
    {
        playerNum = num;
        name = "Player" + num;
    }
}
