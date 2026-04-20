using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// Specialized Manager for tutorial UI. 
// Uses data structures to map strings to Unity objects for easy designer access.
public class TutorialUIManager : InstanceBaseClass<TutorialUIManager>
{
    [SerializeField] private List<UI_Elements> ui_elements;
    [SerializeField] private List<UI_Buttons> ui_buttons;
    [SerializeField] private List<PlayerData> player;
    [SerializeField] private List<CheckBoxFill> checkBoxFills;
    [SerializeField] private List<TutorialPopup> tutorialPopups;

    public void ShowPopup(string name, string text) => SetPopupState(name, true, text);
    public void HidePopup(string name) => SetPopupState(name, false);

    [System.Serializable]
    public struct TutorialPopup
    {
        public string popupName; 
        public GameObject popupObject;
        public TextMeshProUGUI textElement;

    }
        [System.Serializable]
    public struct UI_Elements
    {
        public string name;
        public GameObject uiObject;
    }

    [System.Serializable]
    public struct UI_Buttons
    {
        public string buttonName;
        public Button buttonObject;
    }

    [System.Serializable]
    public struct PlayerData
    {
        public string playerName;
        public GameObject playerObject;
    }

    [System.Serializable]
    public struct CheckBoxFill
    {
        public string name;
        public GameObject uiObject;
    }

    public GameObject GetPlayer(string name, bool enable)
    {
        foreach (var p in player)
        {
            if (p.playerName == name)
            {
                p.playerObject.SetActive(enable);
                return p.playerObject;
            }
            
        }
        return null;
    }


    public Button GetButton(string name)
    {
        foreach (var button in ui_buttons)
        {
            if (button.buttonName == name)
            {
                return button.buttonObject;
            }
        }
        return null;
    }

    public void ShowUIElement(string name)
    {
        foreach (var element in ui_elements)
        {
            if (element.name == name)
            {
                element.uiObject.SetActive(true);
            }
        }
    }

    public void HideUIElement(string name)
    {
        foreach (var element in ui_elements)
        {
            if (element.name == name)
            {
                element.uiObject.SetActive(false);
            }
        }
    }

    // Checks if a specific task UI element is currently enabled/completed.
    public bool CheckFill(string name)
    {
        foreach (var fill in checkBoxFills)
        {
            if (fill.name == name)
            {
                return fill.uiObject.activeSelf;
            }
        }

        return false;
    }

    // Generic method to manage popup visibility and dynamic text content.
    public void SetPopupState(string name, bool isActive, string content = "")
    {
        foreach (var popup in tutorialPopups)
        {
            if (popup.popupName == name)
            {
                popup.popupObject.SetActive(isActive);

                if (isActive && popup.textElement != null)
                {
                    popup.textElement.text = content;
                }
                return; 
            }
        }
    }
}
