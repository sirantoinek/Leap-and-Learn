using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{    
    public AnswerField answerField;
    public TMP_InputField answerInputField;
    public TextMeshProUGUI textDisplay;
    //operation toggles
    public bool isAddOn = false;
    public bool isSubtractOn = false;
    public bool isMultiplyOn = false;
    public bool isDivideOn = false;
    public List<int> operationsList;
    //This is where the questions are created
    public int firstNumber;
    public int secondNumber;
    public int answer;
    public int currentOperation;


    //check what the staus is of the buttons before calling start and initalize them
    public void Awake() {
        //toggle list is multiplication, division, addition, then subtraction
        int isMultToggleOn = PlayerPrefs.GetInt("Toggle0", 0);
        int isDivToggleOn = PlayerPrefs.GetInt("Toggle1", 0);
        int isAddToggleOn = PlayerPrefs.GetInt("Toggle2", 0);
        int isSubToggleOn = PlayerPrefs.GetInt("Toggle3", 0);
        if(isAddToggleOn == 1) {isAddOn = true;}
        if(isSubToggleOn == 1) {isSubtractOn = true;}
        if(isMultToggleOn == 1) {isMultiplyOn = true;}
        if(isDivToggleOn == 1) {isDivideOn = true;}
        operationsList = getOperationsList();
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textDisplay = GameObject.Find("AnswerTextUI").GetComponent<TextMeshProUGUI>();
        // finds the specific UI TextInput which allows us to actually use the input box in relation to movement
        answerInputField = GameObject.Find("AnswerInputField").GetComponent<TMP_InputField>();
        
    }


    public List<int> getOperationsList() {
        List<int> operations = new List<int>();
        if (isAddOn) {
            Debug.Log("Addition enabled");
            operations.Add(1);
        }
        if (isSubtractOn) {
            Debug.Log("Subtraaction enabled");
            operations.Add(2);
        }
        if (isMultiplyOn) {
            Debug.Log("Multiply enabled");
            operations.Add(3);
        }
        if (isDivideOn) {
            Debug.Log("Divide enabled");
            operations.Add(4);
        }
        //for debug
        Debug.Log("Operations in List: ");
        foreach(var op in operations) {
            Debug.Log(op);
        }

        return operations;
    }
    public string getQuestion() {
        if (currentOperation == 1) {
            return firstNumber + " + " + secondNumber + " = ?";
        }
        if (currentOperation == 2) {
            return firstNumber + " - " + secondNumber + " = ?";
        }
        if (currentOperation == 3) {
            return firstNumber + " x " + secondNumber + " = ?";
        }
        if (currentOperation == 4) {
            return firstNumber + " \u00F7 " + secondNumber + " = ?";
        }
        return "Unknown Operation";
    }
    public bool checkAnswer(int answerInput) {
        if(answerInput == answer) {
            return true;
        }
        return false;
    }
    public void newQuestion() {
        //randomize the operation from selection
        //operation 1 is add, 2 substract, 3 mltiple, 4 divide
        currentOperation = operationsList[UnityEngine.Random.Range(0, operationsList.Count)];

        //add
        if(currentOperation == 1) {
            firstNumber = UnityEngine.Random.Range(0,11);
            secondNumber = UnityEngine.Random.Range(0,11);
            answer = firstNumber + secondNumber;
        }
        //subtract
        else if (currentOperation == 2) {
            firstNumber = UnityEngine.Random.Range(0,11);
            while (secondNumber > firstNumber) {
                secondNumber = UnityEngine.Random.Range(0,11);
            }
            answer = firstNumber - secondNumber;
        }
        //multiply
        else if (currentOperation == 3) {
            firstNumber = UnityEngine.Random.Range(0,13);
            secondNumber = UnityEngine.Random.Range(0,13);
            answer = firstNumber * secondNumber;
        }
        //divide
        else if (currentOperation == 4) {
            //for divide we want even numbers, so the first nmber will end up being the answer to the corresponding multiplication question, and the secodn and answer will be the digits that multiply to get the first
            /* example:
            * 4 x 3 = 12
            * 4(firstNumber) x 3(secondNumber) = 12(answer)
            * division problem:
            * 12(answer) / 3(secondNumber) = 4(firstNumber)
            * 12 / 3 = 4
            */
            //set it up like a normal muliplication question
            secondNumber = UnityEngine.Random.Range(1,13);
            //cannot divide by zero
            answer = UnityEngine.Random.Range(1,13);
            firstNumber = secondNumber * answer;
        }
        
    }
}
