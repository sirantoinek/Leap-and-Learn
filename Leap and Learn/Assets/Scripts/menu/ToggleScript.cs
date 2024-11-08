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
            int lastToggle = i;
            toggles[i].onValueChanged.AddListener(delegate{EnsureAtLeastOneToggleOn(lastToggle);});
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
}
