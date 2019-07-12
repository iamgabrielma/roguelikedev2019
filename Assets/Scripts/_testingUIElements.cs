using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _testingUIElements : MonoBehaviour
{
    public GameObject engine;
    public GameObject playerReference;
    private Engine engineMethod;

    //[Range(0, 1)]
    private float _outlinePercentage;

    public Transform statusLogBackgroundOutline;
    public Transform messageLogBackgroundOutline;

    public RectTransform pauseMenu;
    private bool isPauseMenuActive;

    public RectTransform levelUpMenu;
    private bool isLevelUpMenuActive;

    private enum levelUpOptions { hull, energy };

    //private string _guiText;
    //public Text gt;

    //public static MessageLog MessageLog { get; private set; }

    void Start()
    {
        if (engine == null)
        {
            engine = GameObject.Find("Engine");
        }
        if (playerReference == null)
        {
            playerReference = GameObject.Find("Player");
        }
        // UI styles
        CreateOutline();

        pauseMenu.gameObject.SetActive(false);
        levelUpMenu.gameObject.SetActive(false);
        isPauseMenuActive = false;
        isLevelUpMenuActive = false;


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
    // TODO: refactor ToggleLevelUpMenu() and ToggleMenu() into an unique method
    public void ToggleLevelUpMenu()
    {

        if (isLevelUpMenuActive == false)
        {
            levelUpMenu.gameObject.SetActive(true);
            isLevelUpMenuActive = true;
        }
        else
        {
            levelUpMenu.gameObject.SetActive(false);
            isLevelUpMenuActive = false;
        }

    }

    public void SaveGameButton()
    {
        // Saves game
        Debug.Log("Saved game");
        engineMethod = engine.gameObject.GetComponent<Engine>();
        engineMethod.SaveGame();

    }

    public void LoadGameButton()
    {
        Debug.Log("Loads game");
        engineMethod = engine.gameObject.GetComponent<Engine>();
        engineMethod.LoadGame();

    }

    public void UpgradeHullIntegrity() 
    {
        playerReference = FindPlayer();
        Entity.LevelUp(playerReference, levelUpOptions.hull.ToString());

    }
    public void UpgradeEnergy() {

        playerReference = FindPlayer();
        Entity.LevelUp(playerReference, levelUpOptions.energy.ToString());
    }

    void CreateOutline() {

        _outlinePercentage = 0.02f;
        statusLogBackgroundOutline.localScale = Vector3.one * (1 - _outlinePercentage);
        messageLogBackgroundOutline.localScale = Vector3.one * (1 - _outlinePercentage);
    }

    private GameObject FindPlayer() {

        if (playerReference == null)
        {
            playerReference = GameObject.Find("Player");
            return playerReference;
        }
        return playerReference;
    }

}
