using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour {

    public Texture2D aimingReticle;
    public Font eightBit;
    public Material fontMat;

    private float hueCold = 255.0f / 360.0f;
    private HSBColor guiColor = new HSBColor(0.0f, 1.0f, 1.0f, 1.0f);
    private GUIStyle style = new GUIStyle();
    private CharacterEnergy scriptCharEnergy;
    private GameObject goCharacter;

    void Awake()
    {
        Screen.showCursor = false;
    }

	// Use this for initialization
	void Start () {
        goCharacter = GameObject.Find("Character");
        scriptCharEnergy = goCharacter.GetComponent<CharacterEnergy>();


        if (eightBit)
        {
            if (fontMat)
                eightBit.material = fontMat;
            else
                Debug.LogError("Font material not assigned!");
            style.font = eightBit;
        }
        else
            Debug.LogError("Font not assigned!");
        if (!aimingReticle)
            Debug.LogError("Aiming texture not assinged!");

	}
	
	// Update is called once per frame
    void OnGUI()
    {
        guiColor.h = Mathf.Abs(hueCold - (scriptCharEnergy.currentEnergy / 100.0f * hueCold));
        style.normal.textColor = HSBColor.ToColor(guiColor);
        GUI.Label(new Rect(25, 25, 100, 30), scriptCharEnergy.currentEnergy.ToString("f1") + "°", style);
        GUI.Label(new Rect(Screen.width / 2 - aimingReticle.width / 2, Screen.height / 2 - aimingReticle.height / 2, aimingReticle.width, aimingReticle.height), aimingReticle);
    }
}
