﻿using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{
    private BoardScript board; // Provides data about players on map and map itself
    private int playerNum; // Number of this player
    private int numOfMoves; // Number of made moves in current series of moves

    // Use this for initialization
    void Start()
    {
        board = gameObject.GetComponent<BoardScript>();
        numOfMoves = 0;     
    }

    void Update()
    {
        if (networkView.isMine)
        {
            int onTurn = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>().OnTurn();
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                MoveLeft(onTurn);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                MoveRight(onTurn);
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                MoveUp(onTurn);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                MoveDown(onTurn);
        }
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

    #region Move functions

    public void MoveLeft(int onTurn)
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
                    SetNumOfMoves(numOfMoves+1);
            }
        }
        else if (playerNum == 1 && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos + 1))
            {
                board.playersPosition[playerNum].Z += 1;
                transform.Translate(0, 0, -1);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos + 1] == (int)BoardScript.Rooms.Hallway)
                    SetNumOfMoves(numOfMoves+1);
            }
        }
        else if ((playerNum == 2 || playerNum == 3) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos + 1, zPos))
            {
                board.playersPosition[playerNum].X += 1;
                transform.Translate(-1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos + 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    SetNumOfMoves(numOfMoves+1);
            }
        }
        else if ((playerNum == 4 || playerNum == 5) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos - 1))
            {
                board.playersPosition[playerNum].Z -= 1;
                transform.Translate(0, 0, 1);
                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos - 1] == (int)BoardScript.Rooms.Hallway)
                    SetNumOfMoves(numOfMoves+1);
            }
        }
    }

    public void MoveRight(int onTurn)
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
                    SetNumOfMoves(numOfMoves+1);
            }
        }
        else if (playerNum == 1 && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos - 1))
            {
                board.playersPosition[playerNum].Z -= 1;
                transform.Translate(0, 0, 1);
                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos - 1] == (int)BoardScript.Rooms.Hallway)
                    SetNumOfMoves(numOfMoves+1);
            }
        }
        else if ((playerNum == 2 || playerNum == 3) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos - 1, zPos))
            {
                board.playersPosition[playerNum].X -= 1;
                transform.Translate(1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos - 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    SetNumOfMoves(numOfMoves+1);
            }
        }
        else if ((playerNum == 4 || playerNum == 5) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos + 1))
            {
                board.playersPosition[playerNum].Z += 1;
                transform.Translate(0, 0, -1);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos + 1] == (int)BoardScript.Rooms.Hallway)
                    SetNumOfMoves(numOfMoves+1);
            }
        }
    }

    public void MoveUp(int onTurn)
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
                    SetNumOfMoves(numOfMoves+1);
            }
        }
        else if (playerNum == 1 && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos + 1, zPos))
            {
                board.playersPosition[playerNum].X += 1;
                transform.Translate(-1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos + 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    SetNumOfMoves(numOfMoves+1);
            }
        }
        else if ((playerNum == 2 || playerNum == 3) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos - 1))
            {
                board.playersPosition[playerNum].Z -= 1;
                transform.Translate(0, 0, 1);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos - 1] == (int)BoardScript.Rooms.Hallway)
                    SetNumOfMoves(numOfMoves+1);
            }
        }
        else if ((playerNum == 4 || playerNum == 5) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos - 1, zPos))
            {
                board.playersPosition[playerNum].X -= 1;
                transform.Translate(1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos - 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    SetNumOfMoves(numOfMoves+1);
            }
        }
    }

    public void MoveDown(int onTurn)
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
                    SetNumOfMoves(numOfMoves+1);
            }
        }
        else if (playerNum == 1 && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos - 1, zPos))
            {
                board.playersPosition[playerNum].X -= 1;
                transform.Translate(1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos - 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    SetNumOfMoves(numOfMoves+1);
            }
        }
        else if ((playerNum == 2 || playerNum == 3) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos, zPos + 1))
            {
                board.playersPosition[playerNum].Z += 1;
                transform.Translate(0, 0, -1);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos, zPos + 1] == (int)BoardScript.Rooms.Hallway)
                    SetNumOfMoves(numOfMoves+1);
            }
        }
        else if ((playerNum == 4 || playerNum == 5) && onTurn == playerNum)
        {
            if (board.isValid(playerNum, xPos, zPos, xPos + 1, zPos))
            {
                board.playersPosition[playerNum].X += 1;
                transform.Translate(-1, 0, 0);

                if (board.board[xPos, zPos] == (int)BoardScript.Rooms.Hallway || board.board[xPos + 1, zPos] == (int)BoardScript.Rooms.Hallway)
                    SetNumOfMoves(numOfMoves+1);
            }
        }
    }

    #endregion

    #region Get and set methods

    public void SetNum(int num)
    {
        networkView.RPC("SetNumRPC", RPCMode.AllBuffered, num);
    }

    public void SetNumOfMoves(int moves)
    {
        networkView.RPC("SetNumOfMovesRPC", RPCMode.AllBuffered, moves);
    }

    public int NumOfMoves()
    {
        return numOfMoves;
    }

    #endregion

    #region RPC set methods

    [RPC]
    private void SetNumRPC(int num)
    {
        playerNum = num;
        name = "Player" + num;
    }

    [RPC]
    private void SetNumOfMovesRPC(int moves)
    {
        numOfMoves = moves;
    }

    #endregion
}
