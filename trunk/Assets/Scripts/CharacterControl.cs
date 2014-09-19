using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{
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

    // Use this for initialization
    void Start()
    {
        board = GameObject.Find("Board").gameObject.GetComponent<BoardScript>();
        oldPosition = transform.position;
        newPosition = transform.position;
        lerpPosition = 0F;
        lerpTime = 0.75F;
        numOfMoves = 0;
        moveStarted = false;
    }

    void Update()
    {
        if (networkView.isMine)
        {
            var gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();

            //Player must throw dices before his movement
            if (gameManager.DicesSum() == GameManager.INVALID_DICES_SUM)
                return;

            // Rotate coordinate system and then move, if move fails rotate to original position
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

            // Update top view camera
            var topViewCamera = GameObject.Find("TopViewCamera");
            topViewCamera.GetComponent<TopViewCameraRotation>().target = topViewCamera.transform.rotation * Quaternion.Euler(0, 0, -angle);
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
