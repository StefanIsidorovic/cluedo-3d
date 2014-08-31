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
    private bool asking = false;
    private int playerAsking = -1;
    #endregion

    private Triple<int, int, int> playersWhoHaveCards;

    private int numberOfProcessedPlayers = 0;

    // #TODO: Refactor code.
    // #NOTE: Move rect and help variables from functions;
    void Start()
    {
        backOfCard = (Texture2D)Resources.Load("BackOfCard", typeof(Texture2D));
        playersWhoHaveCards = new Triple<int, int, int>(-1, -1, -1);
        askedFor = new Triple<Rooms, Characters, Weapons>(0, 0, 0);
        gameManager = MonoSingleton<GameManager>.Instance;
        board = MonoSingleton<BoardScript>.Instance;
        heightCoef = Percentage(Screen.height, 30);
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
       // test =(Texture2D) Resources.Load("Cards/message", typeof(Texture2D));
        dieFacesVector = new List<Texture2D>();
        dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/images", typeof(Texture2D)));
        dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/1", typeof(Texture2D)));
        dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/2", typeof(Texture2D)));
        dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/3", typeof(Texture2D)));
        dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/4", typeof(Texture2D)));
        dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/5", typeof(Texture2D)));
        dieFacesVector.Add((Texture2D)Resources.Load("dieFaces/6", typeof(Texture2D)));
    }

    // GUI elements

    void OnGUI()
    {
        InitAskDialogPositionVariables();
        // Showing initial dialog for choosing cards to form a question, and setting variable someoneasking to true, so other player 
        // know when somebody else is forming a question.
        if (askDialogShow)
        {
            AskDialog_ChoosingCards();
            setSomeoneAskingRPC(true);
        }
        else
        {
            if (someoneAsking)
            {
                AskDialog_SomeoneAsking();
            }
        }
        // If some other player than me asked a question and I have one of cards he requested
        if (asking && playerAsking != playerNum &&
            (playerNum == gameManager.PlayerWhoHasCard((int)askedFor.First)
            || playerNum == gameManager.PlayerWhoHasCard((int)askedFor.Second)
            || playerNum == gameManager.PlayerWhoHasCard((int)askedFor.Third)))
        {
            guardAsking = true;
            AskDialog_ChooseCardsToShow();
        }
        // If I dont have any cards to show. Also for increment counter of processed players for "Asker" also.
        if (asking && !guardAsking)
        {
            guardAsking = true;
            increaseNumberOfProcessedPlayersRPC();
        }
        if (GameObject.Find("NetworkManager").gameObject.GetComponent<NetworkManager>().GameStarted())
        {
            setCards(playerNum);
            ShowSideBar();
        }
        if (numberOfProcessedPlayers == gameManager.NumOfPlayers() && GameObject.Find("NetworkManager").gameObject.GetComponent<NetworkManager>().GameStarted())
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
            new Rect(0, 0, Percentage(Screen.width, 25) - 25, 21 * 20 + 60 + heightCoef + 10)
        );

        GUI.Box(new Rect(0, 0, Percentage(Screen.width, 25) - 25, 21 * 20 + 60 + heightCoef + 10), "BLAH");
        
        // Generate 2d part for throwing dices - everything about this part is within a group.
 
        GUI.BeginGroup(new Rect(0, 0, Percentage(Screen.width, 25) - 25, heightCoef - 10));

        //two rect for presenting the dices and centering them 
        GUI.Box(new Rect((Percentage(Screen.width, 25) - 25)/2 - 60, 30, 40, 40), dieFacesVector[num1]);
        GUI.Box(new Rect((Percentage(Screen.width, 25) - 25)/2 + 20, 30, 40, 40), dieFacesVector[num2]);
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
            if (GUI.Button(new Rect((Percentage(Screen.width, 25) - 25)/2 - 50, 100, 100, 50), "Throw dices!"))
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
            if (GUI.Button(new Rect(60, i * 21 + 60 + heightCoef, 100, 30), "Ask!"))
            {
                askDialogShow = true;
            }
        GUI.EndScrollView();

    }
    #endregion

    #region Ask Dialog Elements
    private void AskDialog_ChoosingCards()
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

                if (Popup.List<Characters>(new Rect(stepW * 7, stepH * 6, 100, 30), ref boolCh, ref choseCh, new GUIContent(character), gameManager.AllCharacters(), this.GetPopupListStyle()))
                {
                    cardCharacter = choseCh;
                    character = EnumConverter.ToString(choseCh);
                }
                if (Popup.List<Weapons>(new Rect(stepW * 13, stepH * 6, 100, 30), ref boolWe, ref choseWe, new GUIContent(weapon), gameManager.AllWeapons(), this.GetPopupListStyle()))
                {
                    cardWeapon = choseWe;
                    weapon = EnumConverter.ToString(choseWe);
                }

                if (GUI.Button(new Rect(stepW * 5, stepH * 21, stepW * 8, stepH * 2), askButtonText))
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
            GUIStyle style = new GUIStyle();
            style.fixedHeight = style.fixedWidth = 0;
            style.stretchHeight = style.stretchWidth = true;
            style.border.left = style.border.right = style.border.bottom = style.border.top = 0;
            if (gameManager.PlayerHasCard(playerNum, (int)askedFor.First))
            {
                if (GUI.Button(firstCard, cardTextures[(int)askedFor.First], style))
                {
                    doneChoosing(1);
                }
            }
            if (gameManager.PlayerHasCard(playerNum, (int)askedFor.Second))
            {
                if (GUI.Button(secondCard, cardTextures[(int)askedFor.Second], style))
                {
                    doneChoosing(2);
                }

            }
            if (gameManager.PlayerHasCard(playerNum, (int)askedFor.Third))
            {
                if (GUI.Button(thirdCard, cardTextures[(int)askedFor.Third], style))
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
            GUI.DrawTexture(firstCard, me || playersWhoHaveCards.First == playerNum ? cardTextures[(int)askedFor.First] : backOfCard);
            GUI.DrawTexture(secondCard, me || playersWhoHaveCards.Second == playerNum ? cardTextures[(int)askedFor.Second] : backOfCard);
            GUI.DrawTexture(thirdCard, me || playersWhoHaveCards.Third == playerNum ? cardTextures[(int)askedFor.Third] : backOfCard);

            GUI.Label(firstCardLabel, playersWhoHaveCards.First != -1 ? "Player" + playersWhoHaveCards.First + " showed card!" : "None of other players show card!");
            GUI.Label(secondCardLabel, playersWhoHaveCards.Second != -1 ? "Player" + playersWhoHaveCards.Second + " showed card!" : "None of other players show card!");
            GUI.Label(thirdCardLabel, playersWhoHaveCards.Third != -1 ? "Player" + playersWhoHaveCards.Third + " showed card!" : "None of other players show card!");

            if (GUI.Button(new Rect(stepW * 5, stepH * 21, stepW * 8, stepH * 2), "Ok. Close window."))
            {
                numberOfProcessedPlayers = 0;
                networkView.RPC("ResetGUIVariables", RPCMode.AllBuffered);
                if (me)
                {
                    // #TODO: Logic for ending move!
                }
                
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
        askDialogShow = someoneAsking = guardAsking = false;
        playersWhoHaveCards = new Triple<int, int, int>(-1, -1, -1);
        askedFor = new Triple<Rooms, Characters, Weapons>(0, 0, 0);
        asking = false;
        playerAsking = -1;
        numberOfProcessedPlayers = 0;
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
        widthAskDialog = Percentage(Screen.width, 50);
        heightAskDialog = Percentage(Screen.height, 75);
        stepW = widthAskDialog / 21;
        stepH = heightAskDialog / 24;

        firstCard = new Rect(stepW, 8 * stepH, 5 * stepW, 12 * stepH);
        secondCard = new Rect(stepW * 7, 8 * stepH, 5 * stepW, 12 * stepH);
        thirdCard = new Rect(stepW * 13, 8 * stepH, 5 * stepW, 12 * stepH);

        firstCardLabel = new Rect(stepW, 7 * stepH, 5 * stepW, 20);
        secondCardLabel = new Rect(stepW * 7, 7 * stepH, 5 * stepW, 20);
        thirdCardLabel = new Rect(stepW * 13, 7 * stepH, 5 * stepW, 20);

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
        GUI.BeginGroup(new Rect(Percentage(Screen.width, 5), Percentage(Screen.height, 15), widthAskDialog, heightAskDialog));
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
