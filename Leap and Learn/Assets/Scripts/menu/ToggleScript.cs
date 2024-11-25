using UnityEngine.UI;
using UnityEngine;

public class ToggleScript : MonoBehaviour
{
    public Toggle[] toggles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Listen to the toggles for changes
        for(int i = 0; i < toggles.Length; i++)
        {
            //loads persistant data
            int isToggleOn = PlayerPrefs.GetInt($"Toggle{i}", 0);
            if(isToggleOn == 1) {
                toggles[i].isOn = true;
            }
            else {
                toggles[i].isOn = false;
            }

            //checks state of toggle
            if(toggles[i].isOn) {
                isToggleOn = 1;
            }
            int lastToggle = i;
            toggles[i].onValueChanged.AddListener(delegate
            {
                EnsureAtLeastOneToggleOn(lastToggle);
                //save the sate for when toggle changes
                SaveToggleState(lastToggle);
            });
        }
    }

    // Prevents final toggle from being turned off
    void EnsureAtLeastOneToggleOn(int lastToggle)
    {
        bool allOff = true;
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                allOff = false;
                break;
            }
        }
        if (allOff) toggles[lastToggle].isOn = true;
    }

    //saves the toggle's state to player refs for persistant data
    void SaveToggleState(int toggleIndex) {
        if (toggles[toggleIndex].isOn) {
            PlayerPrefs.SetInt($"Toggle{toggleIndex}", 1);
        }
        else {
            PlayerPrefs.SetInt($"Toggle{toggleIndex}", 0);
        }
        PlayerPrefs.Save();
    }
}
