using UnityEngine;
using System;
using TMPro;

public class QuestionManager : MonoBehaviour
{    
    public AnswerField answerField;
    public TMP_InputField answerInputField;
    public TextMeshProUGUI textDisplay;
    //This is where the questions are created (multiplication currently)
    public int firstNumber;
    public int secondNumber;

    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textDisplay = GameObject.Find("AnswerTextUI").GetComponent<TextMeshProUGUI>();
        // finds the specific UI TextInput which allows us to actually use the input box in relation to movement
        answerInputField = GameObject.Find("AnswerInputField").GetComponent<TMP_InputField>();
        
    }


    public string getQuestion() {
        return firstNumber + " x " + secondNumber + " = ?";
    }

    public bool checkAnswer(int answerInput) {
        if (answerInput == (firstNumber*secondNumber)) {
            return true;
        }
        else {
            return false;
        }
    }

    public void newQuestion() {
        firstNumber = UnityEngine.Random.Range(0,13);
        secondNumber = UnityEngine.Random.Range(0,13);
    }
}
