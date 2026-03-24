using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 mouseStartPos;
    private bool isDragging = false;
    public float throwForce = 8f;
    public float sideForce = 4f;
    public bool thrown = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (thrown) return;
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            mouseStartPos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            Vector3 mouseEndPos = Input.mousePosition;
            Vector3 delta = mouseEndPos - mouseStartPos;
            float power = (mouseStartPos.y - mouseEndPos.y) * 0.04f;
            float side = delta.x * 0.05f;
            Vector3 force = new Vector3(side * sideForce, 0, power * throwForce);
            rb.AddForce(force, ForceMode.Impulse);
            thrown = true;
            ScoreManager scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
scoreManager.OnThrowStarted();
            
    
        }
    }

    public void ResetBall()
    {
        thrown = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}