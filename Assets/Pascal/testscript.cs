using UnityEngine;

public class testscript : MonoBehaviour
{
    private float speed;

    // Use this for initialization
    private void Start()
    {
        speed = 70f;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.eulerAngles = new Vector3(transform.localEulerAngles.x,
            transform.localEulerAngles.y + Input.acceleration.x * Time.deltaTime * speed, transform.localEulerAngles.z);
        if (Input.GetMouseButton(0)) MoveForward();
    }

    public void MoveForward()
    {
        transform.Translate(0f, 0f, speed * Time.deltaTime * 0.05f);
    }
}