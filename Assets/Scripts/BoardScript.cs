using UnityEngine;
using System.Collections;

public class BoardScript : MonoSingleton<BoardScript>
{
    /// <summary>
    /// Struct that will be used to represent player's position on board.
    /// </summary>
    public struct PlayerPosition
    {
        private int x;
        private int z;

        public PlayerPosition(int px, int pz)
        {
            this.x = px;
            this.z = pz;
        }

        public int X { get { return x; } set { this.x = value; } }
        public int Z { get { return z; } set { this.z = value; } }
    };

    /// <summary>
    /// Struct that contains data about field positions relative to the door and room that door belongs to.
    /// </summary>
    public struct Door
    {
        private int in_x;
        private int in_z;
        private int out_x;
        private int out_z;
        private Rooms room;

        public Door(int x1, int z1, int x2, int z2, Rooms r)
        {
            this.in_x = x1;
            this.in_z = z1;
            this.out_x = x2;
            this.out_z = z2;
            this.room = r;
        }

        public int In_x { get { return in_x; } }
        public int In_z { get { return in_z; } }
        public int Out_x { get { return out_x; } }
        public int Out_z { get { return out_z; } }
        public Rooms Room { get { return room; } }


    };

    #region Board hardcoded definition
    /// <summary>
    /// Map of allRooms
    /// </summary>
    public int[,] board =
	{// TODO: If there's time change board to be array of Rooms instead of int array.
		{(int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Hallway, (int)Rooms.Hallway, (int)Rooms.SleepingRoom, (int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.Hallway,(int)Rooms.Hallway, (int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen},
		{(int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Hallway, (int)Rooms.Hallway, (int)Rooms.SleepingRoom, (int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.Hallway,(int)Rooms.Hallway, (int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen},
		{(int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Hallway, (int)Rooms.Hallway, (int)Rooms.SleepingRoom, (int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.Hallway,(int)Rooms.Hallway, (int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen},
		{(int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Hallway, (int)Rooms.Hallway, (int)Rooms.SleepingRoom, (int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.Hallway,(int)Rooms.Hallway, (int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen},
		{(int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Hallway, (int)Rooms.Hallway, (int)Rooms.SleepingRoom, (int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.Hallway,(int)Rooms.Hallway, (int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen,(int) Rooms.Kitchen},
		{(int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Studio, (int)Rooms.Hallway, (int)Rooms.Hallway, (int)Rooms.SleepingRoom, (int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.SleepingRoom,(int)Rooms.Hallway,(int)Rooms.Hallway, (int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway},
		{(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway},
		{(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway},
		{(int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom ,(int) Rooms.DiningRoom , (int) Rooms.DiningRoom, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Billiard, (int) Rooms.Billiard ,(int) Rooms.Billiard, (int) Rooms.Billiard, (int) Rooms.Billiard},
		{(int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom ,(int) Rooms.DiningRoom , (int) Rooms.DiningRoom, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Billiard, (int) Rooms.Billiard ,(int) Rooms.Billiard, (int) Rooms.Billiard, (int) Rooms.Billiard},
		{(int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom ,(int) Rooms.DiningRoom , (int) Rooms.DiningRoom, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Billiard, (int) Rooms.Billiard ,(int) Rooms.Billiard, (int) Rooms.Billiard, (int) Rooms.Billiard},
		{(int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom ,(int) Rooms.DiningRoom , (int) Rooms.DiningRoom, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Billiard, (int) Rooms.Billiard ,(int) Rooms.Billiard, (int) Rooms.Billiard, (int) Rooms.Billiard},
		{(int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hall, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom ,(int) Rooms.DiningRoom , (int) Rooms.DiningRoom, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Billiard, (int) Rooms.Billiard ,(int) Rooms.Billiard, (int) Rooms.Billiard, (int) Rooms.Billiard},
		{(int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom ,(int) Rooms.DiningRoom , (int) Rooms.DiningRoom, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Billiard, (int) Rooms.Billiard ,(int) Rooms.Billiard, (int) Rooms.Billiard, (int) Rooms.Billiard},
		{(int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom, (int) Rooms.DiningRoom ,(int) Rooms.DiningRoom , (int) Rooms.DiningRoom, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway, (int) Rooms.Hallway,(int) Rooms.Hallway,(int) Rooms.Hallway,(int) Rooms.Hallway},
		{(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway},
		{(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway},
		{(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Cabinet,(int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway},
		{(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Cabinet,(int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library },
		{(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Cabinet,(int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library },
		{(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Cabinet,(int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library },
		{(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Cabinet,(int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library },
		{(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom,(int)Rooms.GuestsRoom, (int)Rooms.GuestsRoom,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Cabinet,(int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Cabinet, (int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Hallway,(int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library, (int)Rooms.Library }
	};
    #endregion

    #region Doors hardcoded definition
    /// <summary>
    /// Doors with their positions and allRooms.
    /// </summary>
    public Door[] doors = {new Door(6, 4, 5, 4, Rooms.Studio), 
						 	new Door(3, 7, 3, 8, Rooms.SleepingRoom),
							new Door(2, 7, 2, 8, Rooms.SleepingRoom),
							new Door(10, 9, 10, 8, Rooms.Hall),
							new Door(13, 4, 12, 4, Rooms.Hall),
							new Door(14,4, 15, 4, Rooms.GuestsRoom),
							new Door(4, 14, 4, 13, Rooms.SleepingRoom),
							new Door(10, 16, 10, 15, Rooms.DiningRoom),
							new Door(15, 14, 14, 14, Rooms.DiningRoom),
							new Door(18, 14, 18, 13, Rooms.Cabinet),
							new Door(16, 12, 17, 12, Rooms.Cabinet),
							new Door(19, 16, 19, 17, Rooms.Library),
							new Door(11, 17, 11, 18, Rooms.Billiard),
							new Door(7, 20, 8, 20, Rooms.Billiard),
							new Door(2, 15, 2, 16, Rooms.Kitchen)};
    #endregion

    /// <summary>
    /// Vector that contains players positions on board.
    /// </summary>
    private PlayerPosition[] playersPosition = { new PlayerPosition(6, 0), new PlayerPosition(0,7),
		                                             new PlayerPosition(5, 22), new PlayerPosition(14,22), 
		                                             new PlayerPosition(22, 16),new PlayerPosition(22,7)
                                               };

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }


    public PlayerPosition PlayerPos(int whichPlayer)
    {
        return playersPosition[whichPlayer];
    }

    /// <summary>
    /// Indicator function for doors.
    /// </summary>
    /// <param name="x1"> x coordinate of first field </param>
    /// <param name="z1"> z coordinate of first field </param>
    /// <param name="x2"> x coordinate of second field </param>
    /// <param name="z2"> z coordinate of second field </param>
    /// <returns>Function returns true if there are door between these two fields.</returns>
    public bool IsDoor(int x1, int z1, int x2, int z2)
    {
        foreach (Door d in doors)
        {
            if (d.In_x == x1 && d.In_z == z1 && d.Out_x == x2 && d.Out_z == z2)
                return true;
            if (d.In_x == x2 && d.In_z == z2 && d.Out_x == x1 && d.Out_z == z1)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Function that check if some move is valid.
    /// </summary>
    /// <param name="playerNum"> Number of current player </param>
    /// <param name="px"> current x coordinate </param>
    /// <param name="pz"> current z coordinate </param>
    /// <param name="npx"> potentional new x coordinate </param>
    /// <param name="npz"> potentional new z coordinate </param>
    /// <returns> True if move is valid </returns>
    public bool IsValid(int playerNum, int px, int pz, int npx, int npz)
    {
        // Dimensions of board are 23*23
        if (npx < 0 || npx > 22 || npz < 0 || npz > 22)
            return false;

        // Player can't walk over some other player
        foreach (PlayerPosition pp in playersPosition)
            if (pp.X == npx && pp.Z == npz)
                return false;

        // Player can't walk through walls (change room without crossing the door)
        int currentRoom = board[px, pz];
        int nextRoom = board[npx, npz];
        if (currentRoom != nextRoom && !IsDoor(px, pz, npx, npz))
            return false;

        return true;
    }

    /// <summary>
    /// Player can check if he is in any room or hallway using this method.
    /// </summary>
    /// <param name="whichPlayer">Player asking for position.</param>
    /// <returns>Room enum for player.</returns>
    public Rooms WhereAmI(int whichPlayer)
    {
        var playerPos = playersPosition[whichPlayer];
        return (Rooms)board[playerPos.X, playerPos.Z];
    }

    public void SetPlayerPosition(int whichPlayer, int x, int z)
    {
        networkView.RPC("SetPlayerPositionRPC", RPCMode.AllBuffered, whichPlayer, x, z);
    }

    [RPC]
    private void SetPlayerPositionRPC(int whichPlayer, int x, int z)
    {
        playersPosition[whichPlayer].X = x;
        playersPosition[whichPlayer].Z = z;
    }
}
