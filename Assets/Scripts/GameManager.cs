using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoSingleton<GameManager>
{
    public int onTurn = 0;
    public int sumDice = 10;
    public string playerName;
    public bool playing = true;
    private int numOfPlayers;

    public void incrementNumberOfPlayers()
    {
        networkView.RPC("incrementNumberOfPlayersRPC", RPCMode.AllBuffered);
    }

    [RPC]
    private void incrementNumberOfPlayersRPC()
    {
        ++numOfPlayers;
    }

    public void decrementNumberOfPlayers()
    {
        networkView.RPC("decrementNumberOfPlayersRPC", RPCMode.AllBuffered);
    }

    [RPC]
    private void decrementNumberOfPlayersRPC()
    {
        --numOfPlayers;
    }

    public void setTurn(int turn)
    {
        networkView.RPC("setTurnRPC", RPCMode.AllBuffered, turn);
    }

    [RPC]
    private void setTurnRPC(int turn)
    {
        onTurn = turn;
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        try
		{    
			playerName = "Player" + onTurn;
            var playerObject = GameObject.Find(playerName).gameObject.GetComponent<CharacterControl>();
            if (sumDice == playerObject.numOfMoves)
            {
                playerObject.setMoves(0);
                setTurn((onTurn + 1) % numOfPlayers);
            }
		} catch(System.Exception)
		{
		}
    }

    void throwDices()
    {
        sumDice = Random.Range(1, 6) + Random.Range(1, 6);
    }
}