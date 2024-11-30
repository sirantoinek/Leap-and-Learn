using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public int coins;
    //the coin txt that will change the number of coins visible in the shop
    public TMP_Text coinUI;
    //current Frog and Hat
    public string currentFrog = "Green";
    public string currentHat = null;
    //array of shop items the SCRIPTABLE OBJECTS
    public ShopItemSO[] shopFrogsSO;
    public ShopItemSO[] shopHatsSO;

    // reference the GAME OBJECT for the panels
    public GameObject[] shopFrogsGO;
    public GameObject[] shopHatsGO;
    // reference the TEMPLATE SCRIPT for the panels
    public ShopTemplate[] shopFrogsPanels;
    public ShopTemplate[] shopHatsPanels;

    //a list  of the buttons for when we want to turn the off or on
    public Button[] myPurchaseFrogsBtns;
    public Button[] myPurchaseHatsBtns;
    //Player account dictionaries
    public Dictionary<string, bool> playerFrogsDict = new Dictionary<string, bool>();
    public Dictionary<string, bool> playerHatsDict = new Dictionary<string, bool>();
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //these for loop just makes sure that if there are more panels than frogs/hats that it doesn't add the blank ones
        for (int i = 0; i < shopFrogsSO.Length; i++) {
            shopFrogsGO[i].SetActive(true);
        }
        for (int i = 0; i < shopHatsSO.Length; i++) {
            shopHatsGO[i].SetActive(true);
        }
        GetPlayerCoinData();
        GetPlayerSkinHatData();
        coinUI.text = "Coins: " + coins.ToString();
        LoadPanels();
        LoadDicts();
        CheckPurchaseable();
    }

    //for testing purposes, uses button to add 20 coins
    public void AddCoins() {
        coins += 20;
        coinUI.text = "Coins: " + coins.ToString();
        CheckPurchaseable();
    }
    //loading coins from player data
    public void GetPlayerCoinData() {
        if (PlayFabController.Instance != null) {
            coins = PlayFabController.Instance.GetCoins();
            coinUI.text = "Coins: " + coins.ToString();
        }
        else {
            Debug.LogError("No player instance found");
            coins = 0;
        }
    }
    public void GetPlayerSkinHatData() {
        if (PlayFabController.Instance != null) {
            currentFrog = PlayFabController.Instance.GetCurrentSkin();
            currentHat = PlayFabController.Instance.GetCurrentHat();
        }
    }
    public void LoadPanels() {
        for (int i = 0; i < shopFrogsSO.Length; i++) {
            shopFrogsPanels[i].titleTxt.text = shopFrogsSO[i].title;
            shopFrogsPanels[i].descriptionTxt.text = shopFrogsSO[i].description;
            shopFrogsPanels[i].costTxt.text = shopFrogsSO[i].baseCost.ToString();
            shopFrogsPanels[i].itemImage.sprite = shopFrogsSO[i].mySprite;
            if (shopFrogsSO[i].title == "Green") {
                shopFrogsPanels[i].itemImage.color = Color.green;
            }
            else if (shopFrogsSO[i].title == "Red") {
                shopFrogsPanels[i].itemImage.color = Color.red;
            }
            else if (shopFrogsSO[i].title == "Black") {
                shopFrogsPanels[i].itemImage.color = Color.blue;
            }
            else if (shopFrogsSO[i].title == "Yellow") {
                shopFrogsPanels[i].itemImage.color = Color.yellow;
            }
            else {
                Debug.LogError("No currentFrog for some reason.");
            }
        }
        for (int i = 0; i < shopHatsSO.Length; i++) {
            shopHatsPanels[i].titleTxt.text = shopHatsSO[i].title;
            shopHatsPanels[i].descriptionTxt.text = shopHatsSO[i].description;
            shopHatsPanels[i].costTxt.text = shopHatsSO[i].baseCost.ToString();
            shopHatsPanels[i].itemImage.sprite = shopHatsSO[i].mySprite;
        }
    }
    //this function laods the two dictionaries with the player data
    public void LoadDicts() {
        if (PlayFabController.Instance != null) {
            playerFrogsDict = PlayFabController.Instance.GetSkins();
            playerHatsDict = PlayFabController.Instance.GetHats();
            foreach(string key in playerFrogsDict.Keys) {
                Debug.Log($"Checking key: '{key}'");
            }
            //checks to see if all frog in the SO array are in the player data, if not, it is added
            foreach(ShopItemSO frogSkin in shopFrogsSO) {
                Debug.Log($"Checking key: '{frogSkin.title}'");
                Debug.Log($"Checking key: '{playerFrogsDict.ContainsKey("Black")}'");
                if (playerFrogsDict.ContainsKey(frogSkin.title) == false) {
                    playerFrogsDict.Add(frogSkin.title, false);
                }
            }
            foreach(ShopItemSO hatSkin in shopHatsSO) {
                if (playerHatsDict.ContainsKey(hatSkin.title) == false) {
                    playerHatsDict.Add(hatSkin.title, false);
                }
            }
        }
        else {
            playerFrogsDict = new Dictionary<string, bool>();
            playerHatsDict = new Dictionary<string, bool>();
            //checks to see if all frog in the SO array are in the player data, if not, it is added
            foreach(ShopItemSO frogSkin in shopFrogsSO) {
                if (!playerFrogsDict.ContainsKey(frogSkin.title)) {
                    playerFrogsDict.Add(frogSkin.title, false);
                }
            }
            foreach(ShopItemSO hatSkin in shopHatsSO) {
                if (!playerHatsDict.ContainsKey(hatSkin.title)) {
                    playerHatsDict.Add(hatSkin.title, false);
                }
            }
        }
        playerFrogsDict["Green"] = true;
        //shows whats in the dictionary
        foreach(var frogSkin in playerFrogsDict) {
            Debug.LogError($"ID: {frogSkin.Key}, Bool: {frogSkin.Value}");
        }
        foreach(var HatSkin in playerHatsDict) {
            Debug.LogError($"ID: {HatSkin.Key}, Bool: {HatSkin.Value}");
        }
    }
    public void OffloadFrogData() {
        PlayFabController.Instance.SetSkins(playerFrogsDict);
        PlayFabController.Instance.SetCurrentSkin(currentFrog);
        
    }
    public void OffloadHatData() {
        PlayFabController.Instance.SetSkins(playerHatsDict);
        PlayFabController.Instance.SetCurrentHat(currentHat);
    }

    //This function checks whether an item can be purchased or not and makes it to where
    // the button will flip off if they can't
    public void CheckPurchaseable() {
        for (int i = 0; i < shopFrogsSO.Length; i++) {
            //if you have enough money AND if it's not already bought
            if (coins >= shopFrogsSO[i].baseCost && (playerFrogsDict[shopFrogsSO[i].title] == false)) {
                myPurchaseFrogsBtns[i].interactable = true;
            }
            // if it's already bought we want to change the "purchase" to "Equip" bc its already bought
            else if (playerFrogsDict[shopFrogsSO[i].title] == true) {
                myPurchaseFrogsBtns[i].interactable = true;
                TMP_Text frogBtnText = myPurchaseFrogsBtns[i].GetComponentInChildren<TMP_Text>();
                //unless its the current frog, then obviously it needs to be "unequip"
                if(shopFrogsSO[i].title == currentFrog) {
                    frogBtnText.text = "Equipped";
                }
                else {
                    frogBtnText.text = "Equip";
                }
            }
            else {
                myPurchaseFrogsBtns[i].interactable = false;
            }
        }
        for (int i = 0; i < shopHatsSO.Length; i++) {
            //if you have enough money AND if it's not already bought
            if (coins >= shopHatsSO[i].baseCost && (playerHatsDict[shopHatsSO[i].title] == false)) {
                myPurchaseHatsBtns[i].interactable = true;
                
            }
            // if it's already bought we want to change the "purchase" to "Equip" bc its already bought
            else if (playerHatsDict[shopHatsSO[i].title] == true) {
                myPurchaseHatsBtns[i].interactable = true;
                TMP_Text hatBtnText = myPurchaseHatsBtns[i].GetComponentInChildren<TMP_Text>();
                if(shopHatsSO[i].title == currentHat) {
                    hatBtnText.text = "Unequip";
                }
                else {
                    hatBtnText.text = "Equip";
                }
            }
            else {
                myPurchaseHatsBtns[i].interactable = false;
            }
        }
    }

    //purchase a frog
    public void FrogButtonPressed(int BtnNum) {
        string frogName = shopFrogsSO[BtnNum].title;
        TMP_Text frogBtnText = myPurchaseFrogsBtns[BtnNum].GetComponentInChildren<TMP_Text>();
        if(frogBtnText.text == "Purchase") {
            if (coins >= shopFrogsSO[BtnNum].baseCost && playerFrogsDict[frogName] == false) {
                coins -= shopFrogsSO[BtnNum].baseCost;
                coinUI.text = "Coins: " + coins.ToString();
                playerFrogsDict[frogName] = true;
                //Debug.LogError("Purcahsed " + frogName);
                frogEquip(frogName);
                //Lock/unlock items
                CheckPurchaseable();
            }
        }
        else if (frogBtnText.text == "Equip") {
            frogEquip(frogName);
        }
        else if (frogBtnText.text == "Equipped") {
            // Do nothing
        }
        else {
            Debug.LogError("Something went wrong with the button pressing :(");
        }
        if (PlayFabController.Instance != null) {
            PlayFabController.Instance.SetCoins(coins);
            OffloadFrogData();
        }
        
    }
    //purchase a hat
    public void HatButtonPressed(int BtnNum) {
        string hatName = shopHatsSO[BtnNum].title;
        TMP_Text hatBtnText = myPurchaseHatsBtns[BtnNum].GetComponentInChildren<TMP_Text>();
        if (hatBtnText.text == "Purchase") {
            if (coins >= shopHatsSO[BtnNum].baseCost && playerHatsDict[hatName] == false) {
                coins -= shopHatsSO[BtnNum].baseCost;
                coinUI.text = "Coins: " + coins.ToString();
                playerHatsDict[hatName] = true;
                hatEquip(hatName);
                //Lock/unlock items
                CheckPurchaseable();
            }
        }
        else if (hatBtnText.text == "Equip") {
            hatEquip(hatName);
        }
        else if (hatBtnText.text == "Unequip") {
            hatUnequip();
        }
        else {
            Debug.LogError("Something went wrong with the button pressing :(");
        }
        if (PlayFabController.Instance != null) {
            PlayFabController.Instance.SetCoins(coins);
            OffloadHatData();
        }
        
    }

    //there is no unequipping the frog bc we alwasy need a frog skin, and that default is green, however we can unequip the hat if need be
    public void frogEquip(string frogName) {
        currentFrog = frogName;
        //resets all the button values
        CheckPurchaseable();
    }

    public void hatEquip(string hatName) {
        currentHat = hatName;
        //resets all the button values
        CheckPurchaseable();
    }

    public void hatUnequip() {
        currentHat = null;
        //resets all the button values
        CheckPurchaseable();
    }

}

