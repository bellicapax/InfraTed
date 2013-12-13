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



        StartCoroutine(ChangeColors());
    }

    void OnGUI()
    {
        //startHW = GUILayoutUtility.GetRect(new GUIContent("Start"), startExitStyle);
        //exitHW = GUILayoutUtility.GetRect(new GUIContent("Exit"), startExitStyle);

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
