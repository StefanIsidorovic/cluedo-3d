using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GUIScript : MonoBehaviour
{
    //***IMPORTANT: Read Unity3D GUI documentation before edit.
    // Hint: Every position of element is defined as Rect where first two values are upper left corner and second two values
    // are down right corner.

    // Help structures for Unity3D GUI elements (storing values and iterating through groups of elements aka type of cards)
    private List<string> rooms;
    private List<string> persons;
    private List<string> weapons;
    private Dictionary<string, bool> toogle;
    private Dictionary<string, string> textBoxes;
    private Vector2 scrollPosition = Vector2.zero;

    // helper variebles for defining Rectangles for positions of elements.
    // Height of space intended for dices.
    private int heightCoef = 0;
    // width of textbox.
    private int textBoxWidth = 160;

    // Use this for initialization
    void Start()
    {
        heightCoef = Percentage(Screen.height, 30);
        toogle = new Dictionary<string, bool>();
        textBoxes = new Dictionary<string, string>();
        //rooms
        rooms = new List<string>();
        rooms.Add("Biliard");
        rooms.Add("Kitchen");
        rooms.Add("Hall");
        rooms.Add("Studio");
        rooms.Add("Library");
        rooms.Add("Cabinet");
        rooms.Add("Guest Room");
        rooms.Add("Dining Room");
        rooms.Add("Sleeping Room");
        //persons
        persons = new List<string>();
        persons.Add("Black");
        persons.Add("Yellow");
        persons.Add("Red");
        persons.Add("Blue");
        persons.Add("White");
        persons.Add("Green");
        //weapons
        weapons = new List<string>();
        weapons.Add("Rope");
        weapons.Add("Wrench");
        weapons.Add("Knife");
        weapons.Add("Gun");
        weapons.Add("Candlestick");
        weapons.Add("Lead Pipe");

        foreach (var item in rooms)
        {
            toogle.Add(item, false);
            textBoxes.Add(item, "");
        }

        foreach (var item in persons)
        {
            toogle.Add(item, false);
            textBoxes.Add(item, "");
        }

        foreach (var item in weapons)
        {
            toogle.Add(item, false);
            textBoxes.Add(item, "");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    // Determine percentage of given value, primary for percentage of screen
    int Percentage(int value, int percent)
    {
        return (int)((double)value * ((double)percent / 100));
    }

    // GUI elements

    void OnGUI()
    {
        if (GameObject.Find("NetworkManager").gameObject.GetComponent<NetworkManager>().GameStarted())
            ShowSideBar();
    }

    private void ShowSideBar()
    {
        scrollPosition = GUI.BeginScrollView(
            new Rect(Percentage(Screen.width, 75), 0, Percentage(Screen.width, 25), Screen.height),
            scrollPosition,
            new Rect(0, 0, Percentage(Screen.width, 25) - 25, 21 * 20 + 60 + heightCoef + 10)
        );
        GUI.Box(new Rect(0, 0, Percentage(Screen.width, 25) - 25, 21 * 20 + 60 + heightCoef + 10), "BLAH");
        //generate elements
        GUI.Label(new Rect(0, 0 + heightCoef, 80, 20), "Rooms");
        int i = 0;
        foreach (var item in rooms)
        {
            toogle[item] = GUI.Toggle(new Rect(0, i * 20 + 20 + heightCoef, 110, 20), toogle[item], item);
            textBoxes[item] = GUI.TextField(new Rect(120, i * 20 + 20 + heightCoef, textBoxWidth, 20), textBoxes[item]);
            i++;
        }
        GUI.Label(new Rect(0, i * 20 + 20 + heightCoef, 80, 20), "Persons");
        foreach (var item in persons)
        {
            toogle[item] = GUI.Toggle(new Rect(0, i * 20 + 40 + heightCoef, 110, 20), toogle[item], item);
            textBoxes[item] = GUI.TextField(new Rect(120, i * 20 + 40 + heightCoef, textBoxWidth, 20), textBoxes[item]);
            i++;
        }
        GUI.Label(new Rect(0, i * 20 + 40 + heightCoef, 80, 20), "Weapons");
        foreach (var item in weapons)
        {
            toogle[item] = GUI.Toggle(new Rect(0, i * 20 + 60 + heightCoef, 110, 20), toogle[item], item);
            textBoxes[item] = GUI.TextField(new Rect(120, i * 20 + 60 + heightCoef, textBoxWidth, 20), textBoxes[item]);
            i++;
        }
        GUI.EndScrollView();

    }

    private void AskDialog_ChoosingCards()
    {
    }
}
