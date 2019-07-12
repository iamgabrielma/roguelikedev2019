using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{

    [SerializeField] private int currentLevel;
    [SerializeField] private int currentxp;
    [SerializeField] private int totalxp;
    [SerializeField] private int levelUpBase;
    [SerializeField] private float levelUpModifier;
    [SerializeField] private float xpNextLevel;

    void Start()
    {
        currentLevel = 1;
        currentxp = 0;
        totalxp = 0;
        levelUpBase = 10;
        levelUpModifier = 0.1f;

        xpNextLevel = 11;
    }

    public int CurrentLevel { get { return currentLevel; } }


    public void AddXP(int _xp)
    {
        currentxp = currentxp + _xp;
        CheckIfLevelUp(currentxp);
    }

    private float CalculateXPNextLevel()
    {
        xpNextLevel = levelUpBase * (currentLevel * (1+ levelUpModifier)) ;
        return xpNextLevel;
    }

    private void CheckIfLevelUp(int _currentXp)
    {
        if (_currentXp > xpNextLevel)
        {
            // Level up, reset the current xp, and calculate new xp required to level up again
            currentLevel++;
            totalxp = totalxp + currentxp;
            currentxp = 0;
            MessageLogManager.Instance.AddToQueue("Your battle skills grow stronger! You reached level " + currentLevel);
            CalculateXPNextLevel();
        }
    }

}
