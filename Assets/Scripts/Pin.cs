using UnityEngine;

public class Pin : MonoBehaviour
{
    private Rigidbody rb;
    public bool knocked = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!knocked)
        {
            if (Mathf.Abs(transform.rotation.eulerAngles.x) > 30 || Mathf.Abs(transform.rotation.eulerAngles.z) > 30)
            {
                knocked = true;
            }
        }
    }
}