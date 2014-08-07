using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    private const string typeName = "UniqueGameName";
    private const string gameName = "RoomNameAlex";

    private bool isRefreshingHostList = false;
    private HostData[] hostList;

    public GameObject playerPrefab;
	public Vector3[] startPositions = {new Vector3 (-1, 0, 22),new Vector3 (5, 0, 15) , new Vector3 (0, 0, 0),
		new Vector3 (-9, 0, 0),  new Vector3 (-17, 0, 6), new Vector3 (-17, 0, 15)};

	public int numOfPlayersConnected;

	void Start()
	{
		numOfPlayersConnected = 0;
	}

    void OnGUI()
    {
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
    }

    private void StartServer()
    {
        Network.InitializeServer(5, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
    }

    void OnServerInitialized()
    {
        SpawnPlayer();
    }


    void Update()
    {
        if (isRefreshingHostList && MasterServer.PollHostList().Length > 0)
        {
            isRefreshingHostList = false;
            hostList = MasterServer.PollHostList();
        }

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
        numOfPlayersConnected = hostData.connectedPlayers;
        Network.Connect(hostData);
    }

    void OnConnectedToServer()
    {
		if (numOfPlayersConnected == 7) {
			Debug.Log("Too many players in game!");
						return;
				}
        
        SpawnPlayer();
    }


    private void SpawnPlayer()
    {
		Debug.Log("connected: " + numOfPlayersConnected);

		Debug.Log ("Spawning new player. PlayerNum = " + numOfPlayersConnected + ".");
		var spawnedPlayer = Network.Instantiate(playerPrefab, startPositions[numOfPlayersConnected], Quaternion.identity, 0);

		string playerName = "Player" + numOfPlayersConnected;
		spawnedPlayer.name = playerName;

		var playerObject = GameObject.Find (playerName).gameObject.GetComponent<CharacterControl> ();
		playerObject.setNumNet(numOfPlayersConnected);
		playerObject.tag = "Player";

		//numOfPlayersConnected++;
		Debug.Log ("New player spawned on position: " + startPositions[numOfPlayersConnected].x + "," + 
		           startPositions[numOfPlayersConnected].y + ", " + startPositions[numOfPlayersConnected].z);
    }

    
}
