using UnityEngine;
using System.Collections;


public class NetworkManager : MonoBehaviour
{
    // Unique typename for game (Hopefully unique).
    private const string typeName = "Cluedo3DSAMOSTeam";
    private string gameName ="";
	// Only show Gui Window if needed.
	private bool show = false;
	public Rect boxRect = new Rect (Screen.width / 2 - 150, Screen.height / 2 - 150, 300, 300);

    public Material[] playerMaterials; 

    private bool isRefreshingHostList = false;
    private HostData[] hostList;

    public GameObject playerPrefab;
    public Vector3[] startPositions = {new Vector3 (8, 2, 15),new Vector3 (14, 2, 8) , new Vector3 (9, 2, -7),new Vector3 (0, 2, -7),  new Vector3 (-8, 2, -1), new Vector3 (-8, 2, 8)};
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

    // Number of minimum players need to be so game could start, its for testing and debuging purposes
    // because its frustrating to start three instances everytime. By default is set to 3 but you can change it from Unity editor, NOT HERE!
    // Again, NOT HERE, in unity editor (inspector).
    public int minimumNumberOfPlayers = 3;

    // Variable for server to know how many players are there
    // you can change it to public so you can track changes in debug.
    private int numOfPlayersConnected = 0;

    // Variable for network manager to know which player is his, for later spawning.
    private int numOfPlayer = 0;

    // Bool triggers for game start dialog.
    private bool spawnTrigger = false;
    private bool startGameDialog = true;
    // Do not change this to public, you have below method for checking if game is started.
    private bool gameStarted = false;
    private bool serverStarted = false;


    //#TODO: Disconnect logic.
    //#TODO: Chat and name logic. 

    void Start()
    {
    }

    void OnGUI()
    {
        // Start or join server dialog



        if (!Network.isClient && !Network.isServer)
        {

			if(show){
				GUI.Box(boxRect,"Make a new room");
				DialogWindow();
			}

			if(!show){
            if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
                StartServer();

            if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
                RefreshHostList();

            if (hostList != null)
            {
                for (int i = 0; i < hostList.Length; i++)
                {
                    if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
                        JoinServer(hostList[i]);
                }
            }
			}
        }
        // start game dialog
        if ((Network.isClient || Network.isServer) && startGameDialog)
        {
            if (Network.isServer)
            {
                if (numOfPlayersConnected < minimumNumberOfPlayers)
                {
                    GUI.Label(new Rect(10, 10, 100, 20), "Waiting for more people to connect!");
                }
                else
                {
                    if (GUI.Button(new Rect(100, 100, 250, 100), "Start game!"))
                    {
                        StartGame();
                    }
                }

            }
            if (Network.isClient)
            {
                GUI.Label(new Rect(10, 10, 100, 20), "Waiting for game to start");
            }
        }

    }

    void Update()
    {
        if (!spawnTrigger && gameStarted)
        {
            spawnTrigger = true;
            SpawnPlayer();
        }
        if (isRefreshingHostList && MasterServer.PollHostList().Length > 0)
        {
            isRefreshingHostList = false;
            hostList = MasterServer.PollHostList();
        }

    }


		
	

    //Network methods
    private void StartServer()
    {
		show = true;

	}
	//function caled in onGui Gui.Window
	void DialogWindow ()
    {
		GUI.Label (new Rect(boxRect.x+10,boxRect.y+30,250,30), "Insert your game room name");
		gameName = GUI.TextField (new Rect(boxRect.x+10,boxRect.y+70,250,30), gameName).Trim ();
		if (GUI.Button (new Rect (boxRect.x+10,boxRect.y+110,80,30), "Make room")) {
				
						if (string.IsNullOrEmpty (gameName))
								gameName = "DefaultRoomName";
						Network.InitializeServer (5, 25000, !Network.HavePublicAddress ());
						MasterServer.RegisterHost (typeName, gameName);
						show = false;
						serverStarted = true;
				}
		if (GUI.Button (new Rect (boxRect.x+180,boxRect.y+110,80,30), "Cancel")) {
			gameName="";
			show = false;
		}				
	}
	
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
    }

    //Game Logic methods
    private void SpawnPlayer()
    {
        Debug.Log("connected: " + numOfPlayer);

        var spawnedPlayer = Network.Instantiate(playerPrefab, startPositions[numOfPlayer], Quaternion.identity, 0);

        string playerName = "Player" + numOfPlayer;
        spawnedPlayer.name = playerName;

        var playerObject = GameObject.Find(playerName).gameObject.GetComponent<CharacterControl>();
        playerObject.SetNum(numOfPlayer);
        playerObject.SetMaterial(numOfPlayer);
        playerObject.tag = "Player";
        GameObject.Find("GUI").GetComponent<GUIScript>().setPlayerNum(numOfPlayer);
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

    // Wrapper methods
    private void ChangePlayersConnected(int numberOfPlayers)
    {
        networkView.RPC("ChangePlayersConnectedNetwork", RPCMode.AllBuffered, numberOfPlayers);
    }

    private void StartGame()
    {
        networkView.RPC("StartGameNetwork", RPCMode.AllBuffered);
    }

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
        gameStarted = true;
        startGameDialog = false;
    }
}
