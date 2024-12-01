using UnityEngine;
using UnityEngine.UI;

public class FrogHats : MonoBehaviour
{
    public string currentHat;
    public SpriteRenderer hatRenderer;
    public Sprite[] hatSprites;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        getCurrentHat();
        if(currentHat != null) {
            hatRenderer = GetComponent<SpriteRenderer>();
            int spriteIndex = findCurrentSprite();
            if (spriteIndex != -1)
            {
                hatRenderer.sprite = hatSprites[spriteIndex];
            }
        }
    }


    public void getCurrentHat() {
        if (PlayFabController.Instance != null) {
            currentHat = PlayFabController.Instance.GetCurrentHat();
        }
        else {
            currentHat = "null";
        }
        
       
    }
    public int findCurrentSprite() {
        for(int i = 0; i < hatSprites.Length; i ++) {
            if (hatSprites[i].name == currentHat) {
                return i;
            }
        }
        Debug.LogError("Uh-oh. Could not fidn Sprite Hat to put on frog. :(");
        return -1;
    }
}
