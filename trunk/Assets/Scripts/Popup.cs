
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Popup
{
    static int popupListHash = "PopupList".GetHashCode();
    // Delegate
    public delegate void ListCallBack();

    public static bool List<T>(Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent, IEnumerable<T> list, GUIStyle listStyle)
    {
        GUIStyle boxStyle = "box";
        GUIStyle buttonStyle = "button";
        int controlID = GUIUtility.GetControlID(popupListHash, FocusType.Passive);
        bool done = false;
        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.mouseDown:
                if (position.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl = controlID;
                    showList = true;
                }
                break;
            case EventType.mouseUp:
                if (showList)
                {
                    done = true;
                }
                break;
        }

     
        GUI.Label(position, buttonContent, buttonStyle);
        if (showList)
        {
            //Get type of object
            int startValue = 0;
            bool enums = false;
            // Get our list of strings
            if (typeof(Characters) == typeof(T))
            {
                startValue = EnumeratorConstants.CharacterStartValue;
                enums = !enums ? true : false;
            }
            if (typeof(Weapons) == typeof(T))
            {
                startValue = EnumeratorConstants.WeaponStartValue;
                enums = !enums ? true : false;
            }
            string[] text = new string[6];
            // convert to string
            int i = 0;
            foreach (var item in list)
            {
                if (enums)
                    text[i] = EnumConverter.ToString(item);
                else
                    text[i] = item.ToString();
                i++;
            }

            Rect listRect = new Rect(position.x, position.y, position.width, 6 * 20);
            GUI.Box(listRect, "", boxStyle);
            listEntry = GUI.SelectionGrid(listRect, listEntry, text, 1, listStyle) + startValue;
            
        }
        if (done)
        {
            showList = false;
        }

        return done;
    }
}