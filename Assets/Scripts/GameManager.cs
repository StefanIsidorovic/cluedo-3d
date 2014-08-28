﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* #TODO:
 * 4. koji igrac je pokazao koju kartu set i get
 */

public class GameManager : MonoSingleton<GameManager>
{
    /// <summary>
    /// Reference to created empty gameObject with attached GUIScript on it.
    /// </summary>
    public GameObject GUIObject;
    /// <summary>
    /// Constant that represents invalid player. As player has ID which is int null can't be returned to indicate invalid player, thus this is solution.
    /// </summary>
    public const int INVALID_PLAYER_NUM = -1;
    /// <summary>
    /// Denotes which player can move it's piece (players are enumerated from 0 to 5).
    /// </summary>
    private int onTurn;
    /// <summary>
    /// Keeps info about how many players are playing the game.
    /// NOTE: This variable is dependent on NetworkManager to call GameManager.incrementNumberOfPlayers() method when each player is spawned.
    /// </summary>
    private int numOfPlayers;
    /// <summary>
    /// List of all available rooms in the game.
    /// </summary>
    private List<Rooms> allRooms;
    /// <summary>
    /// List of all available characters in the game.
    /// </summary>
    private List<Characters> allCharacters;
    /// <summary>
    /// List of all available weapons in the game.
    /// </summary>
    private List<Weapons> allWeapons;
    /// <summary>
    /// Game solution. Keeps indexes of solution room, person and weapon, from allRooms, allCharacters and allWeapons respectively.
    /// </summary>
    private Triple<int, int, int> solution;
    /// <summary>
    /// Keeps information which player has which cards after cards are dealt.
    /// </summary>
    private Dictionary<int, List<int>> cardsDistribution;
    /// <summary>
    /// Flag used in Update() method to decide if cards should be dealt.
    /// </summary>
    private bool shouldDeal;
    /// <summary>
    /// When player asks a question what is asked is saved here so it can be reused in GUI.
    /// </summary>
    private Pair<int, Triple<Rooms?, Characters?, Weapons?>> question;

    // Use this for initialization
    void Start()
    {
        onTurn = numOfPlayers = 0;
        allRooms = new List<Rooms>(new Rooms[]{ Rooms.Studio, Rooms.Hall, Rooms.GuestsRoom, Rooms.SleepingRoom, 
                                             Rooms.DiningRoom, Rooms.Cabinet, Rooms.Kitchen, Rooms.Billiard, 
                                             Rooms.Library, Rooms.Hallway
                                            });
        allCharacters = new List<Characters>(new Characters[] { Characters.MrsScarlet, Characters.MrBlack, Characters.MrsBlue, 
                                                             Characters.MrGreen, Characters.MrYellow, Characters.MrsWhite 
                                                            });
        allWeapons = new List<Weapons>(new Weapons[] { Weapons.Candlestick, Weapons.Knife, Weapons.LeadPipe, Weapons.Revolver, Weapons.Rope, Weapons.Wrench });
        shouldDeal = true;
        cardsDistribution = new Dictionary<int, List<int>>();
        question = new Pair<int, Triple<Rooms?, Characters?, Weapons?>>(GameManager.INVALID_PLAYER_NUM, 
                                                                        new Triple<Rooms?, Characters?, Weapons?>(null, null, null));
        GUIObject = new GameObject("GUI");
        GUIObject.AddComponent<GUIScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // Deal cards
        var networkManager = GameObject.Find("NetworkManager").gameObject.GetComponent<NetworkManager>();
        if (shouldDeal && Network.isServer && networkManager.GameStarted())
        {
            SetNumOfPlayers(networkManager.NumberOfPlayersConnected());
            InitSolution();
            DealCards();
            shouldDeal = false;
        }

        // Controll movement of players
        try
        {
            // Test if players are spawn and get current player 
            string playerName = "Player" + onTurn;
            var playerObject = GameObject.Find(playerName).gameObject.GetComponent<CharacterControl>();

            // TODO: Remove constant and get real value from dices.
            if (10 == playerObject.NumOfMoves())
            {
                playerObject.SetNumOfMoves(0);
                SetTurn((onTurn + 1) % numOfPlayers);
            }
        }
        catch (System.Exception)
        {
        }
    }

    #region Public interface for cards and players

    /// <summary>
    /// Provides list of all available rooms as IEnumerable.
    /// </summary>
    /// <returns>Returns list of all rooms.</returns>
    public IEnumerable<Rooms> AllRooms()
    {
        foreach (var room in allRooms)
        {
            yield return room;
        }
    }

    /// <summary>
    /// Provides list of all available characters as IEnumerable.
    /// </summary>
    /// <returns>Return list of characters.</returns>
    public IEnumerable<Characters> AllCharacters()
    {
        foreach (var person in allCharacters)
        {
            yield return person;
        }
    }

    /// <summary>
    /// Provides list of all available weapons as IEnumerable.
    /// </summary>
    /// <returns>Returns list of weapons.</returns>
    public IEnumerable<Weapons> AllWeapons()
    {
        foreach (var weapon in allWeapons)
        {
            yield return weapon;
        }
    }

    /// <summary>
    /// Returns all cards that are dealt to player.
    /// </summary>
    /// <param name="whichPlayer">Player whose cards are returned.</param>
    /// <returns>Returns list of cards as IEnumerable for given player.</returns>
    public IEnumerable<int> PlayerCards(int whichPlayer)
    {
        foreach (var card in cardsDistribution[whichPlayer])
        {
            yield return card;
        }
    }

    /// <summary>
    /// Interface to get player that has card.
    /// </summary>
    /// <param name="card">Wanted card.</param>
    /// <returns>Player who has card of GameManager.INVALID_PLAYER_NUM constant.</returns>
    public int PlayerWhoHasCard(int card)
    {
        foreach (var playerCardsPair in cardsDistribution)
        {
            if (playerCardsPair.Value.Contains(card))
            {
                return playerCardsPair.Key;
            }
        }
        return GameManager.INVALID_PLAYER_NUM;
    }

    /// <summary>
    /// Checks if player has card.
    /// </summary>
    /// <param name="whichPlayer">Player for which checking is done.</param>
    /// <param name="card">Card to check for.</param>
    /// <returns>True if player has card, false otherwise.</returns>
    public bool PlayerHasCard(int whichPlayer, int card)
    {
        return whichPlayer == PlayerWhoHasCard(card);
    }

    public void SetAskedQuestion(int whichPlayer, Rooms room, Characters person, Weapons weapon)
    {
        networkView.RPC("SetAskedQuestionRPC", RPCMode.AllBuffered, whichPlayer, (int)room, (int)person, (int)weapon);
    }

    public Pair<int, Triple<Rooms?, Characters?, Weapons?>> AskedQuestion()
    {
        return question;
    }

    #endregion

    #region Functions for cards dealing

    /// <summary>
    /// Chooses one card from all rooms, characters and weapons in random fashion (3 cards in total) and these cards are solution to the game.
    /// NOTE: Solution is saved in GameManager.solution.
    /// </summary>
    private void InitSolution()
    {
        int whichRoom = Random.Range(0, allRooms.Count - 2); // Hallway is not a valid room
        int whichPerson = Random.Range(0, allCharacters.Count - 1);
        int whichWeapon = Random.Range(0, allWeapons.Count - 1);
        SetSolution(whichRoom, whichPerson, whichWeapon);
    }

    /// <summary>
    /// Deals cards to players in round robin fashion.
    /// NOTE: Distribution of cards is saved in GameManager.cardsDistribution.
    /// </summary>
    private void DealCards()
    {
        // Fill all cards list with cards that are not in solution 
        List<int> allCards = new List<int>();
        foreach (var room in allRooms)
        {
            if (room != allRooms[solution.First] && room != Rooms.Hallway) allCards.Add((int)room);
        }
        foreach (var character in allCharacters)
        {
            if (character != allCharacters[solution.Second]) allCards.Add((int)character);
        }
        foreach (var weapon in allWeapons)
        {
            if (weapon != allWeapons[solution.Third]) allCards.Add((int)weapon);
        }

        // Create lists in dictionary for players
        networkView.RPC("InitCardsDistributionRPC", RPCMode.AllBuffered);

        // Shuffle and deal cards
        Algorithms.Shuffle<int>(allCards);
        int currentPlayer = 0;
        foreach (var card in allCards)
        {
            DealCardToPlayer(card, currentPlayer);
            currentPlayer = (currentPlayer + 1) % numOfPlayers;
        }
    }

    #endregion

    #region Get and set methods

    /// <summary>
    /// Getter for GameManager.onTurn field.
    /// </summary>
    /// <returns>Current value of GameManager.onTurn field</returns>
    public int OnTurn()
    {
        return onTurn;
    }

    /// <summary>
    /// Setter for GameManager.onTurn field.
    /// NOTE: This setter affects all the clones of this object as it is wrapper for RPC call.
    /// </summary>
    /// <param name="turn"></param>
    public void SetTurn(int turn)
    {
        networkView.RPC("SetTurnRPC", RPCMode.AllBuffered, turn);
    }

    public void SetNumOfPlayers(int numberOfPlayers)
    {
        networkView.RPC("SetNumOfPlayersRPC", RPCMode.AllBuffered, numberOfPlayers);
    }

    public void SetSolution(int room, int person, int weapon)
    {
        networkView.RPC("SetSolutionRPC", RPCMode.AllBuffered, room, person, weapon);
    }

    public void DealCardToPlayer(int card, int player)
    {
        networkView.RPC("DealCardToPlayerRPC", RPCMode.AllBuffered, card, player);
    }

    #endregion

    #region RPC set methods

    /// <summary>
    /// RPC setter for GameManager.onTurn field.
    /// </summary>
    /// <param name="turn">Value to set GameManager.onTurn parameter to.</param>
    [RPC]
    private void SetTurnRPC(int turn)
    {
        onTurn = turn;
    }

    [RPC]
    private void SetNumOfPlayersRPC(int numberOfPlayers)
    {
        numOfPlayers = numberOfPlayers;
    }

    [RPC]
    private void SetSolutionRPC(int room, int person, int weapon)
    {
        solution = new Triple<int, int, int>(room, person, weapon);
    }

    [RPC]
    private void InitCardsDistributionRPC()
    {
        for (int i = 0; i < numOfPlayers; i++)
        {
            cardsDistribution.Add(i, new List<int>());
        }
    }

    [RPC]
    private void DealCardToPlayerRPC(int card, int player)
    {
        cardsDistribution[player].Add(card);
    }

    [RPC]
    private void SetAskedQuestionRPC(int whichPlayer, int room, int person, int weapon)
    {
        question.First = whichPlayer;
        question.Second.First = (Rooms)room;
        question.Second.Second = (Characters)person;
        question.Second.Third = (Weapons)weapon;
    }

    #endregion

}