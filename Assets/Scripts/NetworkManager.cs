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
    public int minimumNumberOfPlayers = 3;

    // Variable for server to know how many players are there
    // you can change it to public so you can track changes in debug.
    private int numOfPlayersConnected = 0;

    // Bool triggers for game start dialog.
    private bool spawnTrigger = false;
    private bool startGameDialog = true;
    // Do not change this to public, you have below method for checking if game is started.
    private bool gameStarted = false;
    private bool serverStarted = false;
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
    private GUIStyle smallLabelStyle;
    private Vector2 scrollPosition = Vector2.zero;

    //private GUIStyle boxStyleBackground;
    private bool isRefreshedFirstTime = false;
    #endregion

    #region Camera
    public Vector3[] startPositions = { new Vector3(8, 2, 15), new Vector3(14, 2, 8), new Vector3(9, 2, -7), new Vector3(0, 2, -7), new Vector3(-8, 2, -1), new Vector3(-8, 2, 8) };
    public Vector3[] startCameraRotations = { 
                                                new Vector3(45, 180, 0), 
                                                new Vector3(45, -90, 0), 
                                                new Vector3(45, 0, 0), 
                                                new Vector3(45, 0, 0), 
                                                new Vector3(45, 90, 0), 
                                                new Vector3(45, 90, 0) };
    public Vector3[] startTopViewCameraRotations = {
                                                       new Vector3(90, 180, 0), 
                                                       new Vector3(90, 270, 0), 
                                                       new Vector3(90, 0, 0), 
                                                       new Vector3(90, 0, 0), 
                                                       new Vector3(90, 90, 0), 
                                                       new Vector3(90, 90, 0)};

    #endregion

    #region Player data
    private string publicPlayerName = "";
    public GameObject playerPrefab;
    public Material[] playerMaterials;
    // Variable for network manager to know which player is his, for later spawning.
    private int numOfPlayer = 0;
	private int hosts;
    #endregion

    //#TODO: Disconnect logic.
    //#TODO: Chat and name logic. 

    void Start()
    {
    }
    void Update()
    {
        if (isRefreshingHostList && MasterServer.PollHostList().Length > 0)
        {
            isRefreshingHostList = false;
            hostList = MasterServer.PollHostList();
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
            playerNameBox = new Rect(Screen.width / 4 - 150, Screen.height / 10, 300, 200);
            GUI.Box(playerNameBox, "You can choose your name!", boxStyle);
            ShowPlayerNameBoxDialog();

            //Second line
            makeRoomBox = new Rect(Screen.width / 2 - 150, Screen.height / 10, 300, 200);
            GUI.Box(makeRoomBox, "Make a new room", boxStyle);
            ShowMakeRoomDialog();

            //Third line
            hostsBox = new Rect(Screen.width * 3 / 4 - 150, Screen.height / 10, 300, 200);
            GUI.Box(hostsBox, "Enter an existing room", boxStyle);
            ShowHostsList();


            GUI.Box(new Rect(Screen.width / 4 - 150, Screen.height / 2 + 100, 965, 350), "" ,boxStyle);
            GUI.Label(new Rect(Screen.width/4 - 140, Screen.height/2 +100 , 965, 350), "* Cluedo is a board game that requires the use"+ 
            "of reasoning and logic skills and has a murder and mystery theme.\n" +
            "* Cluedo was designed by Anthony Pratt, a successful musician, who thought of the game during World War II.\n" +
            "* The game was first made in 1949 by Waddingtons who changed the name from ‘Murder’ (which the Pratt’s had called it) to ‘Cluedo’.\n"+
            "* In North America, Cluedo is known as Clue and some of the character’s names are changed.\n"+
            "* Cluedo was first designed to have 11 rooms, 10 characters and 9 weapons instead of the typical 9 rooms, 6 characters and six weapons.\n"+
            "* Elva Pratt, Anthony’s wife, designed the original artwork for the Cluedo board.\n"+
            "* ‘Cluedo’ is a combination of the word ‘clue’ and ‘ludo’, ‘ludo’ being Latin for ‘play’.\n"+
            "* Although Cluedo was initially designed as a game, it has been turned into films, books and other types of media.\n"+
            "* The murder victim of Cluedo is Dr Black, or Mr Boddy.\n" +
            "* The typical weapons of Cluedo are the candlestick, dagger, revolver, lead pipe, wrench and the rope, however, Pratt’s original game included an axe, bomb, syringe and poison as well as some other interesting weapons.", labelTextStyle);


        }
        // start game dialog
        if ((Network.isClient || Network.isServer) && startGameDialog)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", backgroundStyle);
            GUI.Label(new Rect(20, 20, 300, 60), "The game will begin in just a second! Please wait for more players to connect. Minimum number of player is " + minimumNumberOfPlayers + ".", smallLabelStyle);
            //GUI.Box(new Rect(Screen.width / 2 - 320, 80, 640, 480), "", boxStyleBackground);
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
                    GUI.Label(new Rect(Screen.width - 250, 20, 300, 200), textMessage, bigLabelStyle);
                }
                else
                {
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
        buttonTextStyle.normal.textColor = Color.yellow;
        labelTextStyle.fontStyle = FontStyle.Bold;
        labelTextStyle.normal.textColor = Color.yellow;
        inputTextStyle.fontStyle = FontStyle.Bold;
        inputTextStyle.normal.textColor = Color.yellow;
        bigLabelStyle.fontStyle = FontStyle.Bold;
        bigLabelStyle.normal.textColor = Color.yellow;
        bigLabelStyle.fontSize = 17;
        bigLabelStyle.alignment = TextAnchor.UpperCenter;
        //bigLabelStyle.normal.background = (Texture2D)Resources.Load("blackBackground", typeof(Texture2D));
        //boxStyleBackground.normal.background = (Texture2D)Resources.Load("labelBackground2", typeof(Texture2D));

        smallLabelStyle.fontStyle = FontStyle.Bold;
        smallLabelStyle.normal.textColor = Color.yellow;
        smallLabelStyle.alignment = TextAnchor.MiddleCenter;
        //smallLabelStyle.normal.background = (Texture2D)Resources.Load("blackBackground", typeof(Texture2D));
        //Making styles for boxes
        boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.fontStyle = FontStyle.Bold;
        boxStyle.fontSize = 18;
        boxStyle.normal.textColor = Color.yellow;
        boxStyle.alignment = TextAnchor.UpperCenter;
        boxStyle.normal.background = (Texture2D)Resources.Load("proba2", typeof(Texture2D));

        //Setting background of StartPage
        backgroundStyle = new GUIStyle(GUI.skin.box);
        backgroundStyle.normal.background = (Texture2D)Resources.Load("crimescene", typeof(Texture2D));
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", backgroundStyle);
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
            MasterServer.RegisterHost(typeName, gameName);
            show = false;
            serverStarted = true;
            SetListConnectedPlayers();
        }
    }

    void ShowPlayerNameBoxDialog()
    {
        GUI.Label(new Rect(playerNameBox.x + 10, playerNameBox.y + 50, 250, 30), "Enter your name:", labelTextStyle);
        publicPlayerName = GUI.TextField(new Rect(playerNameBox.x + 10, playerNameBox.y + 90, 280, 30), publicPlayerName, inputTextStyle).Trim();
    }

    private void ShowHostsList()
    {
		if (hostList == null)
						hosts = 0;
				else
						hosts = hostList.Length;


        GUI.Label(new Rect(hostsBox.x+50,hostsBox.y + 45, 200, 30), "Refresh list of rooms", labelTextStyle);
        if (GUI.Button(new Rect(hostsBox.x+10, hostsBox.y +40, 30, 30), "R", buttonTextStyle) || !isRefreshedFirstTime)
        {
            RefreshHostList();
            isRefreshedFirstTime = true;
        }

		scrollPosition = GUI.BeginScrollView(new Rect(Screen.width * 3 / 4 - 150, Screen.height / 10+70, 300, 130),
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
        if (!isRefreshingHostList)
        {
            isRefreshingHostList = true;
            MasterServer.RequestHostList(typeName);
        }
    }

    private void JoinServer(HostData hostData)
    {
        numOfPlayer = hostData.connectedPlayers;
        Network.Connect(hostData);
        //SetListConnectedPlayers();

    }

    void OnConnectedToServer()
    {
        if (numOfPlayersConnected == 7)
        {
            Debug.Log("Too many players in game!");
            return;
        }
        ChangePlayersConnected(numOfPlayer + 1);
        Debug.Log("Number of players: " + numOfPlayer + 1);
        SetListConnectedPlayers();
    }

    #endregion

    #region GameLogic
    private void SpawnPlayer()
    {
        Debug.Log("connected: " + numOfPlayer);

        var spawnedPlayer = Network.Instantiate(playerPrefab, startPositions[numOfPlayer], Quaternion.identity, 0);

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
            var camera1 = GameObject.Find("Main Camera");
            camera1.GetComponent<FollowThePlayer>().target = playerObject.transform;
            camera1.transform.rotation = Quaternion.Euler(startCameraRotations[numOfPlayer]);

            var camera2 = GameObject.Find("TopViewCamera");
            camera2.transform.rotation = Quaternion.Euler(startTopViewCameraRotations[numOfPlayer]);
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
    #endregion

    #region RPC

    // RPC methods, actual methods for changing values. They have wrappers so we can control which peers we want to notify
    // about changes and how. Basically wrappers are not needed, but I like to write, cause I think its cleaner and doesnt force
    // you to learn Unity Network part if you dont want to.
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

    #endregion
}
