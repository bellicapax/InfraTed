using UnityEngine;
using System.Collections;

public class StartScreenGUI : MonoBehaviour {

    public float secondsBetweenHues = 0.1f;
    public Font interGalatic;
    public Rect infraRect;
    public Rect startRect;
    public Rect exitRect;

    private Rect startHW;
    private Rect exitHW;
    private int myHue = 0;
    private Color infraColor;
    private GUIStyle infraStyle = new GUIStyle();
    private GUIStyle startExitStyle = new GUIStyle();

    void Start()
    {
        infraColor = Color.red;
        infraStyle.font = interGalatic;
        infraStyle.fontSize = 90;
        infraStyle.normal.textColor = infraColor;

        startExitStyle.font = interGalatic;
        startExitStyle.fontSize = 64;
        startExitStyle.normal.textColor = Color.white;

        infraRect.x = (Screen.width / 2) - infraRect.width/2;

        startRect.x = Screen.width / 3 - startRect.width/2;
        startRect.y = Screen.height / 3 + startRect.height/2;

        exitRect.x = Screen.width / 3 - exitRect.width/2;
        exitRect.y = Screen.height * (2.0f/3.0f) + exitRect.height/2;

        StartCoroutine(ChangeColors());
    }

    void OnGUI()
    {
        //startHW = GUILayoutUtility.GetRect(new GUIContent("Start"), startExitStyle);
        //exitHW = GUILayoutUtility.GetRect(new GUIContent("Exit"), startExitStyle);
        //startHW = GUILayoutUtility.GetRect(new GUIContent("Infra-Ted"), infraStyle);
       
        //infraRect.width = startHW.width;
        //infraRect.height = startHW.height;

        //startRect.height = startHW.height;
        //startRect.width = startHW.width;
        //exitRect.height = exitHW.height;
        //exitRect.width = exitHW.width;

        GUI.Label(infraRect, "Infra-Ted", infraStyle);
        if(GUI.Button(startRect, "Start", startExitStyle))
        {
            Application.LoadLevel("FirstLevelRemake");
        }
        if(GUI.Button(exitRect, "Exit", startExitStyle))
        {
            Application.Quit();
        }
    }

    IEnumerator ChangeColors()
    {
        while (true)
        {
            if(myHue < 360)
                myHue++;
            else
                myHue = 0;

            infraColor.H(myHue, ref infraColor);
            infraStyle.normal.textColor = infraColor;
            yield return new WaitForSeconds(secondsBetweenHues);
        }
    }
}
