using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    // Unique typename for game (Hopefully unique).
    private const string typeName = "Cluedo3DSAMOSTeam";
    private const string gameName = "RoomNameAlex";

    private bool isRefreshingHostList = false;
    private HostData[] hostList;

    public GameObject playerPrefab;
	public Vector3[] startPositions = {new Vector3 (-1, 0, 22),new Vector3 (5, 0, 15) , new Vector3 (0, 0, 0),
		new Vector3 (-9, 0, 0),  new Vector3 (-17, 0, 6), new Vector3 (-17, 0, 15)};

    // Number of minimum players need to be so game could start, its for testing and debuging purposes
    // because its frustrating to start three instances everytime. By default is set to 3 but you can change it from Unity editor, NOT HERE!
    // Again, NOT HERE, in unity editor (inspector).
    public int minimumNumberOfPlayers = 3;

    // Variable for server to know how many players is there
    // you can change it to public so you can track changes in debug.
	private int numOfPlayersConnected=0;

    // Variable for network manager to know which player is his, for later spawning.
    private int numOfPlayer = 0;

    // Bool triggers for game start dialog.
    private bool spawnTrigger = false;
    private bool startGameDialog = true;
    // Do not change this to public, you have below method for checking if game is started.
    private bool gameStarted = false;
	
    
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
        // start game dialog
        if ((Network.isClient || Network.isServer) && startGameDialog)
        {
            if (Network.isServer)
            {
                if(numOfPlayersConnected < minimumNumberOfPlayers){
                    GUI.Label(new Rect(10, 10, 100, 20), "Waiting for more people to connect!");
                }
                else{
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
        Network.InitializeServer(5, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
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
		if (numOfPlayersConnected == 7) {
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

		Debug.Log ("Spawning new player. PlayerNum = " + numOfPlayer + ".");
		var spawnedPlayer = Network.Instantiate(playerPrefab, startPositions[numOfPlayer], Quaternion.identity, 0);

		string playerName = "Player" + numOfPlayer;
		spawnedPlayer.name = playerName;

		var playerObject = GameObject.Find (playerName).gameObject.GetComponent<CharacterControl> ();
		playerObject.setNumNet(numOfPlayer);
		playerObject.tag = "Player";

		//numOfPlayersConnected++;
		Debug.Log ("New player spawned on position: " + startPositions[numOfPlayer].x + "," + 
		           startPositions[numOfPlayer].y + ", " + startPositions[numOfPlayer].z);
    }

    //Method for to check if game is started.
    public bool isGameStarted()
    {
        return gameStarted;
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
