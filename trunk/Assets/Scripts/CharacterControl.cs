﻿using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{
    private BoardScript board; // Provides data about players on map and map itself
    private int playerNum; // Number of this player
    private int numOfMoves; // Number of made moves in current series of moves
    private string publicName; //Name that player choose

    // Use this for initialization
    void Start()
    {
        board = GameObject.Find("Board").gameObject.GetComponent<BoardScript>();
        numOfMoves = 0;
    }

    void Update()
    {
        if (networkView.isMine)
        {
            var gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();

            //Player must throw dices before his movement
            if (gameManager.DicesSum() == GameManager.INVALID_DICES_SUM)
                return;

            int onTurn = gameManager.OnTurn();
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
        var playerPos = board.PlayerPos(playerNum);
        int xPos = playerPos.X;
        int zPos = playerPos.Z;

        if (playerNum == 0 && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos - 1, zPos))
            {
                board.SetPlayerPosition(playerNum, xPos - 1, zPos);
                transform.Translate(1, 0, 0);

                if (board.board[xPos - 1, zPos] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
        else if (playerNum == 1 && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos, zPos + 1))
            {
                board.SetPlayerPosition(playerNum, xPos, zPos + 1);
                transform.Translate(0, 0, -1);

                if (board.board[xPos, zPos + 1] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
        else if ((playerNum == 2 || playerNum == 3) && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos + 1, zPos))
            {
                board.SetPlayerPosition(playerNum, xPos + 1, zPos);
                transform.Translate(-1, 0, 0);

                if (board.board[xPos + 1, zPos] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
        else if ((playerNum == 4 || playerNum == 5) && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos, zPos - 1))
            {
                board.SetPlayerPosition(playerNum, xPos, zPos - 1);
                transform.Translate(0, 0, 1);
                if (board.board[xPos, zPos - 1] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
    }

    public void MoveRight(int onTurn)
    {
        var playerPos = board.PlayerPos(playerNum);
        int xPos = playerPos.X;
        int zPos = playerPos.Z;

        if (playerNum == 0 && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos + 1, zPos))
            {
                board.SetPlayerPosition(playerNum, xPos + 1, zPos);
                transform.Translate(-1, 0, 0);

                if (board.board[xPos + 1, zPos] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
        else if (playerNum == 1 && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos, zPos - 1))
            {
                board.SetPlayerPosition(playerNum, xPos, zPos - 1);
                transform.Translate(0, 0, 1);
                if (board.board[xPos, zPos - 1] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
        else if ((playerNum == 2 || playerNum == 3) && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos - 1, zPos))
            {
                board.SetPlayerPosition(playerNum, xPos - 1, zPos);
                transform.Translate(1, 0, 0);

                if (board.board[xPos - 1, zPos] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
        else if ((playerNum == 4 || playerNum == 5) && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos, zPos + 1))
            {
                board.SetPlayerPosition(playerNum, xPos, zPos + 1);
                transform.Translate(0, 0, -1);

                if (board.board[xPos, zPos + 1] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
    }

    public void MoveUp(int onTurn)
    {
        var playerPos = board.PlayerPos(playerNum);
        int xPos = playerPos.X;
        int zPos = playerPos.Z;

        if (playerNum == 0 && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos, zPos + 1))
            {
                board.SetPlayerPosition(playerNum, xPos, zPos + 1);
                transform.Translate(0, 0, -1);

                if (board.board[xPos, zPos + 1] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
        else if (playerNum == 1 && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos + 1, zPos))
            {
                board.SetPlayerPosition(playerNum, xPos + 1, zPos);
                transform.Translate(-1, 0, 0);

                if (board.board[xPos + 1, zPos] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
        else if ((playerNum == 2 || playerNum == 3) && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos, zPos - 1))
            {
                board.SetPlayerPosition(playerNum, xPos, zPos - 1);
                transform.Translate(0, 0, 1);

                if (board.board[xPos, zPos - 1] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
        else if ((playerNum == 4 || playerNum == 5) && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos - 1, zPos))
            {
                board.SetPlayerPosition(playerNum, xPos - 1, zPos);
                transform.Translate(1, 0, 0);

                if (board.board[xPos - 1, zPos] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
    }

    public void MoveDown(int onTurn)
    {
        var playerPos = board.PlayerPos(playerNum);
        int xPos = playerPos.X;
        int zPos = playerPos.Z;

        if (playerNum == 0 && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos, zPos - 1))
            {
                board.SetPlayerPosition(playerNum, xPos, zPos - 1);
                transform.Translate(0, 0, 1);

                if (board.board[xPos, zPos - 1] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
        else if (playerNum == 1 && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos - 1, zPos))
            {
                board.SetPlayerPosition(playerNum, xPos - 1, zPos);
                transform.Translate(1, 0, 0);

                if (board.board[xPos - 1, zPos] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
        else if ((playerNum == 2 || playerNum == 3) && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos, zPos + 1))
            {
                board.SetPlayerPosition(playerNum, xPos, zPos + 1);
                transform.Translate(0, 0, -1);

                if (board.board[xPos, zPos + 1] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
        else if ((playerNum == 4 || playerNum == 5) && onTurn == playerNum)
        {
            if (board.IsValid(playerNum, xPos, zPos, xPos + 1, zPos))
            {
                board.SetPlayerPosition(playerNum, xPos + 1, zPos);
                transform.Translate(-1, 0, 0);

                if (board.board[xPos + 1, zPos] == (int)Rooms.Hallway)
                    SetNumOfMoves(numOfMoves + 1);
            }
        }
    }

    #endregion

    #region Get and set methods

    public int GetPlayerNum()
    {
        return playerNum;
    }

    public void SetNum(int num)
    {
        networkView.RPC("SetNumRPC", RPCMode.AllBuffered, num);
    }

    public void SetNumOfMoves(int moves)
    {
        networkView.RPC("SetNumOfMovesRPC", RPCMode.AllBuffered, moves);
    }

    public void SetMaterial(int mat)
    {
        networkView.RPC("SetMaterialRPC", RPCMode.AllBuffered, mat);
    }

    public int NumOfMoves()
    {
        return numOfMoves;
    }

    public void SetPublicName(string playerName)
    {
        networkView.RPC("SetPublicNameRPC", RPCMode.AllBuffered, playerName);
    }

    public string PublicName()
    {
        return publicName;
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

    [RPC]
    private void SetMaterialRPC(int materialIndex)
    {
        Material mat = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().playerMaterials[materialIndex];
        transform.Find("PlayerGreen").renderer.material = mat;
        transform.Find("Sphere").renderer.material = mat;
    }

    [RPC]
    private void SetPublicNameRPC(string playerName)
    {
        publicName = playerName;
    }
    #endregion
}