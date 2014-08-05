using UnityEngine;
using System.Collections;

public class BoardScript : MonoBehaviour {


	public enum Rooms : int
	{
		Studio = 1, Hall, GuestsRoom, SleepingRoom, DiningRoom, Cabinet, Kitchen, Billiard, Library, Hallway
	};

	//public Rooms RoomsProperty { get; set;}

	public struct PlayerPosition
	{
		private int x;
		private int z;

		public PlayerPosition(int px, int pz)
		{
			this.x = px;
			this.z = pz;
		}

		public int X {get {return x;} set {this.x = value;}}
		public int Z {get {return z;} set {this.z = value;}}
	};

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

		public int In_x {get {return in_x;}}
		public int In_z {get {return in_z;}}
		public int Out_x {get {return out_x;}}
		public int Out_z {get {return out_z;}}
		public Rooms Room {get {return room;}}


	};

	public int[,] board =
	{
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

	public PlayerPosition[] playersPosition;
	// Use this for initialization
	void Start () {
		playersPosition = new PlayerPosition[]{new PlayerPosition(6, 0), new PlayerPosition(0,7),
			new PlayerPosition(5, 22), new PlayerPosition(14,22), 
			new PlayerPosition(22, 16),new PlayerPosition(22,7)};
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool isDoor(int x1, int z1, int x2, int z2)
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

	public bool isValid(int playerNum, int px, int pz, int npx, int npz)
	{
		if (npx < 0 || npx > 22 || npz < 0 || npz > 22)
						return false;
		PlayerPosition player = playersPosition[playerNum];
		PlayerPosition newPlayerPosition = new PlayerPosition (npx, npz);

		foreach (PlayerPosition pp in playersPosition)
						if (pp.X == npx && pp.Z == npz)
								return false;

		int currentRoom = board [px, pz];
		int nextRoom = board [npx, npz];
		Debug.Log ("Current Room: " + currentRoom + " NewRoom: " + nextRoom);
		if (currentRoom != nextRoom && !isDoor (px, pz, npx, npz))
						return false;
		return true;

	}
}
