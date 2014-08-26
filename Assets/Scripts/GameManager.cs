using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoSingleton<GameManager>
{
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
        InitSolution();
        shouldDeal = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Controll movement of players
        try
        {
            // Test if players are spawn and get current player 
            string playerName = "Player" + onTurn;
            var playerObject = GameObject.Find(playerName).gameObject.GetComponent<CharacterControl>();

            // Deal cards
            if (shouldDeal)
            {
                DealCards();
                shouldDeal = false;
            }

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

    #endregion

    #region Functions for cards dealing

    /// <summary>
    /// Chooses one card from all rooms, characters and weapons in random fashion (3 cards in total) and these cards are solution to the game.
    /// NOTE: Solution is saved in GameManager.solution.
    /// </summary>
    private void InitSolution()
    {
        int whichRoom = Random.Range(0, allRooms.Count - 1);
        int whichPerson = Random.Range(0, allCharacters.Count - 1);
        int whichWeapon = Random.Range(0, allWeapons.Count - 1);
        solution = new Triple<int, int, int>(whichRoom, whichPerson, whichWeapon);
    }

    /// <summary>
    /// Deals cards to players in round robin fashion.
    /// NOTE: Distribution of cards is saved in GameManager.cardsDistribution.
    /// </summary>
    private void DealCards()
    {
        // Fill all cards list with cards that are not in solution 
        cardsDistribution = new Dictionary<int, List<int>>();
        List<int> allCards = new List<int>();
        foreach (var room in allRooms)
        {
            if (room != allRooms[solution.First]) allCards.Add((int)room);
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
        for (int i = 0; i < numOfPlayers; i++)
        {
            cardsDistribution.Add(i, new List<int>());
        }

        // Shuffle and deal cards
        Algorithms.Shuffle<int>(allCards);
        int currentPlayer = 0;
        foreach (var card in allCards)
        {
            cardsDistribution[currentPlayer].Add(card);
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
    public void setTurn(int turn)
    {
        networkView.RPC("setTurnRPC", RPCMode.AllBuffered, turn);
    }

    /// <summary>
    /// Increments indicator for how many players are playing the game.
    /// </summary>
    public void incrementNumberOfPlayers()
    {
        networkView.RPC("incrementNumberOfPlayersRPC", RPCMode.AllBuffered);
    }

    /// <summary>
    /// Decrements indicator for how many players are playing the game.
    /// </summary>
    public void decrementNumberOfPlayers()
    {
        networkView.RPC("decrementNumberOfPlayersRPC", RPCMode.AllBuffered);
    }

    #endregion

    #region RPC set methods

    /// <summary>
    /// RPC for incrementing indicator for how many players are playing the game.
    /// </summary>
    [RPC]
    private void incrementNumberOfPlayersRPC()
    {
        ++numOfPlayers;
    }

    /// <summary>
    /// RPC for decrementing indicator for how many players are playing the game.
    /// </summary>
    [RPC]
    private void decrementNumberOfPlayersRPC()
    {
        --numOfPlayers;
    }

    /// <summary>
    /// RPC setter for GameManager.onTurn field.
    /// </summary>
    /// <param name="turn">Value to set GameManager.onTurn parameter to.</param>
    [RPC]
    private void setTurnRPC(int turn)
    {
        onTurn = turn;
    }

    #endregion

}