using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public GameObject[] pins;
    public Text scoreText;
    public Text messageText;

    private int totalScore = 0;
    private int throwCount = 0;           // 1 или 2 в текущем фрейме
    private int knockedInFrame = 0;       // сколько всего сбито за 2 броска
    private bool isStrike = false;
    private BallController ballController;
    private GameObject ball;
    private Vector3[] initialPinPositions;
    private Quaternion[] initialPinRotations;
    private Vector3 initialBallPosition;

    private bool throwProcessed = false;
    private float throwStartTime = 0f;

    void Start()
    {
        ball = GameObject.Find("Ball");
        ballController = ball.GetComponent<BallController>();
        initialBallPosition = ball.transform.position;

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
            // Таймер безопасности (на всякий случай)
            if (Time.time - throwStartTime > 6f)
            {
                ProcessThrow();
                return;
            }

            bool ballStopped = ball.GetComponent<Rigidbody>().velocity.magnitude < 0.15f;
            bool allPinsStopped = true;

            foreach (var pin in pins)
            {
                if (pin.GetComponent<Rigidbody>().velocity.magnitude > 0.15f)
                {
                    allPinsStopped = false;
                    break;
                }
            }

            if (ballStopped && allPinsStopped)
            {
                ProcessThrow();
            }
        }
    }

    private void ProcessThrow()
    {
        throwProcessed = true;
        throwCount++;

        int currentKnocked = 0;
        foreach (var pin in pins)
        {
            if (pin.GetComponent<Pin>().knocked) currentKnocked++;
        }

        int newKnockedThisThrow = currentKnocked - knockedInFrame;
        knockedInFrame = currentKnocked;

        if (throwCount == 1)
        {
            if (newKnockedThisThrow == 10)
            {
                isStrike = true;
                totalScore += 10;
                messageText.text = "STRIKE! (+10)";
                Invoke("ResetAll", 2f);
            }
            else
            {
                totalScore += newKnockedThisThrow;
                messageText.text = "First throw: " + newKnockedThisThrow;
                Invoke("ResetBallOnly", 1.5f);   // ← вот что было сломано!
            }
        }
        else if (throwCount == 2)
        {
            if (knockedInFrame == 10 && !isStrike)
            {
                totalScore += 10;
                messageText.text = "SPARE! (+10)";
            }
            else
            {
                totalScore += newKnockedThisThrow;
                messageText.text = "Second throw: " + newKnockedThisThrow;
            }
            Invoke("ResetAll", 2f);
        }

        UpdateUI();
    }

    private void ResetBallOnly()
    {
        ballController.ResetBall();
        ball.transform.position = initialBallPosition;
        ball.transform.rotation = Quaternion.identity;
        throwProcessed = false;
        throwStartTime = Time.time;
        ballController.thrown = false;
    }

    private void ResetAll()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            pins[i].transform.position = initialPinPositions[i];
            pins[i].transform.rotation = initialPinRotations[i];
            pins[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            pins[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            pins[i].GetComponent<Pin>().knocked = false;
        }

        ResetBallOnly();
        knockedInFrame = 0;
        throwCount = 0;
        isStrike = false;
        throwProcessed = false;
        messageText.text = "";
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + totalScore;
    }

    // Вызывается из BallController когда бросок начат
    public void OnThrowStarted()
    {
        throwStartTime = Time.time;
        throwProcessed = false;
    }
}