using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreQuiz : MonoBehaviour
{
    public static CoreQuiz a;

    public ColorsData colors;

    public ColorItem choise;
    public AnswerItem[] answers;

    //Options
    public int questionsQ;

    //Inner
    int currentQuestion = -1;

    [System.Serializable]
    public struct AnswerItem {
        public bool isRight;
        public ColorItem color;
    }

    void Awake() {
        if (CoreQuiz.a == null)
            a = this;
    }

    void Start()
    {
        answers = new AnswerItem[questionsQ];
        AskQuestion();
    }

    void Update()
    {
        
    }

    public void AskQuestion() {
        CoreScene.a.ResetScene();
        currentQuestion++;
        if (currentQuestion < questionsQ) {
            CoreScene.a.ColorizeSpots();
            int choiseI = Random.Range(0, 3);
            choise = CoreScene.a.spots[choiseI].color;
            CoreScene.a.text.text = "Quel est le cube " + choise.name + "?";
            CoreScene.a.canChoose = true;
        } else {
            ShowResults();
        }
    }

    void ShowResults() {
        var results = AnswersData();
        CoreScene.a.text.text = "Oui: " + results.rightAnswers.ToString() + ", Non: " + results.wrongAnswers.ToString();
    }

    public bool CheckAnswer(CoreScene.Spot spot) {
        CoreScene.a.canChoose = false;
        bool isRight = spot.color.color == choise.color;
        answers[currentQuestion] = new AnswerItem() {
            isRight = isRight,
            color = spot.color
        };
        if (isRight) {
            Tween.a.MoveTo(spot.cube, CoreScene.a.lightCone.position, 3f, Easing.Ease.EaseInExpo);
        }
        Tween.a.DoAfter(3f, () => {
            AskQuestion();
        });
        return isRight;
    }

    (int rightAnswers, int wrongAnswers) AnswersData() {
        int right = 0;
        int wrong = 0;
        foreach (var a in answers) {
            if (a.isRight) {
                right++;
            } else {
                wrong++;
            }
        }
        return (right, wrong);
    }

    public ColorItem RandomColor {
        get {
            return colors.RandomColor;
        }
    }
}
