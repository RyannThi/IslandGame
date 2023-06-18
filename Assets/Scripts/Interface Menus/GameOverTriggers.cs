using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameOverTriggers : EventTrigger, IPointerClickHandler
{
    private GameOverManager gameOverScreen;
    private void Awake()
    {
        gameOverScreen = FindObjectOfType<GameOverManager>();
    }
    public void ChangeSelectionIndex()
    {
        gameOverScreen.audioSource.PlayOneShot(gameOverScreen.buttonMove);
        switch (gameObject.name)
        {
            case "GameOverContinue":

                gameOverScreen.mainGroupSelectionIndex = 0;
                break;

            case "GameOverOptions":

                gameOverScreen.mainGroupSelectionIndex = 1;
                break;

            case "GameOverExitGame":

                gameOverScreen.mainGroupSelectionIndex = 2;
                break;

        }
    }
    public void ConfirmSelection()
    {
        if (gameOverScreen.mainGroupInteract == true)
        {
            gameOverScreen.mousePress = true;
        }
    }
}
