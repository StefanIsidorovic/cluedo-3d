using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GUIScript : MonoBehaviour
{
    //***IMPORTANT: Read Unity3D GUI documentation before edit.
    // Hint: Every position of element is defined as Rect where first two values are upper left corner and second two values
    // are down right corner.

    // Game manager instance
    private GameManager gameManager;
    private BoardScript board;
    private Texture2D backOfCard;
    #region Variables for sidebar elements
    // Help structures for Unity3D GUI elements (storing values and iterating through groups of elements aka type of cards)
    private Dictionary<string, bool> toogle;
    private Dictionary<string, string> textBoxes;
    private Dictionary<int, Texture2D> cardTextures;
    private Vector2 scrollPosition = Vector2.zero;
    private object[] dropdownCharacters, dropdownWeapons;
    public List<Texture2D> dieFacesVector;
    public bool dicesThrown = false;
    public int num1 = 0;
    public int num2 = 0;
    // helper variebles for defining Rectangles for positions of elements.
    // Height of space intended for dices.
    private int heightCoef = 0;
    // width of textbox.
    private int textBoxWidth = 160;
    #endregion

    #region Askdialog - Popuplist variables
    // dropdown variables
    private bool boolCh = false, boolWe = false;
    private int cardCharacter = 0, cardWeapon = 0, choseWe = 0, choseCh = 0;
    private string weapon = "Choose weapon", character = "Choose character", askButtonText = "Ask!";
    #endregion

    private int playerNum;

    private bool askDialogShow = false, someoneAsking = false, guardAsking = false;

    private Triple<Rooms, Characters, Weapons> askedFor;

    #region Askdialog position variables
    private Rect firstCard, secondCard, thirdCard, firstCardLabel, secondCardLabel, thirdCardLabel;
    private int stepW, stepH, widthAskDialog, heightAskDialog;
    private bool asking = false, askDialogShowCardsBool = false, askDialogShowQuestion = false;
    private int playerAsking = -1;
    private Rect boxForChosingCards;
    #endregion

    private bool setCardGuard = false;
    private bool questionAsk = false;
    private bool infoBox = false;
    private string infoBoxLabel = "";

    private Triple<int, int, int> playersWhoHaveCards;
    private bool questionToogle = false;
    private int numberOfProcessedPlayers = 0;

    private bool endGameInfo = false;
    // #TODO: Refactor code.
    // #NOTE: Move rect and help variables from functions;
    void Start()
    {
        backOfCard = (Texture2D)Resources.Load("BackOfCard", typeof(Texture2D));
        playersWhoHaveCards = new Triple<int, int, int>(-1, -1, -1);
        askedFor = new Triple<Rooms, Characters, Weapons>(0, 0, 0);
        gameManager = MonoSingleton<GameManager>.Instance;
        board = MonoSingleton<BoardScript>.Instance;
        heightCoef = 160;
        toogle = new Dictionary<string, bool>();
        textBoxes = new Dictionary<string, string>();
        cardTextures = new Dictionary<int, Texture2D>();
        cardTextures.Add(-1, backOfCard);
        foreach (var item in gameManager.AllRooms())
        {
            string strItem = EnumConverter.ToString(item);
            toogle.Add(strItem, false);
            textBoxes.Add(strItem, "");
            if (item != Rooms.Hallway)
                cardTextures.Add((int)item, (Texture2D)Resources.Load("Cards/" + strItem, typeof(Texture2D)));
        }
        foreach (var item in gameManager.AllWeapons())
        {
            string strItem = EnumConverter.ToString(item);
            toogle.Add(strItem, false);
            textBoxes.Add(strItem, "");
            cardTextures.Add((int)item, (Texture2D)Resources.Load("Cards/" + strItem, typeof(Texture2D)));
        }
        foreach (var item in gameManager.AllCharacters())
        {
            string strItem = EnumConverter.ToString(item);
            toogle.Add(strItem, false);
            textBoxes.Add(strItem, "");
            cardTextures.Add((int)item, (Texture2D)Resources.Load("Cards/" + strItem, typeof(Texture2D)));
        }
        dieFacesVector = new List<Texture2D>();
        dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/images", typeof(Texture2D)));
        // this
        for (int i = 1; i < 7; i++ )
            dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/"+i, typeof(Texture2D)));
        // instead of
        //dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/1", typeof(Texture2D)));
        //dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/2", typeof(Texture2D)));
        //dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/3", typeof(Texture2D)));
        //dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/4", typeof(Texture2D)));
        //dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/5", typeof(Texture2D)));
        //dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/6", typeof(Texture2D)));
    }

    // GUI elements

    void OnGUI()
    {
        if (infoBox)
        {
            InfoBox();
        }

        InitAskDialogPositionVariables();
        if (GameObject.Find("NetworkManager").gameObject.GetComponent<NetworkManager>().GameStarted() && !endGameInfo)
        {
            if (!setCardGuard)
            {
                playerNum = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().NumOfMyPlayer();
                setCardGuard = true;
                setCards(playerNum);
            }

            ShowSideBar();
        }

        if (questionAsk)
        {
            AskDialog_ChoosingCards(true);
        }

        // Showing initial dialog for choosing cards to form a question, and setting variable someoneasking to true, so other player 
        // know when somebody else is forming a question.
        if (askDialogShow)
        {
            AskDialog_ChoosingCards();
            setSomeoneAskingRPC(true);
        }
        // Show cards from question asked.
        if (askDialogShowQuestion && playerAsking != playerNum )
        {
            AskDialog_ShowQuestionCards();
        }
        // Dialog for choosing card to show.
        if(askDialogShowCardsBool)
        {
            AskDialog_ChooseCardsToShow();
        }

        if (numberOfProcessedPlayers == gameManager.NumOfPlayers() && GameObject.Find("NetworkManager").gameObject.GetComponent<NetworkManager>().GameStarted() )
        {
            AskDialog_ShowCardsToPlayers(playerAsking == playerNum);
        }
    }

    #region Rest Of GUI
    private void ShowSideBar()
    {

        scrollPosition = GUI.BeginScrollView(
            new Rect(Percentage(Screen.width, 75), 0, Percentage(Screen.width, 25), Screen.height),
            scrollPosition,
            new Rect(0, 0, Percentage(Screen.width, 25) - 25, 21 * 20 + 60 + heightCoef + 80)
        );

        GUI.Box(new Rect(0, 0, Percentage(Screen.width, 25) - 25, 21 * 20 + 60 + heightCoef + 80), "");
        
        // Generate 2d part for throwing dices - everything about this part is within a group.
 
        GUI.BeginGroup(new Rect(0, 0, Percentage(Screen.width, 25) - 25, heightCoef - 10));

        //two rect for presenting the dices and centering them 
        GUI.Box(new Rect((Percentage(Screen.width, 25) - 25)/2 - 60, 20, 40, 40), dieFacesVector[num1]);
        GUI.Box(new Rect((Percentage(Screen.width, 25) - 25)/2 + 20, 20, 40, 40), dieFacesVector[num2]);
        //chacking if this player is on turn

        int onTurn = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>().OnTurn();
        GameObject myPlayer = null;
        int numberOfPlayers = GameObject.Find("NetworkManager").gameObject.GetComponent<NetworkManager>().NumberOfPlayersConnected();

        for (int j = 0; j < numberOfPlayers; j++)
        {
            var player = GameObject.Find("Player" + j);
            if (player != null && player.networkView.isMine)
                myPlayer = GameObject.Find("Player" + j);
        }
        if (myPlayer != null)
        {
            //int playerNum = myPlayer.gameObject.GetComponent<CharacterControl>().GetPlayerNum();
            if (playerNum != onTurn || dicesThrown)
                GUI.enabled = false;
            int numberOfMovesMade = myPlayer.gameObject.GetComponent<CharacterControl>().NumOfMoves();
            if (GUI.Button(new Rect((Percentage(Screen.width, 25) - 25)/2 - 50, 80, 100, 50), "Throw dices!"))
            {
                ThrowDices();
                GameObject.Find("GameManager").gameObject.GetComponent<GameManager>().SetDicesSum((num1+num2));
            }
            if (numberOfMovesMade == (num1 + num2))
            {
                dicesThrown = false;
            }
            if (playerNum != onTurn || dicesThrown)
                GUI.enabled = true;
        }
        GUI.EndGroup();

        //generate elements
        GUI.Label(new Rect(0, 0 + heightCoef, 80, 20), "Rooms");
        int i = 0;
        foreach (var item1 in gameManager.AllRooms())
        {
            string item = EnumConverter.ToString(item1);
            toogle[item] = GUI.Toggle(new Rect(0, i * 20 + 20 + heightCoef, 110, 20), toogle[item], item);
            textBoxes[item] = GUI.TextField(new Rect(120, i * 20 + 20 + heightCoef, textBoxWidth, 20), textBoxes[item]);
            i++;
        }
        GUI.Label(new Rect(0, i * 20 + 20 + heightCoef, 80, 20), "Persons");
        foreach (var item1 in gameManager.AllCharacters())
        {
            string item = EnumConverter.ToString(item1);
            toogle[item] = GUI.Toggle(new Rect(0, i * 20 + 40 + heightCoef, 110, 20), toogle[item], item);
            textBoxes[item] = GUI.TextField(new Rect(120, i * 20 + 40 + heightCoef, textBoxWidth, 20), textBoxes[item]);
            i++;
        }
        GUI.Label(new Rect(0, i * 20 + 40 + heightCoef, 80, 20), "Weapons");
        foreach (var item1 in gameManager.AllWeapons())
        {
            string item = EnumConverter.ToString(item1);
            toogle[item] = GUI.Toggle(new Rect(0, i * 20 + 60 + heightCoef, 110, 20), toogle[item], item);
            textBoxes[item] = GUI.TextField(new Rect(120, i * 20 + 60 + heightCoef, textBoxWidth, 20), textBoxes[item]);
            i++;
        }
        if (gameManager.OnTurn() == playerNum)
        {
            if (GUI.Button(new Rect(60, i * 21 + 60 + heightCoef, 100, 30), "Ask!"))
            {
                askDialogShow = true;
            }

            if (GUI.Button(new Rect(60, i * 21 + 90 + heightCoef, 100, 30), "Master Ask!"))
            {
                questionAsk = true;
            }
        }
        GUI.EndScrollView();

    }

    private void InfoBox()
    {
        BeginAskDialogBox();

        GUI.Label(new Rect(widthAskDialog / 2 - 60, heightAskDialog / 2, 200, 50), infoBoxLabel);
        if (GUI.Button(new Rect(155, stepH * 21, 130, 30), "Close"))
        {
            infoBox = false;
        }

        GUI.EndGroup();
    }
    #endregion

    #region Ask Dialog Elements
    private void AskDialog_ChoosingCards(bool question = false)
    {
        Rooms whereAmI = board.WhereAmI(playerNum);

        if (whereAmI != Rooms.Hallway)
        {
            BeginAskDialogBox();
            {
                GUI.DrawTexture(firstCard, cardTextures[(int)whereAmI]);
                if (cardCharacter != 0)
                {
                    GUI.DrawTexture(secondCard, cardTextures[cardCharacter]);
                    askButtonText = askButtonText == "Must choose character!" ? "Ask!" : askButtonText;
                }
                if (cardWeapon != 0)
                {
                    GUI.DrawTexture(thirdCard, cardTextures[cardWeapon]);
                    askButtonText = askButtonText == "Must choose weapon!" ? "Ask!" : askButtonText;
                }

                if (Popup.List<Characters>(new Rect(165, stepH * 3, 115, 30), ref boolCh, ref choseCh, new GUIContent(character), gameManager.AllCharacters(), this.GetPopupListStyle()))
                {
                    cardCharacter = choseCh;
                    character = EnumConverter.ToString(choseCh);
                }
                if (Popup.List<Weapons>(new Rect(315, stepH * 3, 115, 30), ref boolWe, ref choseWe, new GUIContent(weapon), gameManager.AllWeapons(), this.GetPopupListStyle()))
                {
                    cardWeapon = choseWe;
                    weapon = EnumConverter.ToString(choseWe);
                }
                if (!question)
                {
                    if (GUI.Button(new Rect(155, stepH * 21, 130, 30), askButtonText))
                    {
                        if (cardCharacter == 0)
                        {
                            askButtonText = "Must choose character!";
                        }
                        else
                            if (cardWeapon == 0)
                            {
                                askButtonText = "Must choose weapon!";
                            }
                            else
                            {
                                askDialogShow = false;
                                setSomeoneAskingRPC(false); // ???
                                setQuestionRPC((int)whereAmI, cardCharacter, cardWeapon);
                                setAskingRPC(true, playerNum);
                                increaseNumberOfProcessedPlayersRPC();

                            }
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(60, stepH * 21, 130, 30), "Ask!"))
                    {
                        Triple<int, int, int> questionCards = new Triple<int, int, int>((int)whereAmI, cardCharacter, cardWeapon);
                        if (gameManager.CheckSolution(questionCards))
                        {
                            EndGameRPC(playerNum);
                        }
                        else
                        {
                            questionAsk = false;
                            infoBox = true;
                            infoBoxLabel = "You are wrong!";
                        }
                    }

                    if (GUI.Button(new Rect(200, stepH * 21, 130, 30), "Cancel"))
                    {
                        questionAsk = false;
                    }
                }
            }
            GUI.EndGroup();
        }

    }

    private void AskDialog_SomeoneAsking()
    {
        GUI.Label(new Rect(0, 0, 150, 100), "Someone other is playing now!");
    }

    private void AskDialog_ChooseCardsToShow()
    {
        // Dimensions of dialog box
        BeginAskDialogBox();
        {
            //GUIStyle style = new GUIStyle();
            //style.stretchHeight = style.stretchWidth = true;
            //style.border.left = style.border.right = style.border.bottom = style.border.top = 0;
            if (gameManager.PlayerHasCard(playerNum, (int)askedFor.First))
            {
                if (GUI.Button(firstCard, cardTextures[(int)askedFor.First]))
                {
                    doneChoosing(1);
                }
            }
            else
            {
                GUI.DrawTexture(firstCard, cardTextures[(int)askedFor.First]);
            }
            if (gameManager.PlayerHasCard(playerNum, (int)askedFor.Second))
            {
                if (GUI.Button(secondCard, cardTextures[(int)askedFor.Second]))
                {
                    doneChoosing(2);
                }

            }
            else
            {
                GUI.DrawTexture(secondCard, cardTextures[(int)askedFor.Second]);
            }
            if (gameManager.PlayerHasCard(playerNum, (int)askedFor.Third))
            {
                if (GUI.Button(thirdCard, cardTextures[(int)askedFor.Third]))
                {
                    doneChoosing(3);
                }
            }
            else
            {
                GUI.DrawTexture(thirdCard, cardTextures[(int)askedFor.Third]);
            }
        }
        GUI.EndGroup();
    }

    private void AskDialog_ShowCardsToPlayers(bool me = false)
    {
        BeginAskDialogBox();
        {
            GUI.DrawTexture(firstCard, me || playersWhoHaveCards.First == playerNum ? cardTextures[(int)askedFor.First] : backOfCard);
            GUI.DrawTexture(secondCard, me || playersWhoHaveCards.Second == playerNum ? cardTextures[(int)askedFor.Second] : backOfCard);
            GUI.DrawTexture(thirdCard, me || playersWhoHaveCards.Third == playerNum ? cardTextures[(int)askedFor.Third] : backOfCard);

            GUI.Label(firstCardLabel, playersWhoHaveCards.First != -1 ? "Player" + playersWhoHaveCards.First + " showed card!" : "None of other players show card!");
            GUI.Label(secondCardLabel, playersWhoHaveCards.Second != -1 ? "Player" + playersWhoHaveCards.Second + " showed card!" : "None of other players show card!");
            GUI.Label(thirdCardLabel, playersWhoHaveCards.Third != -1 ? "Player" + playersWhoHaveCards.Third + " showed card!" : "None of other players show card!");

            if (GUI.Button(new Rect(155, stepH * 21, 130, 30), "Ok.Close window."))
            {
                numberOfProcessedPlayers = 0;
                ResetGUIVariables();
                //networkView.RPC("ResetGUIVariables", RPCMode.AllBuffered);
               // if (me)
               // {
                    // #TODO: Logic for ending move!
                    //if (true)
                    //{
                    //    Use Singleton feature, dont need to use find. Or simple use gameManager field of this script :)
                    //    var gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
                    //    var onTurn = gameManager.OnTurn();
                    //    var numOfPlayers = gameManager.NumOfPlayers();
                    //    gameManager.SetTurn((onTurn + 1) % numOfPlayers);
                    //    gameManager.SetDicesSum(GameManager.INVALID_DICES_SUM);
                    //}
               // }
                
            }
        }
        GUI.EndGroup();
    }

    private void AskDialog_ShowQuestionCards()
    {
        BeginAskDialogBox();
        {
            GUI.DrawTexture(firstCard, cardTextures[(int)askedFor.First]);
            GUI.DrawTexture(secondCard, cardTextures[(int)askedFor.Second]);
            GUI.DrawTexture(thirdCard, cardTextures[(int)askedFor.Third] );

            GUI.Label(firstCardLabel, "Room");
            GUI.Label(secondCardLabel, "Character");
            GUI.Label(thirdCardLabel, "Weapon");
            if (playerNum == gameManager.PlayerWhoHasCard((int)askedFor.First)
            || playerNum == gameManager.PlayerWhoHasCard((int)askedFor.Second)
            || playerNum == gameManager.PlayerWhoHasCard((int)askedFor.Third))
            {
                if (GUI.Button(new Rect(155, stepH * 21, 130, 30), "Show Cards!"))
                {
                    askDialogShowCardsBool = true;
                    askDialogShowQuestion = false;
                }
            }
            else
                if (GUI.Button(new Rect(155, stepH * 21, 130, 30), "Close dialog!"))
                {
                    askDialogShowQuestion = false;
                    increaseNumberOfProcessedPlayersRPC();
                }

        }
        GUI.EndGroup();
    }

    #endregion

    #region setters
    public void setPlayerNum(int num)
    {
        playerNum = num;
    }

    void setAskingRPC(bool value, int player)
    {
        networkView.RPC("setAsking", RPCMode.AllBuffered, value, player);
    }
    void setQuestionRPC(int room, int character, int weapon)
    {
        networkView.RPC("setQuestion", RPCMode.AllBuffered, room, character, weapon);
    }
    void setSomeoneAskingRPC(bool value)
    {
        networkView.RPC("setSomeoneAsking", RPCMode.AllBuffered, value);
    }
    void setPlayerHasCardRPC(int playerNum, int field)
    {
        networkView.RPC("setPlayerHasCard", RPCMode.AllBuffered, playerNum, field);
    }

    void increaseNumberOfProcessedPlayersRPC()
    {
        networkView.RPC("increaseNumberOfProcessedPlayers", RPCMode.AllBuffered);
    }

    void EndGameRPC(int playerNum)
    {
        networkView.RPC("EndGame", RPCMode.AllBuffered, playerNum);
    }
    #endregion

    #region RPC
    [RPC]
    void setSomeoneAsking(bool value)
    {
        if (!askDialogShow)
            someoneAsking = value;
    }
    [RPC]
    void setAsking(bool value, int player)
    {
        asking = value;
        playerAsking = player;
        askDialogShowQuestion = true;
    }
    [RPC]
    void setQuestion(int room, int character, int weapon)
    {
        askedFor.First = (Rooms)room;
        askedFor.Second = (Characters)character;
        askedFor.Third = (Weapons)weapon;
    }
    [RPC]
    void setPlayerHasCard(int playerNum, int field)
    {
        if (field == 1)
            playersWhoHaveCards.First = playerNum;
        if (field == 2)
            playersWhoHaveCards.Second = playerNum;
        if (field == 3)
            playersWhoHaveCards.Third = playerNum;
    }
    [RPC]
    void increaseNumberOfProcessedPlayers()
    {
        ++numberOfProcessedPlayers;
    }
    [RPC]
    void ResetGUIVariables()
    {
        boolCh = boolWe = false;
        cardCharacter = cardWeapon = choseWe = choseCh = 0;
        weapon = "Choose weapon";
        character = "Choose character";
        askButtonText = "Ask!";
        askDialogShow = someoneAsking = guardAsking = askDialogShowCardsBool = askDialogShowQuestion = questionAsk = false;
        playersWhoHaveCards = new Triple<int, int, int>(-1, -1, -1);
        askedFor = new Triple<Rooms, Characters, Weapons>(0, 0, 0);
        asking = false;
        playerAsking = -1;
        numberOfProcessedPlayers = 0;
    }
    [RPC]
    void EndGame(int playerWon)
    {
        ResetGUIVariables();
        endGameInfo = true;
        infoBox = true;
        infoBoxLabel = "Player " + playerWon + " wins! Congrats!";

        //endgame logic for other components
    }
    #endregion

    #region HelperMethods

    // Determine percentage of given value, primary for percentage of screen
    int Percentage(int value, int percent)
    {
        return (int)((double)value * ((double)percent / 100));
    }
    GUIStyle GetPopupListStyle()
    {
        GUIStyle listStyle = new GUIStyle();
        listStyle.normal.textColor = Color.white;
        listStyle.onHover.background =
        listStyle.hover.background = new Texture2D(2, 2);
        listStyle.padding.left =
        listStyle.padding.right =
        listStyle.padding.top =
        listStyle.padding.bottom = 4;
        return listStyle;
    }

    void InitAskDialogPositionVariables()
    {
        
        widthAskDialog = 450;//Percentage(Screen.width, 50);
        heightAskDialog = 250;//Percentage(Screen.height, 75);
        boxForChosingCards = new Rect((Percentage(Screen.width, 75)) / 2 - 220, Screen.height / 2 - 125, widthAskDialog, heightAskDialog);
        stepW = widthAskDialog / 21;
        stepH = heightAskDialog / 24;

        firstCard =     new Rect( 30, 7 * stepH, 90, 13 * stepH);
        secondCard =    new Rect(180, 7 * stepH, 90, 13 * stepH);
        thirdCard =     new Rect(330, 7 * stepH, 90, 13 * stepH);

        firstCardLabel =     new Rect( 30, stepH * 3, 90, 20);
        secondCardLabel =    new Rect(180, stepH * 3, 90, 20);
        thirdCardLabel =     new Rect(330, stepH * 3, 90, 20);

    }

    void setCards(int num)
    {
        foreach (var item in gameManager.PlayerCards(num))
        {
            toogle[EnumConverter.ToString((int)item)] = true;
        }
    }

    void doneChoosing(int choice)
    {
        setPlayerHasCardRPC(playerNum, choice);
        increaseNumberOfProcessedPlayersRPC();
        asking = false;
    }

    void BeginAskDialogBox()
    {
        GUIStyle style = new GUIStyle(GUI.skin.box);
        GUI.BeginGroup(boxForChosingCards, style);
    }

    private void ThrowDices()
    {
        if (!dicesThrown)
        {
            num1 = Random.Range(1, 6);
            num2 = Random.Range(1, 6);
            dicesThrown = true;
        }

    }
    #endregion
}
