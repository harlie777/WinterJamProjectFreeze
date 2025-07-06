using UnityEngine;

public class Bobber : MonoBehaviour
{
    public float bobHeight = 0.1f;
    public float bobSpeed = 2f;

    private float originalY;

    void Start()
    {
        originalY = transform.localPosition.y;
    }

    void Update()
    {
        float newY = originalY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        Vector3 pos = transform.localPosition;
        pos.y = newY;
        transform.localPosition = pos;
    }
}
