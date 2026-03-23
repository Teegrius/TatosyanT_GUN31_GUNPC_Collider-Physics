using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public GameObject[] pins;
    public Text scoreText;
    public Text messageText;
    private int totalScore = 0;
    private int throwCount = 0;
    private int previousKnocked = 0;
    private bool isStrike = false;
    private BallController ballController;
    private GameObject ball;
    private Vector3[] initialPinPositions;
    private Quaternion[] initialPinRotations;
    private bool throwProcessed = false;

    void Start()
    {
        ball = GameObject.Find("Ball");
        ballController = ball.GetComponent<BallController>();
        pins = GameObject.FindGameObjectsWithTag("Pin");
        initialPinPositions = new Vector3[pins.Length];
        initialPinRotations = new Quaternion[pins.Length];
        for (int i = 0; i < pins.Length; i++)
        {
            initialPinPositions[i] = pins[i].transform.position;
            initialPinRotations[i] = pins[i].transform.rotation;
        }
        UpdateUI();
    }

    void Update()
    {
        if (ballController.thrown && !throwProcessed)
        {
            if (ball.GetComponent<Rigidbody>().velocity.magnitude < 0.5f)
            {
                bool allStopped = true;
                foreach (var pin in pins)
                {
                    if (pin.GetComponent<Rigidbody>().velocity.magnitude > 0.5f) { allStopped = false; break; }
                }
                if (allStopped)
                {
                    ProcessThrow();
                    throwProcessed = true;
                }
            }
        }
    }

    private void ProcessThrow()
    {
        throwCount++;
        int currentKnocked = 0;
        foreach (var pin in pins)
        {
            if (pin.GetComponent<Pin>().knocked) currentKnocked++;
        }
        int newKnocked = currentKnocked - previousKnocked;
        previousKnocked = currentKnocked;

        if (throwCount == 1)
        {
            if (newKnocked == 10)
            {
                isStrike = true;
                totalScore += 10;
                messageText.text = "Strike! (+10)";
            }
            else
            {
                totalScore += newKnocked;
                messageText.text = "First throw: " + newKnocked;
            }
        }
        else if (throwCount == 2)
        {
            if (currentKnocked == 10 && !isStrike)
            {
                totalScore += 10;
                messageText.text = "Spare! (+10)";
            }
            else
            {
                totalScore += newKnocked;
                messageText.text = "Second throw: " + newKnocked;
            }
        }
        UpdateUI();

        if (throwCount >= 2 || isStrike)
        {
            Invoke("ResetGame", 2f);
        }
    }

    private void ResetGame()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            pins[i].transform.position = initialPinPositions[i];
            pins[i].transform.rotation = initialPinRotations[i];
            pins[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            pins[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            pins[i].GetComponent<Pin>().knocked = false;
        }
        ball.transform.position = new Vector3(0, 0.5f, -8f);
        ball.transform.rotation = Quaternion.identity;
        ballController.ResetBall();
        previousKnocked = 0;
        throwCount = 0;
        isStrike = false;
        throwProcessed = false;
        messageText.text = "";
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + totalScore;
    }
}