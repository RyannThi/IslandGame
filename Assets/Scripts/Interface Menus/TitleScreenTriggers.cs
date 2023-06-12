using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleScreenTriggers : EventTrigger, IPointerClickHandler
{
    private TitleScreen titleScreen;
    private void Awake()
    {
        titleScreen = FindObjectOfType<TitleScreen>();
    }
    public void ChangeSelectionIndex()
    {
        titleScreen.audioSource.PlayOneShot(titleScreen.buttonMove);
        switch (gameObject.name)
        {
            case "LabelStart":

                titleScreen.mainGroupSelectionIndex = 0;
                break;

            case "LabelOptions":

                titleScreen.mainGroupSelectionIndex = 1;
                break;

            case "LabelCredits":

                titleScreen.mainGroupSelectionIndex = 2;
                break;

            case "LabelQuit":

                titleScreen.mainGroupSelectionIndex = 3;
                break;

            // ----------------------- Options

            //case "FullscreenHeader":
            //case "FullscreenText":
            case "FullscreenSlider":

                titleScreen.optionsGroupSelectionIndex = 0;
                break;

            //case "ResolutionHeader":
            //case "ResolutionText":
            case "ResolutionSlider":

                titleScreen.optionsGroupSelectionIndex = 1;
                break;

            //case "MusicHeader":
            //case "MusicText":
            case "MusicSlider":

                titleScreen.optionsGroupSelectionIndex = 2;
                break;

            //case "SoundHeader":
            //case "SoundText":
            case "SoundSlider":

                titleScreen.optionsGroupSelectionIndex = 3;
                break;

            //case "MouseHeader":
            //case "MouseText":
            case "MouseSlider":

                titleScreen.optionsGroupSelectionIndex = 4;
                break;

            case "SaveHeader":

                titleScreen.optionsGroupSelectionIndex = 5;
                break;

        }
    }
    public void ConfirmSelection()
    {
        if (titleScreen.mainGroupInteract == true && (gameObject == titleScreen.selections[0].gameObject || gameObject == titleScreen.selections[1].gameObject || gameObject == titleScreen.selections[2].gameObject || gameObject == titleScreen.selections[3].gameObject))
        {
            titleScreen.mousePress = true;
        }
        else if (titleScreen.optionsGroupInteract == true)
        {
            titleScreen.mousePress = true;
            titleScreen.fullscreenText.text = titleScreen.fullScreenNames[(int)titleScreen.fullscreenSlider.value];
            titleScreen.resolutionText.text = titleScreen.resolutions[(int)titleScreen.resolutionSlider.value, 0] + " x " + titleScreen.resolutions[(int)titleScreen.resolutionSlider.value, 1];
            titleScreen.musicText.text = (int)(titleScreen.musicSlider.value * 100) + "%";
            titleScreen.soundText.text = (int)(titleScreen.soundSlider.value * 100) + "%";
            titleScreen.mouseText.text = titleScreen.mouseSlider.value + "%";
        }
    }
}
