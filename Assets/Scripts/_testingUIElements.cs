using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _testingUIElements : MonoBehaviour
{
    public GameObject engine;
    private Engine engineMethod;

    //[Range(0, 1)]
    private float _outlinePercentage;

    public Transform statusLogBackgroundOutline;
    public Transform messageLogBackgroundOutline;

    public RectTransform pauseMenu;
    private bool isPauseMenuActive;

    //private string _guiText;
    //public Text gt;

    //public static MessageLog MessageLog { get; private set; }

    void Start()
    {
        if (engine == null)
        {
            engine = GameObject.Find("Engine");
        }
        // UI styles
        CreateOutline();

        pauseMenu.gameObject.SetActive(false);
        isPauseMenuActive = false;


    }

    public void ToggleMenu() {

        if (isPauseMenuActive == false)
        {
            pauseMenu.gameObject.SetActive(true);
            isPauseMenuActive = true;
        }
        else
        {
            pauseMenu.gameObject.SetActive(false);
            isPauseMenuActive = false;
        }

    }

    public void SaveGameButton()
    {
        // Saves game
        Debug.Log("Saves game");
        engineMethod = engine.gameObject.GetComponent<Engine>();
        engineMethod.SaveGame();

    }

    public void LoadGameButton()
    {
        Debug.Log("Loads game");
        engineMethod = engine.gameObject.GetComponent<Engine>();
        engineMethod.LoadGame();

    }

    void CreateOutline() {

        _outlinePercentage = 0.02f;
        statusLogBackgroundOutline.localScale = Vector3.one * (1 - _outlinePercentage);
        messageLogBackgroundOutline.localScale = Vector3.one * (1 - _outlinePercentage);
    }

}
