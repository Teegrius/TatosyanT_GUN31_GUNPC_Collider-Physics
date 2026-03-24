using UnityEngine;

public class Pin : MonoBehaviour
{
    public bool knocked = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!knocked)
        {
            // Надёжная проверка: кегля наклонена больше чем на ~45°
            if (Vector3.Dot(transform.up, Vector3.up) < 0.7f)
            {
                knocked = true;
            }
        }
    }
}