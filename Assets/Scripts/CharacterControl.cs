using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{
    /// <summary>
    /// Starting coordinates on board for players.
    /// </summary>
    public static readonly Vector3[] playersStartPositions = { 
                                                                new Vector3(8, 2, 15), 
                                                                new Vector3(14, 2, 8), 
                                                                new Vector3(9, 2, -7), 
                                                                new Vector3(0, 2, -7), 
                                                                new Vector3(-8, 2, -1), 
                                                                new Vector3(-8, 2, 8) 
                                                             };

    /// <summary>
    /// Array of angles in degrees. These angles are used to change coordinate system rotation of player prefabs
    /// so their 'z' axis would be theirs front.
    /// </summary>
    public static readonly int[] playersSpawnAngles = { 180, 270, 0, 0, 90, 90 };

    private BoardScript board; // Provides data about players on map and map itself
    private int playerNum; // Number of this player
    private int numOfMoves; // Number of made moves in current series of moves
    private string publicName; //Name that player choose

    private static Vector3 faceForward = new Vector3(0, 0, -1);
    private static Vector3 faceBackwards = new Vector3(0, 0, 1);
    private static Vector3 faceLeft = new Vector3(1, 0, 0);
    private static Vector3 faceRight = new Vector3(-1, 0, 0);
    private static Vector3 yAxis = new Vector3(0, 1, 0);
    private static Vector3 zAxis = new Vector3(0, 0, 1);

    private Vector3 oldPosition;
    private Vector3 newPosition;
    private float lerpPosition, lerpTime;
    private bool moveStarted;
    private int spawnAngle;

    // Use this for initialization
    void Start()
    {
        board = GameObject.Find("Board").gameObject.GetComponent<BoardScript>();
        oldPosition = transform.position;
        newPosition = transform.position;
        lerpPosition = 0F;
        lerpTime = 0.30F;
        numOfMoves = 0;
        moveStarted = false;
        spawnAngle = playersSpawnAngles[playerNum];
    }

    void Update()
    {
        if (networkView.isMine)
        {
            //Player must throw dices before his movement and it must be his turn to play
            var gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
            if (gameManager.DicesSum() == GameManager.INVALID_DICES_SUM || gameManager.OnTurn() != playerNum)
                return;

            // Start move on arrow key pressed only if another move isn't in progress
            if (!moveStarted)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    Move(270);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    Move(90);
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    Move(0);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    Move(180);
                }
            }

            // Move player in a smooth fashion
            if (transform.position != newPosition)
            {
                lerpPosition += Time.deltaTime / lerpTime;
                transform.position = Vector3.Lerp(oldPosition, newPosition, lerpPosition);
                moveStarted = true;
            }
            else
            {
                // Reset move variables 
                oldPosition = newPosition;
                lerpPosition = 0F;
                
                // Increase number of moves when done moving
                if (moveStarted)
                {
                    var currentPosition = board.PlayerPos(playerNum);
                    if (board.board[currentPosition.X, currentPosition.Z] == (int)Rooms.Hallway)
                    {
                        SetNumOfMoves(numOfMoves + 1);
                    }
                    moveStarted = false;
                }
            }
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 syncPosition = Vector3.zero;
        if (stream.isWriting)
        {
            syncPosition = rigidbody.position;
            stream.Serialize(ref syncPosition);
        }
        else
        {
            stream.Serialize(ref syncPosition);
            rigidbody.position = syncPosition;
        }
    }

    #region Move functions

    public bool Move(int angle)
    {
        // When "TopViewCamera" is changed to "Main Camera" and the move is made on "Main Camera" player's
        // coordinate system will be turned in direction of move. After that changing to "TopViewCamera"
        // gives player with bad coordinate system for top view moving. So we need to check and reset coordinate
        // system to original one in case of top view camera.
        if (CameraControler.Instance.IsTopViewCamera)
        {
            transform.rotation = Quaternion.Euler(0, spawnAngle, 0);
        }

        // Save rotation, rotate, get current position and decode move
        var saveRotation = transform.rotation;
        transform.Rotate(yAxis, angle);
        var currentPosition = board.PlayerPos(playerNum);
        var nextPosition = NextPosition(currentPosition);

        // Set next position if move is valid (board and new position for "this")
        if (board.IsValid(playerNum, currentPosition.X, currentPosition.Z, nextPosition.X, nextPosition.Z))
        {
            // Update positions
            newPosition = transform.position + transform.forward;
            board.SetPlayerPosition(playerNum, nextPosition.X, nextPosition.Z);

            // Update players coordinate system to original rotation in case of top view camera
            if (CameraControler.Instance.IsTopViewCamera)
            {
                transform.rotation = Quaternion.Euler(0, CharacterControl.playersSpawnAngles[playerNum], 0);
            }
            return true;
        }
        
        // Move wasn't successful so player needs to be rotated to original position
        transform.rotation = saveRotation;
        return false;
    }

    private BoardScript.PlayerPosition NextPosition(BoardScript.PlayerPosition currentPosition)
    {
        if (transform.forward == faceForward)
        {
            return new BoardScript.PlayerPosition(currentPosition.X, currentPosition.Z + 1);
        }
        else if (transform.forward == faceBackwards)
        {
            return new BoardScript.PlayerPosition(currentPosition.X, currentPosition.Z - 1);
        }
        else if (transform.forward == faceLeft)
        {
            return new BoardScript.PlayerPosition(currentPosition.X - 1, currentPosition.Z);
        }
        else
        {
            return new BoardScript.PlayerPosition(currentPosition.X + 1, currentPosition.Z);
        }
    }

    #endregion

    #region Get and set methods

    public int GetPlayerNum()
    {
        return playerNum;
    }

    public void SetNum(int num)
    {
        networkView.RPC("SetNumRPC", RPCMode.AllBuffered, num);
    }

    public void SetNumOfMoves(int moves)
    {
        networkView.RPC("SetNumOfMovesRPC", RPCMode.AllBuffered, moves);
    }

    public void SetMaterial(int mat)
    {
        networkView.RPC("SetMaterialRPC", RPCMode.AllBuffered, mat);
    }

    public int NumOfMoves()
    {
        return numOfMoves;
    }

    public void SetPublicName(string playerName)
    {
        networkView.RPC("SetPublicNameRPC", RPCMode.AllBuffered, playerName);
    }

    public string PublicName()
    {
        return publicName;
    }

    #endregion

    #region RPC set methods

    [RPC]
    private void SetNumRPC(int num)
    {
        playerNum = num;
        name = "Player" + num;
    }

    [RPC]
    private void SetNumOfMovesRPC(int moves)
    {
        numOfMoves = moves;
    }

    [RPC]
    private void SetMaterialRPC(int materialIndex)
    {
        Material mat = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().playerMaterials[materialIndex];
        transform.Find("PlayerGreen").renderer.material = mat;
        transform.Find("Sphere").renderer.material = mat;
    }

    [RPC]
    private void SetPublicNameRPC(string playerName)
    {
        publicName = playerName;
    }
    #endregion
}
