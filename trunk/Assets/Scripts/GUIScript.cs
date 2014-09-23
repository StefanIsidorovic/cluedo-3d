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
    private GUIStyle sideBarStyle;
    private GUIStyle labelSideBarStyle;
    // helper variebles for defining Rectangles for positions of elements.
    // Height of space intended for dices.
    private int heightCoef = 0;
    // width of textbox.
    private int textBoxWidth = 140;
    private int leftMarginSideBar = 10;
    private bool finishedAsking = false;
    #endregion

    #region Variables for dices box
    public List<Texture2D> dieFacesVector;
    public bool dicesThrown = false;
    public int num1 = 0;
    public int num2 = 0;
    private bool showDicesBox = false;
    private int onTurn;
    private int numOfMoves;
    private GUIStyle dicesBoxStyle;
    private GUIStyle labelDicesStyle;
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
    private Rect firstCard, secondCard, thirdCard, firstCardLabel, secondCardLabel, thirdCardLabel, twoOneCard, twoTwoCard, twoOneCardLabel, twoTwoCardLabel;
    private int stepW, stepH, widthAskDialog, heightAskDialog;
    private bool asking = false, askDialogShowCardsBool = false, askDialogShowQuestion = false;
    private int playerAsking = -1;
    private Rect boxForChosingCards;
    #endregion

    private bool setCardGuard = false;
    private bool questionAsk = false;
    private bool infoBox = false;
    private string infoBoxLabel = "";
    private string textMessageForAllPlayers;

    private Triple<int, int, int> playersWhoHaveCards;
    private bool questionToogle = false;
    private int numberOfProcessedPlayers = 0;

    private Triple<int, int, int> solutions;
    private int WhoWon;

    string PublicPlayerName;

    private bool endGameInfo = false;
    // #TODO: Refactor code.
    // #NOTE: Move rect and help variables from functions;
    void Start()
    {
        solutions = new Triple<int, int, int>(-1, -1, -1);
        onTurn = 0;
        textMessageForAllPlayers = "";
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
            {
                Texture2D temp = (Texture2D)Resources.Load("Cards/" + strItem, typeof(Texture2D));
                cardTextures.Add((int)item, temp);
            }
        }
        foreach (var item in gameManager.AllWeapons())
        {
            string strItem = EnumConverter.ToString(item);
            toogle.Add(strItem, false);
            textBoxes.Add(strItem, "");
            Texture2D temp = (Texture2D)Resources.Load("Cards/" + strItem, typeof(Texture2D));
            cardTextures.Add((int)item, temp);
        }
        foreach (var item in gameManager.AllCharacters())
        {
            string strItem = EnumConverter.ToString(item);
            toogle.Add(strItem, false);
            textBoxes.Add(strItem, "");
            Texture2D temp = (Texture2D)Resources.Load("Cards/" + strItem, typeof(Texture2D));
            cardTextures.Add((int)item, temp);
        }
        dieFacesVector = new List<Texture2D>();
        dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/images", typeof(Texture2D)));
        for (int i = 1; i < 7; i++)
            dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/" + i, typeof(Texture2D)));
    }

    // GUI elements

    void OnGUI()
    {
        if (infoBox)
        {
            InfoBox();
        }

        if (endGameInfo)
        {
            EndGameDialog();
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
        if (askDialogShowQuestion && playerAsking != playerNum)
        {
            AskDialog_ShowQuestionCards();
        }
        // Dialog for choosing card to show.
        if (askDialogShowCardsBool)
        {
            AskDialog_ChooseCardsToShow();
        }

        if (numberOfProcessedPlayers == gameManager.NumOfPlayers() && GameObject.Find("NetworkManager").gameObject.GetComponent<NetworkManager>().GameStarted())
        {
            AskDialog_ShowCardsToPlayers(playerAsking == playerNum);
        }
    }

    #region Rest Of GUI
    private void ShowSideBar()
    {
        // Generate 2d part for throwing dices 
        onTurn = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>().OnTurn();
        //style
        sideBarStyle = new GUIStyle(GUI.skin.box);
        //sideBarStyle.normal.background = (Texture2D)Resources.Load("house", typeof(Texture2D));

        // label with a message to all players
        GUI.Box(new Rect(0, 25, 240, Screen.height), "", sideBarStyle);
        GUI.Label(new Rect(10, 25, 230, Screen.height - 150), TextMessageForAllPlayers());
        GUIStyle leaveButtonStyle = new GUIStyle(GUI.skin.button);
        leaveButtonStyle.normal.background = (Texture2D)Resources.Load("leave", typeof(Texture2D));
        leaveButtonStyle.active.background = leaveButtonStyle.normal.background;
        leaveButtonStyle.hover.background = leaveButtonStyle.normal.background;

        if (GUI.Button(new Rect(10, Screen.height - 100, 50, 100), "", leaveButtonStyle))
        {
            Network.CloseConnection(Network.connections[0], true);
        }

        GUIStyle leaveLabelStyle = new GUIStyle(GUI.skin.label);
        leaveButtonStyle.fontSize = 20;
        leaveButtonStyle.normal.textColor = Color.gray;
        leaveButtonStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(80, Screen.height - 70, 200, 50), "<size=20>Leave Game</size>", leaveLabelStyle);


        GameObject myPlayer = GameObject.Find("Player" + playerNum);

        if (myPlayer != null)
        {
            if (playerNum == gameManager.OnTurn())
            {
                GenerateMessageForAllPlayers();
                numOfMoves = myPlayer.gameObject.GetComponent<CharacterControl>().NumOfMoves();
                if (numOfMoves == 0 && !dicesThrown)
                    showDicesBox = true;
                drowDices();
            }
        }
        //SideBar
        sideBarStyle = new GUIStyle(GUI.skin.box);
        //sideBarStyle.normal.background = (Texture2D)Resources.Load("house", typeof(Texture2D));

        labelSideBarStyle = new GUIStyle(GUI.skin.label);
        labelSideBarStyle.fontSize = 18;
        labelSideBarStyle.fontStyle = FontStyle.Bold;

        scrollPosition = GUI.BeginScrollView(
            new Rect(Screen.width - 300, 25, 300, Screen.height),
            scrollPosition,
            new Rect(0, 0, 300, 21 * 20 + 60 + heightCoef + 80)
        );

        GUI.Box(new Rect(0, 0, 300, 21 * 20 + 60 + heightCoef + 80), "", sideBarStyle);

        //generate checkboxes on sideBar 
        GUI.Label(new Rect(leftMarginSideBar, 20, 80, 30), "Rooms", labelSideBarStyle);
        int i = 0;
        foreach (var item1 in gameManager.AllRooms())
        {
            if (item1 != Rooms.Hallway)
            {
                string item = EnumConverter.ToString(item1);
                toogle[item] = GUI.Toggle(new Rect(leftMarginSideBar, i * 22 + 60, 110, 20), gameManager.PlayerHasCard(playerNum, (int)EnumConverter.ToEnum(item)) ? true : toogle[item], item);
                textBoxes[item] = GUI.TextField(new Rect(130, i * 22 + 60, textBoxWidth, 20), textBoxes[item]);
                i++;
            }
        }
        GUI.Label(new Rect(leftMarginSideBar, i * 22 + 60, 80, 30), "Persons", labelSideBarStyle);
        foreach (var item1 in gameManager.AllCharacters())
        {
            string item = EnumConverter.ToString(item1);
            toogle[item] = GUI.Toggle(new Rect(leftMarginSideBar, i * 22 + 60 + 35, 110, 20), gameManager.PlayerHasCard(playerNum, (int)EnumConverter.ToEnum(item)) ? true : toogle[item], item);
            textBoxes[item] = GUI.TextField(new Rect(130, i * 22 + 60 + 35, textBoxWidth, 20), textBoxes[item]);
            i++;
        }
        GUI.Label(new Rect(leftMarginSideBar, i * 22 + 110, 80, 30), "Weapons", labelSideBarStyle);
        foreach (var item1 in gameManager.AllWeapons())
        {
            string item = EnumConverter.ToString(item1);
            toogle[item] = GUI.Toggle(new Rect(leftMarginSideBar, i * 22 + 140, 110, 20), gameManager.PlayerHasCard(playerNum, (int)EnumConverter.ToEnum(item)) ? true : toogle[item], item);
            textBoxes[item] = GUI.TextField(new Rect(130, i * 22 + 140, textBoxWidth, 20), textBoxes[item]);
            i++;
        }
        if (gameManager.OnTurn() == playerNum)
        {
            if (board.WhereAmI(playerNum) == Rooms.Hallway)
                GUI.enabled = false;
            if (!finishedAsking)
            {
                if (GUI.Button(new Rect(30, i * 22 + 160, 100, 30), "Ask!"))
                {
                    askDialogShow = true;


                }
            }

            if (GUI.Button(new Rect(160, i * 22 + 160, 100, 30), "Master Ask!"))
            {
                questionAsk = true;
            }
            if (board.WhereAmI(playerNum) == Rooms.Hallway)
                GUI.enabled = true;
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
            gameManager.SetQuestionIsAsked(true);
            ResetGUIVariables();
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
                                finishedAsking = true;

                            }
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(80, stepH * 21, 130, 30), "Ask!"))
                    {
                        Triple<int, int, int> questionCards = new Triple<int, int, int>((int)whereAmI, cardCharacter, cardWeapon);
                        if (gameManager.CheckSolution(questionCards))
                        {
                            solutions = questionCards;
                            EndGameRPC(playerNum, questionCards.First, questionCards.Second, questionCards.Third);

                        }
                        else
                        {
                            questionAsk = false;
                            infoBox = true;
                            infoBoxLabel = "You are wrong!";
                        }
                    }

                    if (GUI.Button(new Rect(240, stepH * 21, 130, 30), "Cancel"))
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
            if (gameManager.PlayerHasCard(playerNum, (int)askedFor.First))
            {
                if (GUI.Button(firstCard, cardTextures[(int)askedFor.First]))
                {
                    doneChoosing(1);
                }
            }
            if (gameManager.PlayerHasCard(playerNum, (int)askedFor.Second))
            {
                if (GUI.Button(secondCard, cardTextures[(int)askedFor.Second]))
                {
                    doneChoosing(2);
                }

            }
            if (gameManager.PlayerHasCard(playerNum, (int)askedFor.Third))
            {
                if (GUI.Button(thirdCard, cardTextures[(int)askedFor.Third]))
                {
                    doneChoosing(3);
                }
            }

        }
        GUI.EndGroup();
    }

    private void AskDialog_ShowCardsToPlayers(bool me = false)
    {
        BeginAskDialogBox();
        {

            int numOfShowedCards = 0;
            if ((int)playersWhoHaveCards.First != -1) numOfShowedCards++;
            if ((int)playersWhoHaveCards.Second != -1) numOfShowedCards++;
            if ((int)playersWhoHaveCards.Third != -1) numOfShowedCards++;

            bool trigger = false;
            string player1 = "", player2 = "", player3 = "";
            if (playersWhoHaveCards.First != -1)
            {
                player1 = GameObject.Find("Player" + playersWhoHaveCards.First).gameObject.GetComponent<CharacterControl>().PublicName();
                Rect temp = new Rect(0, 0, 0, 0);
                Rect tempLabel = new Rect(0, 0, 0, 0);
                if (numOfShowedCards == 1)
                {
                    temp = secondCard;
                    tempLabel = secondCardLabel;
                }
                if (numOfShowedCards == 2)
                {
                    temp = twoOneCard;
                    tempLabel = twoOneCardLabel;
                    trigger = true;
                }
                if (numOfShowedCards == 3)
                {
                    temp = firstCard;
                    tempLabel = firstCardLabel;
                }

                GUI.DrawTexture(temp, me || playersWhoHaveCards.First == playerNum ? cardTextures[(int)askedFor.First] : backOfCard);
                GUI.Label(tempLabel, playersWhoHaveCards.First != -1 ? player1 + " showed card!" : "None of other players show card!");
                if (string.IsNullOrEmpty(player1))
                    player1 = "Player" + playersWhoHaveCards.First;
            }

            if (playersWhoHaveCards.Second != -1)
            {
                player2 = GameObject.Find("Player" + playersWhoHaveCards.Second).gameObject.GetComponent<CharacterControl>().PublicName();
                Rect temp = new Rect(0, 0, 0, 0);
                Rect tempLabel = new Rect(0, 0, 0, 0);
                if (numOfShowedCards == 1)
                {
                    temp = secondCard;
                    tempLabel = secondCardLabel;
                }
                if (numOfShowedCards == 2)
                {
                    if (!trigger)
                    {
                        temp = twoOneCard;
                        tempLabel = twoOneCardLabel;
                        trigger = true;
                    }
                    else
                    {
                        temp = twoTwoCard;
                        tempLabel = twoTwoCardLabel;
                    }
                }
                if (numOfShowedCards == 3)
                {
                    temp = secondCard;
                    tempLabel = secondCardLabel;
                }
                GUI.Label(tempLabel, playersWhoHaveCards.Second != -1 ? player2 + " showed card!" : "None of other players show card!");
                GUI.DrawTexture(temp, me || playersWhoHaveCards.Second == playerNum ? cardTextures[(int)askedFor.Second] : backOfCard);
                if (string.IsNullOrEmpty(player2))
                    player2 = "Player" + playersWhoHaveCards.Second;
            }

            if (playersWhoHaveCards.Third != -1)
            {
                player3 = GameObject.Find("Player" + playersWhoHaveCards.Third).gameObject.GetComponent<CharacterControl>().PublicName();
                Rect temp = new Rect(0, 0, 0, 0);
                Rect tempLabel = new Rect(0, 0, 0, 0);
                if (numOfShowedCards == 1)
                {
                    temp = secondCard;
                    tempLabel = secondCardLabel;
                }
                if (numOfShowedCards == 2)
                {
                    temp = twoTwoCard;
                    tempLabel = twoTwoCardLabel;
                }
                if (numOfShowedCards == 3)
                {
                    temp = thirdCard;
                    tempLabel = thirdCardLabel;
                }
                GUI.Label(tempLabel, playersWhoHaveCards.Third != -1 ? player3 + " showed card!" : "None of other players show card!");
                GUI.DrawTexture(temp, me || playersWhoHaveCards.Third == playerNum ? cardTextures[(int)askedFor.Third] : backOfCard);
                if (string.IsNullOrEmpty(player3))
                    player3 = "Player" + playersWhoHaveCards.Third;
            }


            if (GUI.Button(new Rect(155, stepH * 21, 130, 30), "Ok.Close window."))
            {
                numberOfProcessedPlayers = 0;
                ResetGUIVariables();

                if (me)
                    gameManager.SetQuestionIsAsked(true);
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
            GUI.DrawTexture(thirdCard, cardTextures[(int)askedFor.Third]);

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

    private void EndGameDialog()
    {
        BeginAskDialogBox();
        {
            PublicPlayerName = GameObject.Find("Player" + WhoWon).gameObject.GetComponent<CharacterControl>().PublicName();

            GUI.DrawTexture(firstCard, cardTextures[(int)solutions.First]);
            GUI.DrawTexture(secondCard, cardTextures[(int)solutions.Second]);
            GUI.DrawTexture(thirdCard, cardTextures[(int)solutions.Third]);

            if (GUI.Button(new Rect(155, stepH * 21, 130, 30), "EXIT!"))
            {
                // #TODO: end game
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

    void EndGameRPC(int playerNum, int room, int character, int weapon)
    {
        networkView.RPC("EndGame", RPCMode.AllBuffered, playerNum, room, character, weapon);
    }

    public string TextMessageForAllPlayers()
    {
        return textMessageForAllPlayers;
    }
    public void SetTextMessageForAllPlayers(string text)
    {
        networkView.RPC("SetTextMessageForAllPlayersRPC", RPCMode.AllBuffered, text);
    }

    public void UpdatePlayerNumber()
    {
        networkView.RPC("UpdatePlayerNumberRPC", RPCMode.AllBuffered);
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
        finishedAsking = false;
        dicesThrown = false;
        num1 = 0;
        num2 = 0;
    }
    [RPC]
    void EndGame(int playerWon, int room, int character, int weapon)
    {
        ResetGUIVariables();
        endGameInfo = true;
        solutions.First = room;
        solutions.Second = character;
        solutions.Third = weapon;
        WhoWon = playerWon;

        //endgame logic for other components
    }

    [RPC]
    private void SetTextMessageForAllPlayersRPC(string text)
    {
        textMessageForAllPlayers = text;
    }

    [RPC]
    private void UpdatePlayerNumberRPC()
    {
        playerNum = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().NumOfMyPlayer();
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
        boxForChosingCards = new Rect(Screen.width / 2 - 220, Screen.height / 2 - 125, widthAskDialog, heightAskDialog);
        stepW = widthAskDialog / 21;
        stepH = heightAskDialog / 24;

        firstCard = new Rect(30, 7 * stepH, 90, 13 * stepH);
        secondCard = new Rect(180, 7 * stepH, 90, 13 * stepH);
        thirdCard = new Rect(330, 7 * stepH, 90, 13 * stepH);

        twoOneCard = new Rect(75, 7 * stepH, 90, 13 * stepH);
        twoTwoCard = new Rect(225, 7 * stepH, 90, 13 * stepH);

        twoOneCardLabel = new Rect(75, stepH * 3, 90, 20);
        twoTwoCardLabel = new Rect(225, stepH * 3, 90, 20);

        firstCardLabel = new Rect(30, stepH * 3, 90, 20);
        secondCardLabel = new Rect(180, stepH * 3, 90, 20);
        thirdCardLabel = new Rect(330, stepH * 3, 90, 20);

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
        askDialogShowCardsBool = false;
    }

    void BeginAskDialogBox()
    {
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.normal.background = (Texture2D)Resources.Load("proba2", typeof(Texture2D));
        GUI.BeginGroup(boxForChosingCards, style);
    }

    private void ThrowDices()
    {
        if (!dicesThrown)
        {
            num1 = Random.Range(1, 6);
            num2 = Random.Range(1, 6);
            dicesThrown = true;
            GUI.Box(new Rect(50, 30, 40, 40), dieFacesVector[num1]);
            GUI.Box(new Rect(110, 30, 40, 40), dieFacesVector[num2]);
            GameObject.Find("GameManager").gameObject.GetComponent<GameManager>().SetDicesSum((num1 + num2));
        }

    }

    private void drowDices()
    {
        dicesBoxStyle = new GUIStyle(GUI.skin.box);
        labelDicesStyle = new GUIStyle(GUI.skin.label);
        labelDicesStyle.fontStyle = FontStyle.Bold;
        labelDicesStyle.alignment = TextAnchor.MiddleCenter;
        int numOfMovesMade = GameObject.Find("Player" + gameManager.OnTurn()).gameObject.GetComponent<CharacterControl>().NumOfMoves();
        if ((numOfMovesMade == (num1 + num2)))
        {
            dicesThrown = false;
            num2 = 0;
            num1 = 0;
        }
        if (showDicesBox)
        {
            GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200), "", dicesBoxStyle);
            GUI.BeginGroup(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));

            GUI.Label(new Rect(0, 10, 200, 30), "It's your turn!", labelDicesStyle);
            //two rect for presenting the dices and centering them 
            GUI.Box(new Rect(50, 40, 40, 40), dieFacesVector[num1]);
            GUI.Box(new Rect(110, 40, 40, 40), dieFacesVector[num2]);

            if (dicesThrown)
                GUI.enabled = false;
            if (GUI.Button(new Rect(50, 90, 100, 40), "Throw dices!"))
            {
                ThrowDices();
            }

            if ((numOfMovesMade == (num1 + num2)))
            {
                dicesThrown = false;
                showDicesBox = false;
                num2 = 0;
                num1 = 0;
            }

            if (dicesThrown)
                GUI.enabled = true;

            if (GUI.Button(new Rect(50, 150, 100, 40), "Close!") || askDialogShow == true || questionAsk == true)
            {
                showDicesBox = false;
            }
            GUI.EndGroup();
        }
    }

    private void GenerateMessageForAllPlayers()
    {
        PublicPlayerName = GameObject.Find("Player" + gameManager.OnTurn()).gameObject.GetComponent<CharacterControl>().PublicName();
        int dicesSum = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>().DicesSum();
        if (dicesSum > 0 && askDialogShow == false && !askDialogShowQuestion && !askDialogShowCardsBool && !questionAsk && !infoBox)
        {
            if (!textMessageForAllPlayers.StartsWith("It is " + PublicPlayerName + "'s turn and he/she is allowed to make " + dicesSum + " moves.") &&
                !textMessageForAllPlayers.StartsWith(PublicPlayerName + " has made a mistake with final solution and he/she has been excluded from the game."))
                SetTextMessageForAllPlayers("It is " + PublicPlayerName + "'s turn and he/she is allowed to make " +
            dicesSum + " moves.\n" + textMessageForAllPlayers);
        }
        else if (dicesSum <= 0 && askDialogShow == false && !askDialogShowQuestion && !askDialogShowCardsBool && !infoBox)
        {
            if (!textMessageForAllPlayers.StartsWith("It is " + PublicPlayerName + "'s turn, and he/she didn't throw dices yet."))
                SetTextMessageForAllPlayers("It is " + PublicPlayerName + "'s turn, and he/she didn't throw dices yet.\n" + textMessageForAllPlayers);
        }
        if (askDialogShow == true || askDialogShowQuestion || askDialogShowCardsBool)
        {
            if (!textMessageForAllPlayers.StartsWith(PublicPlayerName + " is currently in " + board.WhereAmI(playerNum) + " and wants to ask the question!")
                &&
                !textMessageForAllPlayers.StartsWith(PublicPlayerName + " asked for room - " + (Rooms)board.WhereAmI(playerNum) +
                    ", character - " + (Characters)cardCharacter + ", weapon - " + (Weapons)cardWeapon + "."))
                SetTextMessageForAllPlayers(PublicPlayerName + " is currently in " + board.WhereAmI(playerNum) + " and wants to ask the question!\n" + textMessageForAllPlayers);

            if (cardCharacter != 0 && cardWeapon != 0)
            {
                if (!textMessageForAllPlayers.StartsWith(PublicPlayerName + " asked for room - " + (Rooms)board.WhereAmI(playerNum) +
                    ", character - " + (Characters)cardCharacter + ", weapon - " + (Weapons)cardWeapon + "."))
                    SetTextMessageForAllPlayers(PublicPlayerName + " asked for room - " + (Rooms)board.WhereAmI(playerNum) +
                    ", character - " + (Characters)cardCharacter + ", weapon - " + (Weapons)cardWeapon + ".\n" + textMessageForAllPlayers);
            }

        }
        else if (questionAsk == true)
        {
            if (!textMessageForAllPlayers.StartsWith(PublicPlayerName + " is currently in " + board.WhereAmI(playerNum) + " and wants to ask for final solution!"))
                SetTextMessageForAllPlayers(PublicPlayerName + " is currently in " + board.WhereAmI(playerNum) + " and wants to ask for final solution!\n" + textMessageForAllPlayers);
        }

        if (infoBox == true && !questionAsk)
        {
            if (!textMessageForAllPlayers.StartsWith(PublicPlayerName + " has made a mistake with final solution and he/she has been excluded from the game."))
                SetTextMessageForAllPlayers(PublicPlayerName + " has made a mistake with final solution and he/she has been excluded from the game.\n" + textMessageForAllPlayers);
        }
    }
    #endregion

}