using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoSingleton<GameManager>
{
    private int onTurn; // Denotes which player can move it's piece (players are enumerated from 0 to 5)
    private int numOfPlayers; // Keeps info about how many players are playing the game

    // Use this for initialization
    void Start()
    {
        onTurn = numOfPlayers = 0;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            string playerName = "Player" + onTurn;
            var playerObject = GameObject.Find(playerName).gameObject.GetComponent<CharacterControl>();
            // TODO: Remove constant and get real value from dices.
            if (10 == playerObject.NumOfMoves())
            {
                playerObject.SetNumOfMoves(0);
                setTurn((onTurn + 1) % numOfPlayers);
            }
        }
        catch (System.Exception)
        {
        }
    }

    #region Get and set methods

    public int OnTurn()
    {
        return onTurn;
    }

    public void setTurn(int turn)
    {
        networkView.RPC("setTurnRPC", RPCMode.AllBuffered, turn);
    }

    public void incrementNumberOfPlayers()
    {
        networkView.RPC("incrementNumberOfPlayersRPC", RPCMode.AllBuffered);
    }

    public void decrementNumberOfPlayers()
    {
        networkView.RPC("decrementNumberOfPlayersRPC", RPCMode.AllBuffered);
    }


    #endregion

    #region RPC set methods

    [RPC]
    private void incrementNumberOfPlayersRPC()
    {
        ++numOfPlayers;
    }

    [RPC]
    private void decrementNumberOfPlayersRPC()
    {
        --numOfPlayers;
    }

    [RPC]
    private void setTurnRPC(int turn)
    {
        onTurn = turn;
    }

    #endregion

}