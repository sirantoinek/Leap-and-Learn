using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerField : MonoBehaviour
{
    // it will always be int, Validation type was set to int
    // https://discussions.unity.com/t/allow-only-numbers-to-a-textfield-and-make-it-an-int/126733/3
    public string inputAnswer;
    public string currentQuestion;
    public frogMovement frogmovement;
    public QuestionManager questionManager;
    public TMP_InputField answerInputField;
    public TextMeshProUGUI textDisplay;


    void Start() {

    }

    // this function reads the user input in the textbox UI and stores it in the inputAnswer variable
    public void ReadStringInput(string s) {
        
        inputAnswer = s;
        Debug.Log(inputAnswer);
        if (!string.IsNullOrEmpty(s) && questionManager.checkAnswer(int.Parse(s))) {
            // always clears the field and deactivates the inputfield to move
            answerInputField.text = "";
            answerInputField.DeactivateInputField();
            displayNewQuestion();
            frogmovement.hasMoved = false;
        }
        else {
            answerInputField.text = "";
            answerInputField.ActivateInputField();
        }
        
    }

    public void displayNewQuestion() {
        // generates new question
        questionManager.newQuestion();

        //display new question
        currentQuestion = questionManager.getQuestion();
        textDisplay.text = currentQuestion;

        // immediatly puts player curser into the TextField UI box
        answerInputField.ActivateInputField();
    }

}