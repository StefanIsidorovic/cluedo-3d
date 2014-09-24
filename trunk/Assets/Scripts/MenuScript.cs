using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

    private Vector2 scrollViewVector = Vector2.zero;
    private Rect dropDownRect = new Rect(0,0,125,300);
    public static string[] list = {"Menu", "About", "Exit"};
   
    int indexNumber;
    bool show = false;
    bool buttonClicked = false;
	private bool mute=false;
	private bool emute=false;
	private float volume=1.0f;

    private NetworkManager NetworkManagerScript;
    private GUIStyle MenuStyle;
    private GUIStyle ButtonStyle;
    private GUIStyle LabelStyle;
	private GUIStyle OnStyle;


    void InitGUIStyles()
    {
        MenuStyle = new GUIStyle(GUI.skin.box);
        MenuStyle.normal.background = (Texture2D)Resources.Load("menu/menuBar", typeof(Texture2D));

        ButtonStyle = new GUIStyle(GUI.skin.button);
        ButtonStyle.normal.background = (Texture2D)Resources.Load("menu/buttonNormal", typeof(Texture2D));
        ButtonStyle.hover.background = (Texture2D)Resources.Load("menu/buttonHover", typeof(Texture2D));
        ButtonStyle.active.background = ButtonStyle.normal.background;

        LabelStyle = new GUIStyle(GUI.skin.label);
        LabelStyle.normal.textColor = Color.black;
        LabelStyle.hover.textColor = Color.black;
        LabelStyle.alignment = TextAnchor.MiddleCenter;


		OnStyle = new GUIStyle (GUI.skin.toggle);
		OnStyle.fixedWidth = 30.0f;
		OnStyle.fixedHeight = 23.0f;
		OnStyle.imagePosition = ImagePosition.ImageOnly;
		OnStyle.margin = new RectOffset (0, 0, 0, 0);
		OnStyle.padding = new RectOffset (0,0,0,0);
		OnStyle.border = new RectOffset (0,0,0,0);
		OnStyle.normal.background = (Texture2D)Resources.Load("volume-up", typeof(Texture2D));
		OnStyle.hover.background =OnStyle.normal.background;
		OnStyle.active.background =OnStyle.normal.background;
		OnStyle.onActive.background =(Texture2D)Resources.Load("volume-muteR", typeof(Texture2D));
		OnStyle.onHover.background = OnStyle.onActive.background;
		OnStyle.onNormal.background = OnStyle.onActive.background;








    }
    void OnGUI()
    {
        InitGUIStyles();
		GUI.Box(new Rect(dropDownRect.x, dropDownRect.y, Screen.width, 25), "", MenuStyle);
		GUI.Label (new Rect (135, -2, 50, 30), "Music : ", LabelStyle);
		mute = GUI.Toggle (new Rect (190, 0, 30, 23), mute,"", OnStyle);
		Jukebox.Instance.audio.mute = mute;
		volume=GUI.HorizontalSlider (new Rect(230, 7, 100,30 ),volume, 0.0f, 1.0f);
		Jukebox.Instance.audio.volume = volume;
		GUI.Label (new Rect(350,-2,50,30),"Effects : ", LabelStyle);
		emute = GUI.Toggle (new Rect(410,0,30,23), emute,"",OnStyle);
		Sounds.Instance.audio.mute=emute;
		if (GUI.Button(new Rect(dropDownRect.x + 2, dropDownRect.y+2, dropDownRect.width, 21), "", ButtonStyle))
        {
            show = !show;
        }

        if (show)
        {
            scrollViewVector = GUI.BeginScrollView(new Rect(dropDownRect.x, (dropDownRect.y + 25), dropDownRect.width, dropDownRect.height), scrollViewVector, new Rect(0, 0, dropDownRect.width, list.Length * 25));

            GUI.Box(new Rect(0, 0, dropDownRect.width, list.Length * 25), "");

            for (int index = 0; index < list.Length; index++)
            {

                if (GUI.Button(new Rect(0, (index * 25), dropDownRect.height, 25), "", ButtonStyle))
                {
                    show = false;
                    indexNumber = index;
                    buttonClicked = true;
                }

                GUI.Label(new Rect(5, (index * 25), dropDownRect.width, 25), list[index], LabelStyle);

            }

            GUI.EndScrollView();
        }
        else
        {
            GUI.Label(new Rect(dropDownRect.x, dropDownRect.y, dropDownRect.width, 25), list[0], LabelStyle);
            if(buttonClicked)
            {
            switch (indexNumber) { 
                case 0:
                    break;
                case 1:
                    NetworkManagerScript.showTextAboutCluedo = !NetworkManagerScript.showTextAboutCluedo;
                    break;
                case 2:
                    Application.Quit();
                    break;
            }
            buttonClicked = false;
            }
        }
    }
	// Use this for initialization
	void Start () {
        NetworkManagerScript = GameObject.Find("NetworkManager").gameObject.GetComponent<NetworkManager>();


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
