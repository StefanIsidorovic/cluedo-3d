using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{

    private CharacterController controller;
    private CollisionFlags collisionFlags;
    private BoardScript board;
    public int playerNum = 0;
    public int numOfMoves;

    public void setNumNet(int num)
    {
        networkView.RPC("setNum", RPCMode.AllBuffered, num);
    }
    
    public void setMoves(int moves)
    {
        networkView.RPC("setMovesRPC", RPCMode.AllBuffered, moves);
    }

    [RPC]
    private void setMovesRPC(int moves)
    {
        numOfMoves = moves;
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
            int onTurn = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>().onTurn;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                moveLeft(onTurn);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                moveRight(onTurn);
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                moveUp(onTurn);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                moveDown(onTurn);
        }
    }

    public void moveLeft(int onTurn)
    {
        int xPos, zPos;
        xPos = board.playersPosition[playerNum].X;
        zPos = board.playersPosition[playerNum].Z;

        if (playerNum == 0 && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos - 1, zPos))
            {
                board.playersPosition[playerNum].X -= 1;
                transform.Translate(1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos - 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + (xPos - 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
            }
        }
        else if (playerNum == 1 && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos + 1))
            {
                board.playersPosition[playerNum].Z += 1;
                transform.Translate(0, 0, -1);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos + 1] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + xPos + " zPos: " + (zPos + 1) + "BROJ POTEZA: " + numOfMoves);
            }
        }
        else if ((playerNum == 2 || playerNum == 3) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos + 1, zPos))
            {
                board.playersPosition[playerNum].X += 1;
                transform.Translate(-1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos + 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + (xPos + 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
            }
        }
        else if ((playerNum == 4 || playerNum == 5) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos - 1))
            {
                board.playersPosition[playerNum].Z -= 1;
                transform.Translate(0, 0, 1);
                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos - 1] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + xPos + " zPos: " + (zPos - 1) + "BROJ POTEZA: " + numOfMoves);
            }
        }
    }

    public void moveRight(int onTurn)
    {
        int xPos, zPos;
        xPos = board.playersPosition[playerNum].X;
        zPos = board.playersPosition[playerNum].Z;

        if (playerNum == 0 && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos + 1, zPos))
            {
                board.playersPosition[playerNum].X += 1;
                transform.Translate(-1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos + 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + (xPos + 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
            }
        }
        else if (playerNum == 1 && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos - 1))
            {
                board.playersPosition[playerNum].Z -= 1;
                transform.Translate(0, 0, 1);
                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos - 1] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + xPos + " zPos: " + (zPos - 1) + "BROJ POTEZA: " + numOfMoves);
            }
        }
        else if ((playerNum == 2 || playerNum == 3) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos - 1, zPos))
            {
                board.playersPosition[playerNum].X -= 1;
                transform.Translate(1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos - 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + (xPos - 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
            }
        }
        else if ((playerNum == 4 || playerNum == 5) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos + 1))
            {
                board.playersPosition[playerNum].Z += 1;
                transform.Translate(0, 0, -1);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos + 1] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + xPos + " zPos: " + (zPos + 1) + "BROJ POTEZA: " + numOfMoves);
            }
        }
    }

    public void moveUp(int onTurn)
    {
        int xPos, zPos;
        xPos = board.playersPosition[playerNum].X;
        zPos = board.playersPosition[playerNum].Z;

        if (playerNum == 0 && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos + 1))
            {
                board.playersPosition[playerNum].Z += 1;
                transform.Translate(0, 0, -1);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos + 1] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + xPos + " zPos: " + (zPos + 1) + "BROJ POTEZA: " + numOfMoves);
            }
        }
        else if (playerNum == 1 && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos + 1, zPos))
            {
                board.playersPosition[playerNum].X += 1;
                transform.Translate(-1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos + 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + (xPos + 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
            }
        }
        else if ((playerNum == 2 || playerNum == 3) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos - 1))
            {
                board.playersPosition[playerNum].Z -= 1;
                transform.Translate(0, 0, 1);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos - 1] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + xPos + " zPos: " + (zPos - 1) + "BROJ POTEZA: " + numOfMoves);
            }
        }
        else if ((playerNum == 4 || playerNum == 5) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos - 1, zPos))
            {
                board.playersPosition[playerNum].X -= 1;
                transform.Translate(1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos - 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + (xPos - 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
            }
        }
    }

    public void moveDown(int onTurn)
    {
        int xPos, zPos;
        xPos = board.playersPosition[playerNum].X;
        zPos = board.playersPosition[playerNum].Z;

        if (playerNum == 0 && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos - 1))
            {
                board.playersPosition[playerNum].Z -= 1;
                transform.Translate(0, 0, 1);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos - 1] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + xPos + " zPos: " + (zPos - 1) + "BROJ POTEZA: " + numOfMoves);
            }
        }
        else if (playerNum == 1 && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos - 1, zPos))
            {
                board.playersPosition[playerNum].X -= 1;
                transform.Translate(1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos - 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + (xPos - 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
            }
        }
        else if ((playerNum == 2 || playerNum == 3) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos + 1))
            {
                board.playersPosition[playerNum].Z += 1;
                transform.Translate(0, 0, -1);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos + 1] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + xPos + " zPos: " + (zPos + 1) + "BROJ POTEZA: " + numOfMoves);
            }
        }
        else if ((playerNum == 4 || playerNum == 5) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos + 1, zPos))
            {
                board.playersPosition[playerNum].X += 1;
                transform.Translate(-1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos + 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    setMoves(numOfMoves+1);

                Debug.Log("xPos: " + (xPos + 1) + " zPos: " + zPos + "BROJ POTEZA: " + numOfMoves);
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
