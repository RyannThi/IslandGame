using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseTriggers : EventTrigger, IPointerClickHandler
{
    private PauseManager pauseScreen;
    private void Awake()
    {
        pauseScreen = FindObjectOfType<PauseManager>();
    }
    public void ChangeSelectionIndex()
    {
        pauseScreen.audioSource.PlayOneShot(pauseScreen.buttonMove);
        switch (gameObject.name)
        {
            case "PauseContinue":

                pauseScreen.mainGroupSelectionIndex = 0;
                break;

            case "PauseOptions":

                pauseScreen.mainGroupSelectionIndex = 1;
                break;

            case "PauseExitGame":

                pauseScreen.mainGroupSelectionIndex = 2;
                break;

            // ----------------------- Options

            case "FullscreenSlider":

                pauseScreen.optionsGroupSelectionIndex = 0;
                break;

            case "ResolutionSlider":

                pauseScreen.optionsGroupSelectionIndex = 1;
                break;

            case "MusicSlider":

                pauseScreen.optionsGroupSelectionIndex = 2;
                break;

            case "SoundSlider":

                pauseScreen.optionsGroupSelectionIndex = 3;
                break;

            case "MouseSlider":

                pauseScreen.optionsGroupSelectionIndex = 4;
                break;

            case "SaveHeader":

                pauseScreen.optionsGroupSelectionIndex = 5;
                break;

        }
    }
    public void ConfirmSelection()
    {
        if (pauseScreen.mainGroupInteract == true)
        {
            pauseScreen.mousePress = true;
        }
        else if (pauseScreen.optionsGroupInteract == true)
        {
            pauseScreen.mousePress = true;
            pauseScreen.fullscreenText.text = pauseScreen.fullScreenNames[(int)pauseScreen.fullscreenSlider.value];
            pauseScreen.resolutionText.text = pauseScreen.resolutions[(int)pauseScreen.resolutionSlider.value, 0] + " x " + pauseScreen.resolutions[(int)pauseScreen.resolutionSlider.value, 1];
            pauseScreen.musicText.text = (int)(pauseScreen.musicSlider.value * 100) + "%";
            pauseScreen.soundText.text = (int)(pauseScreen.soundSlider.value * 100) + "%";
            pauseScreen.mouseText.text = pauseScreen.mouseSlider.value + "%";
        }
    }
}
