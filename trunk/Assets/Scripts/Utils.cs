using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Enum that represents each of 9 allRooms and hallway.
/// </summary>
public enum Rooms : int
{
    Studio = 1, Hall, GuestsRoom, SleepingRoom, DiningRoom, Cabinet, Kitchen, Billiard, Library, Hallway
};

/// <summary>
/// Enum that represents Cluedo allCharacters.
/// </summary>
public enum Characters : int
{
    MrsScarlet = 100, MrBlack, MrsBlue, MrGreen, MrYellow, MrsWhite
};

/// <summary>
/// Enum that represents Cluedo weapons.
/// </summary>
public enum Weapons : int
{
    Candlestick = 1000, Knife, LeadPipe, Revolver, Rope, Wrench
};

public class Pair<T1, T2>
{
    private T1 first;
    private T2 second;

    public Pair(T1 x, T2 y)
    {
        first = x;
        second = y;
    }

    public T1 First { get { return first; } set { first = value; } }
    public T2 Second { get { return second; } set { second = value; } }
}

public class Triple<T1, T2, T3>
{
    private T1 first;
    private T2 second;
    private T3 third;

    public Triple(T1 x, T2 y, T3 z)
    {
        first = x;
        second = y;
        third = z;
    }

    public T1 First { get { return first; } set { first = value; } }
    public T2 Second { get { return second; } set { second = value; } }
    public T3 Third { get { return third; } set { third = value; } }
}

public class Algorithms
{
    public static void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

/// <summary>
/// Manages conversions from enum to other types (only string currently).
/// </summary>
public class EnumConverter
{
    /// <summary>
    /// Converts Rooms, Weapons or Characters enum to string.
    /// </summary>
    /// <param name="toConvert">Enum to covert.</param>
    /// <returns>Returns string representation for enum. For invalid type returns null.</returns>
    public static string ToString(object toConvert)
    {
        if (toConvert is Rooms)
        {
            Rooms room = (Rooms)toConvert;
            switch (room)
            {
                case Rooms.Studio:
                    return "Studio";
                case Rooms.Hall:
                    return "Hall";
                case Rooms.GuestsRoom:
                    return "Guest room";
                case Rooms.SleepingRoom:
                    return "Sleeping room";
                case Rooms.DiningRoom:
                    return "Dining room";
                case Rooms.Cabinet:
                    return "Cabinet";
                case Rooms.Kitchen:
                    return "Kitchen";
                case Rooms.Billiard:
                    return "Billiard";
                case Rooms.Library:
                    return "Library";
                case Rooms.Hallway:
                    return "Hallway";
            }
        }
        else if (toConvert is Characters)
        {
            Characters person = (Characters)toConvert;
            switch (person)
            {
                case Characters.MrsScarlet:
                    return "Mrs. Scarlet";
                case Characters.MrBlack:
                    return "Mr. Black";
                case Characters.MrsBlue:
                    return "Mrs. Blue";
                case Characters.MrGreen:
                    return "Mr. Green";
                case Characters.MrYellow:
                    return "Mr. Yellow";
                case Characters.MrsWhite:
                    return "Mrs. White";
            }
        }
        else if (toConvert is Weapons)
        {
            Weapons weapon = (Weapons)toConvert;
            switch (weapon)
            {
                case Weapons.Candlestick:
                    return "Candlestick";
                case Weapons.Knife:
                    return "Knife";
                case Weapons.LeadPipe:
                    return "Lead pipe";
                case Weapons.Revolver:
                    return "Revolver";
                case Weapons.Rope:
                    return "Rope";
                case Weapons.Wrench:
                    return "Wrench";
            }
        }
        return null;
    }

    /// <summary>
    /// Converts int representation of Rooms, Characters or Weapons to string (these are enum types).
    /// </summary>
    /// <param name="toConvertInt">Integer representation of enum to be converted.</param>
    /// <returns>Returns string representation for given integer, or null if it isn't in any range (range of Rooms, Characters or Weapons).</returns>
    public static string ToString(int toConvertInt)
    {
        if (System.Enum.IsDefined(typeof(Rooms), toConvertInt))
        {
            return EnumConverter.ToString((Rooms)toConvertInt);
        }
        else if (System.Enum.IsDefined(typeof(Characters), toConvertInt))
        {
            return EnumConverter.ToString((Characters)toConvertInt);
        }
        else if (System.Enum.IsDefined(typeof(Weapons), toConvertInt))
        {
            return EnumConverter.ToString((Weapons)toConvertInt);
        }
        return null;
    }

    /// <summary>
    /// Converts given string to appropriate enum. 
    /// NOTE: Caller has responsibility to do proper cast to Rooms, Characters or Weapons enum type.
    ///       In case when the caller doesn't know to which type to cast they can use 'is' operator to test type
    ///       of returned object.
    /// </summary>
    /// <param name="toConvert">String to convert to enum.</param>
    /// <returns>Returns enum, constructed from given string, as generic object.</returns>
    public static object ToEnum(string toConvert)
    {
        switch (toConvert)
        {
            // Rooms
            case "Studio":
                return Rooms.Studio;
            case "Hall":
                return Rooms.Hall;
            case "Guest room":
                return Rooms.GuestsRoom;
            case "Sleeping room":
                return Rooms.SleepingRoom;
            case "Dining room":
                return Rooms.DiningRoom;
            case "Cabinet":
                return Rooms.Cabinet;
            case "Kitchen":
                return Rooms.Kitchen;
            case "Billiard":
                return Rooms.Billiard;
            case "Library":
                return Rooms.Library;
            case "Hallway":
                return Rooms.Hallway;
            
            // Characters
            case "Mrs. Scarlet":
                return Characters.MrsScarlet;
            case "Mr. Black":
                return Characters.MrBlack;
            case "Mrs. Blue":
                return Characters.MrsBlue;
            case "Mr. Green":
                return Characters.MrGreen;
            case "Mr. Yellow":
                return Characters.MrYellow;
            case "Mrs. White":
                return Characters.MrsWhite;

            // Weapons
            case "Candlestick":
                return Weapons.Candlestick;
            case "Knife":
                return Weapons.Knife;
            case "Lead pipe":
                return Weapons.LeadPipe;
            case "Revolver":
                return Weapons.Revolver;
            case "Rope":
                return Weapons.Rope;
            case "Wrench":
                return Weapons.Wrench;
        }
        return null;
    }
}