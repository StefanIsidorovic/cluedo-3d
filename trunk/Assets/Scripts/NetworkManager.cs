using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NetworkManager : MonoBehaviour
{
    #region Network data
    // Unique typename for game (Hopefully unique).
    private const string typeName = "Cluedo3DSAMOSTeam";
    private string gameName = "";
    private bool isRefreshingHostList = false;
    private HostData[] hostList;
    // Number of minimum players need to be so game could start, its for testing and debuging purposes
    // because its frustrating to start three instances everytime. By default is set to 3 but you can change it from Unity editor, NOT HERE!
    // Again, NOT HERE, in unity editor (inspector).
    public int minimumNumberOfPlayers = 2;

    // Variable for server to know how many players are there
    // you can change it to public so you can track changes in debug.
    private int numOfPlayersConnected = 0;

    // Bool triggers for game start dialog.
    private bool spawnTrigger = false;
    private bool startGameDialog = true;
    // Do not change this to public, you have below method for checking if game is started.
    private bool gameStarted = false;
    private bool serverStarted = false;

    private Dictionary<NetworkPlayer, int> mapNetworkPlayerToPlayerNum;
    #endregion

    #region GUI elements
    private bool show = false;
    private Rect makeRoomBox;
    private Rect playerNameBox;
    private Rect hostsBox;
    private Rect aboutCluedoBox;
    private GUIStyle buttonTextStyle;
    private GUIStyle labelTextStyle;
    private GUIStyle inputTextStyle;
    private GUIStyle bigLabelStyle;
    private GUIStyle boxStyle;
    private GUIStyle backgroundStyle;
    private GUIStyle backgroundStyle2;
    private GUIStyle smallLabelStyle;
    private GUIStyle refreshButtonStyle;
    private Vector2 scrollPosition = Vector2.zero;

    //private GUIStyle boxStyleBackground;
    private bool isRefreshedFirstTime = false;
    public bool showTextAboutCluedo = false;

    #endregion

    #region Player data
    private string publicPlayerName = "";
    public GameObject playerPrefab;
    public Material[] playerMaterials;
    // Variable for network manager to know which player is his, for later spawning.
    private int numOfPlayer = 0;
	  private int hosts;
    #endregion
    
    private bool guardNum = true;

    void Start()
    {
        mapNetworkPlayerToPlayerNum = new Dictionary<NetworkPlayer, int>();
    }

    void Update()
    {
        if (Network.isClient)
        {
            if (numOfPlayersConnected > 0 && guardNum)
            {
                guardNum = false;
                SetListConnectedPlayers();
            }
        }
    }
    
    void OnGUI()
    {

        // Start or join server dialog
        if (!Network.isClient && !Network.isServer)
        {
            InitGUIStyles();

            //StartPage

            //First line
            playerNameBox = new Rect(Screen.width/2-150, Screen.height /4 - 100, 300, 200);
            GUI.Box(playerNameBox, "<color=#292929><size=25>Welcome detective!</size></color>", boxStyle);
            ShowPlayerNameBoxDialog();

            if (!showTextAboutCluedo)
            {
                //Second line
                makeRoomBox = new Rect(Screen.width - 350, Screen.height / 4 - 100, 300, 200);
                GUI.Box(makeRoomBox, "<color=#292929><size=25>Make a new room</size></color>", boxStyle);
                ShowMakeRoomDialog();

                //Third line
                hostsBox = new Rect(Screen.width - 350, Screen.height / 2 - 50, 300, 200);
                GUI.Box(hostsBox, "<color=#292929><size=25>Enter an existing room</size></color>", boxStyle);
                ShowHostsList();
            }
            else
            {
                ShowTextAboutCluedo();
            }

        }
        // start game dialog
        if ((Network.isClient || Network.isServer) && startGameDialog)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", backgroundStyle2);
            GUI.Label(new Rect(20, 20, 300, 60), "The game will begin in just a second! Please wait for more players to connect. Minimum number of player is " + minimumNumberOfPlayers + ".", smallLabelStyle);

            if (showTextAboutCluedo)
                ShowTextAboutCluedo();

            string textMessage = "Investigators:" + "\n";
            List<string> connectedPlayers= GameObject.Find("GameManager").gameObject.GetComponent<GameManager>().ConnectedPlayers();
            int i = 1;
            foreach (var playerN in connectedPlayers)
            {
                textMessage += "\n" + i + ". " +playerN + " ";
                i += 1;
            }
            if (Network.isServer)
            {
               
                if (numOfPlayersConnected < minimumNumberOfPlayers)
                {
                    GUI.enabled= false;
                    GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height - 160, 150, 80), "Wait for more!", buttonTextStyle);
                    GUI.enabled = true;
                    if(!showTextAboutCluedo)
                        GUI.Label(new Rect(Screen.width - 250, 20, 300, 200), textMessage, bigLabelStyle);
                }
                else
                {
                    if(!showTextAboutCluedo)
                        GUI.Label(new Rect(Screen.width -250, 20, 300, 200), textMessage, bigLabelStyle);
                    if (GUI.Button(new Rect(Screen.width/2 - 75, Screen.height - 160, 150, 80), "Start game!", buttonTextStyle))
                    {
                        StartGame();
                    }
                }

            }
            if (Network.isClient)
            {
                GUI.Label(new Rect(Screen.width - 250, 20, 300, 200), textMessage, bigLabelStyle);
            }
        }

    }

    #region GUI methods

    void ShowTextAboutCluedo()
    {
        Rect aboutCluedoRect = new Rect(Screen.width - 500, Screen.height / 4 - 100, 500, Screen.height * 3 / 4);
        GUI.Box(aboutCluedoRect, "About Cluedo", boxStyle);

        string aboutCluedo = "* Cluedo is a board game that requires the use" +
        "of reasoning and logic skills and has a murder and mystery theme.\n" +
        "* Cluedo was designed by Anthony Pratt, a successful musician, who thought of the game during World War II.\n" +
        "* The game was first made in 1949 by Waddingtons who changed the name from ‘Murder’ (which the Pratt’s had called it) to ‘Cluedo’.\n" +
        "* In North America, Cluedo is known as Clue and some of the character’s names are changed.\n" +
        "* Cluedo was first designed to have 11 rooms, 10 characters and 9 weapons instead of the typical 9 rooms, 6 characters and six weapons.\n" +
        "* Elva Pratt, Anthony’s wife, designed the original artwork for the Cluedo board.\n" +
        "* ‘Cluedo’ is a combination of the word ‘clue’ and ‘ludo’, ‘ludo’ being Latin for ‘play’.\n" +
        "* Although Cluedo was initially designed as a game, it has been turned into films, books and other types of media.\n" +
        "* The murder victim of Cluedo is Dr Black, or Mr Boddy.\n" +
        "* The typical weapons of Cluedo are the candlestick, dagger, revolver, lead pipe, wrench and the rope, however, Pratt’s original game included an axe, bomb, syringe and poison as well as some other interesting weapons.";
        GUI.TextArea(new Rect(aboutCluedoRect.x + 50, aboutCluedoRect.y + 50, 400, Screen.height * 3 / 5), aboutCluedo);
        if (GUI.Button(new Rect(aboutCluedoRect.x + 350, aboutCluedoRect.y + Screen.height * 3 / 5 + 50, 100, 30), "Close", buttonTextStyle))
        {
            showTextAboutCluedo = !showTextAboutCluedo;
        }
    }

    void InitGUIStyles()
    {
        //Making styles for buttons, labels and textFields (input).
        buttonTextStyle = new GUIStyle(GUI.skin.button);
        labelTextStyle = new GUIStyle(GUI.skin.label);
        inputTextStyle = new GUIStyle(GUI.skin.textField);
        bigLabelStyle = new GUIStyle(GUI.skin.label);
        smallLabelStyle = new GUIStyle(GUI.skin.label);
       // boxStyleBackground = new GUIStyle(GUI.skin.box); 

        buttonTextStyle.fontStyle = FontStyle.Bold;
        buttonTextStyle.normal.textColor = Color.gray;
        labelTextStyle.fontStyle = FontStyle.Bold;
        labelTextStyle.normal.textColor = Color.gray;
        inputTextStyle.fontStyle = FontStyle.Bold;
        inputTextStyle.normal.textColor = Color.gray;
        bigLabelStyle.fontStyle = FontStyle.Bold;
        bigLabelStyle.normal.textColor = Color.gray;
        bigLabelStyle.fontSize = 17;
        bigLabelStyle.alignment = TextAnchor.UpperCenter;
        //bigLabelStyle.normal.background = (Texture2D)Resources.Load("blackBackground", typeof(Texture2D));
        //boxStyleBackground.normal.background = (Texture2D)Resources.Load("labelBackground2", typeof(Texture2D));

        smallLabelStyle.fontStyle = FontStyle.Bold;
        smallLabelStyle.normal.textColor = Color.gray;
        smallLabelStyle.alignment = TextAnchor.MiddleCenter;
        //smallLabelStyle.normal.background = (Texture2D)Resources.Load("blackBackground", typeof(Texture2D));
        //Making styles for boxes
        boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.fontStyle = FontStyle.Bold;
        boxStyle.fontSize = 18;
        boxStyle.normal.textColor = Color.gray;
        boxStyle.alignment = TextAnchor.UpperCenter;
        //boxStyle.normal.background = (Texture2D)Resources.Load("Boxes/cluedo", typeof(Texture2D));

        //Refresh button
        refreshButtonStyle = new GUIStyle(buttonTextStyle);

        refreshButtonStyle.normal.background = (Texture2D)Resources.Load("refresh", typeof(Texture2D));
        refreshButtonStyle.active.background = (Texture2D)Resources.Load("refresh2", typeof(Texture2D));
        refreshButtonStyle.focused.background = refreshButtonStyle.normal.background;
        refreshButtonStyle.hover.background = refreshButtonStyle.normal.background;


        //Setting background of StartPage
        backgroundStyle = new GUIStyle(GUI.skin.box);
        backgroundStyle.normal.background = (Texture2D)Resources.Load("mysteryMan", typeof(Texture2D));
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", backgroundStyle);

        //Setting background of RoomPage
        backgroundStyle2 = new GUIStyle(GUI.skin.box);
        backgroundStyle2.normal.background = (Texture2D)Resources.Load("mysteryMan2", typeof(Texture2D));
    }
    void ShowMakeRoomDialog()
    {
        GUI.Label(new Rect(makeRoomBox.x + 10, makeRoomBox.y + 50, 250, 30), "Insert your game room name:", labelTextStyle);
        gameName = GUI.TextField(new Rect(makeRoomBox.x + 10, makeRoomBox.y + 90, 280, 30), gameName, inputTextStyle).Trim();
        if (GUI.Button(new Rect(makeRoomBox.x + 110, makeRoomBox.y + 130, 80, 30), "Make room", buttonTextStyle))
        {
            if (string.IsNullOrEmpty(gameName))
                gameName = "DefaultRoomName";
            Network.InitializeServer(5, 25000, !Network.HavePublicAddress());
            mapNetworkPlayerToPlayerNum.Add(Network.player, 0);
            MasterServer.RegisterHost(typeName, gameName);
            show = false;
            serverStarted = true;
            SetListConnectedPlayers();
            showTextAboutCluedo = false;
        }
    }

    void ShowPlayerNameBoxDialog()
    {
        labelTextStyle.alignment = TextAnchor.MiddleCenter;
        boxStyle.fontSize = 25;
        GUI.Label(new Rect(playerNameBox.x + 10, playerNameBox.y + 50, 250, 30), "Enter your name:", labelTextStyle);
        publicPlayerName = GUI.TextField(new Rect(playerNameBox.x + 10, playerNameBox.y + 90, 280, 30), publicPlayerName, inputTextStyle).Trim();
        boxStyle.fontSize = 18;
    }

    private void ShowHostsList()
    {
		if (hostList == null)
						hosts = 0;
				else
						hosts = hostList.Length;


        GUI.Label(new Rect(hostsBox.x+50,hostsBox.y + 45, 200, 30), "Refresh list of rooms", labelTextStyle);

        if (GUI.Button(new Rect(hostsBox.x+10, hostsBox.y +40, 32, 32), "", refreshButtonStyle) || !isRefreshedFirstTime)
        {
            RefreshHostList();
            isRefreshedFirstTime = true;
        }

		scrollPosition = GUI.BeginScrollView(new Rect(hostsBox.x, hostsBox.y+70, 300, 130),
		                                     scrollPosition, new Rect(0, 0, hostsBox.width-5, (hosts<4)?130:(hosts*45) )
		                                     );
		//GUI.Button (new Rect (0, 400, 100, 100), "lol");
        if (hostList != null)
        {
            for (int i = 0; i < hostList.Length; i++)
            {
                if (GUI.Button(new Rect( 10,5+35 * i, 270, 30), hostList[i].gameName, buttonTextStyle))
                {
                    //SetListConnectedPlayers();
                    JoinServer(hostList[i]);
                    showTextAboutCluedo = false;
                }
            }
        }

		GUI.EndScrollView ();
    }

    private void SetListConnectedPlayers()
    {
        string playerName = "Player" + numOfPlayer;
        if (string.IsNullOrEmpty(publicPlayerName))
            publicPlayerName = playerName;

        GameObject.Find("GameManager").gameObject.GetComponent<GameManager>().AddPlayerToConnectedPlayers(publicPlayerName);

    }

    #endregion

    #region Network methods

    public bool ServerStarted()
    {
        return serverStarted;
    }

    private void RefreshHostList()
    {
        MasterServer.ClearHostList();
        MasterServer.RequestHostList(typeName);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            hostList = MasterServer.PollHostList();
        }
    }

    private void JoinServer(HostData hostData)
    {
        Network.Connect(hostData);
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        SetPlayerNum(player, Network.connections.Length);
        ChangePlayersConnected(Network.connections.Length+1);
        mapNetworkPlayerToPlayerNum.Add(player, Network.connections.Length);
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        // Find out which player disconnected and delete all of it's data
        int disconnectedPlayer = mapNetworkPlayerToPlayerNum[player];
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
        Debug.Log("Disconnected (NetworkManager):" + disconnectedPlayer);

        // Fix GameManager instance
        var gameManager = GameManager.Instance;
        gameManager.FixCardsAfterPlayerWasDisconnected(disconnectedPlayer);
        gameManager.SetNumOfPlayers(gameManager.NumOfPlayers() - 1);
        
        // Fix player's number stored in NetworkManager and GUIScript
        DecrementNumOfMyPlayer(disconnectedPlayer);
        gameManager.GUI().GetComponent<GUIScript>().UpdatePlayerNumber();

        // Remove disconnected player's position from game board
        BoardScript.Instance.RemoveDisconnectedPlayer(disconnectedPlayer);

        // Fix all players numbers
        for (int i = disconnectedPlayer+1; i < gameManager.NumOfPlayers()+1; i++)
        {
            var currentPlayer = GameObject.Find("Player" + i);
            currentPlayer.GetComponent<CharacterControl>().SetNum(i - 1);
            currentPlayer.name = "Player" + (i - 1);            
        }

        // Update who is on turn after current move finishes. Given that player 'x' leaves the game and
        // player 'y' is on turn there are 3 cases of interest:
        //  1) x < y  => In this case GameManager.onTurn field must remain the same cause all player numbers
        //               higher than x will be decremented, so x+1 player who should be on turn next will actually 
        //               be player number x.
        //  2) x > y  => In this case GameManager.onTurn field should be incremented as normal cause next player on turn
        //               will not be affected by x's disconnection.
        //  3) x == y => This is last one, GameManager.onTurn field must remain for the same reason as stated in case 1).
        //               Additionally, GUI window for throwing dices must be shown to the previous player x+1 and now player x.
        if (disconnectedPlayer < gameManager.OnTurn())
        {
            gameManager.SetTurn(gameManager.OnTurn() % gameManager.NumOfPlayers());
        }
        else if (disconnectedPlayer == gameManager.OnTurn())
        {
            gameManager.SetTurn(gameManager.OnTurn() % gameManager.NumOfPlayers());
            gameManager.SetDicesSum(GameManager.INVALID_DICES_SUM);
            gameManager.SetQuestionIsAsked(false);
        }
    }

    void OnConnectedToServer()
    {
        if (numOfPlayersConnected == 7)
        {
            Debug.Log("Too many players in game!");
            return;
        }
    }

    #endregion

    #region GameLogic
    private void SpawnPlayer()
    {
        Debug.Log("connected: " + numOfPlayer);

        var spawnedPlayer = Network.Instantiate(playerPrefab, CharacterControl.playersStartPositions[numOfPlayer], Quaternion.Euler(0, CharacterControl.playersSpawnAngles[numOfPlayer], 0), 0);

        string playerName = "Player" + numOfPlayer;
        spawnedPlayer.name = playerName;

        var playerObject = GameObject.Find(playerName).gameObject.GetComponent<CharacterControl>();
        playerObject.SetNum(numOfPlayer);
        playerObject.SetMaterial(numOfPlayer);

        playerObject.SetPublicName(publicPlayerName);
        playerObject.tag = "Player";
        //Setting up the cameras
        if (playerObject.networkView.isMine)
        {
            var mainCamera = GameObject.Find("Main Camera");
            mainCamera.GetComponent<FollowThePlayer>().target = playerObject.transform;
            mainCamera.transform.rotation = Quaternion.Euler(CameraControler.mainCameraStartingRotation[numOfPlayer]);

            var topViewCamera = GameObject.Find("TopViewCamera");
            topViewCamera.transform.rotation = Quaternion.Euler(CameraControler.topViewCameraStartingRotation[numOfPlayer]);
        }
    }

    #endregion

    #region Get and Set methods
    //Method for to check if game is started.
    public bool GameStarted()
    {
        return gameStarted;
    }

    public int NumberOfPlayersConnected()
    {
        return numOfPlayersConnected;
    }

    public int NumOfMyPlayer()
    {
        return numOfPlayer;
    }

    public void DecrementNumOfMyPlayer(int disconnectedPlayer)
    {
        networkView.RPC("DecrementNumOfMyPlayerRPC", RPCMode.AllBuffered, disconnectedPlayer);
    }

    #endregion

    #region RPC Wrappers
    private void ChangePlayersConnected(int numberOfPlayers)
    {
        networkView.RPC("ChangePlayersConnectedNetwork", RPCMode.AllBuffered, numberOfPlayers);
    }

    private void StartGame()
    {
        networkView.RPC("StartGameNetwork", RPCMode.AllBuffered);
    }

    private void SetPlayerNum(NetworkPlayer player, int num)
    {
        networkView.RPC("SetPlayerNumNetwork", RPCMode.AllBuffered, player, num);
    }
    #endregion

    #region RPC

    // RPC methods, actual methods for changing values. They have wrappers so we can control which peers we want to notify
    // about changes and how. Basically wrappers are not needed, but I like to write, cause I think its cleaner and doesnt force
    // you to learn Unity Network part if you dont want to.
    [RPC]
    private void SetPlayerNumNetwork(NetworkPlayer player, int num)
    {
        if (Network.player == player)
        {
            numOfPlayer = num;
        }
    }
    [RPC]
    private void ChangePlayersConnectedNetwork(int num)
    {
        numOfPlayersConnected = num;
    }
    [RPC]
    private void StartGameNetwork()
    {
        SpawnPlayer();
        gameStarted = true;
        startGameDialog = false;
    }

    [RPC]
    private void DecrementNumOfMyPlayerRPC(int disconnectedPlayer)
    {
        if (Network.isClient && numOfPlayer > disconnectedPlayer)
            --numOfPlayer;
    }
    #endregion
}
