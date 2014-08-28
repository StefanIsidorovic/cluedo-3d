using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GUIScript : MonoBehaviour
{
    //***IMPORTANT: Read Unity3D GUI documentation before edit.
    // Hint: Every position of element is defined as Rect where first two values are upper left corner and second two values
    // are down right corner.

    // Game manager instance
    private GameManager gameManager;
    private BoardScript board;

    // Help structures for Unity3D GUI elements (storing values and iterating through groups of elements aka type of cards)
    private Dictionary<string, bool> toogle;
    private Dictionary<string, string> textBoxes;
    private Vector2 scrollPosition = Vector2.zero;

    // helper variebles for defining Rectangles for positions of elements.
    // Height of space intended for dices.
    private int heightCoef = 0;
    // width of textbox.
    private int textBoxWidth = 160;

    void Start()
    {
        gameManager = MonoSingleton<GameManager>.Instance;
        board = MonoSingleton<BoardScript>.Instance;

        heightCoef = Percentage(Screen.height, 30);
        toogle = new Dictionary<string, bool>();
        textBoxes = new Dictionary<string, string>();
        foreach (var item in gameManager.AllRooms())
        {
            string strItem = EnumConverter.ToString(item);
            toogle.Add(strItem, false);
            textBoxes.Add(strItem, "");
        }

        foreach (var item in gameManager.AllWeapons())
        {
            string strItem = EnumConverter.ToString(item);
            toogle.Add(strItem, false);
            textBoxes.Add(strItem, "");
        }
        foreach (var item in gameManager.AllCharacters())
        {
            string strItem = EnumConverter.ToString(item);            
            toogle.Add(strItem, false);
            textBoxes.Add(strItem, "");
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
        foreach (var item1 in gameManager.AllRooms())
        {
            string item = EnumConverter.ToString(item1);
            toogle[item] = GUI.Toggle(new Rect(0, i * 20 + 20 + heightCoef, 110, 20), toogle[item], item);
            textBoxes[item] = GUI.TextField(new Rect(120, i * 20 + 20 + heightCoef, textBoxWidth, 20), textBoxes[item]);
            i++;
        }
        GUI.Label(new Rect(0, i * 20 + 20 + heightCoef, 80, 20), "Persons");
        foreach (var item1 in gameManager.AllCharacters())
        {
            string item = EnumConverter.ToString(item1);
            toogle[item] = GUI.Toggle(new Rect(0, i * 20 + 40 + heightCoef, 110, 20), toogle[item], item);
            textBoxes[item] = GUI.TextField(new Rect(120, i * 20 + 40 + heightCoef, textBoxWidth, 20), textBoxes[item]);
            i++;
        }
        GUI.Label(new Rect(0, i * 20 + 40 + heightCoef, 80, 20), "Weapons");
        foreach (var item1 in gameManager.AllWeapons())
        {
            string item = EnumConverter.ToString(item1);
            toogle[item] = GUI.Toggle(new Rect(0, i * 20 + 60 + heightCoef, 110, 20), toogle[item], item);
            textBoxes[item] = GUI.TextField(new Rect(120, i * 20 + 60 + heightCoef, textBoxWidth, 20), textBoxes[item]);
            i++;
        }
        GUI.EndScrollView();

    }

    private void AskDialog_ChoosingCards()
    {
    }

    #region HelperMethods

    #endregion
}
