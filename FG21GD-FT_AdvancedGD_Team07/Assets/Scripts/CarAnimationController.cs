using UnityEngine;

public class CarAnimationController : MonoBehaviour
{

    public Transform leftWing;
    public Transform rightWing;

    private int rot;
    private Rigidbody rb;
    private CarController controller;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CarController>();
    }

    void LateUpdate()
    {
        if (rb.velocity.y < 0f && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !controller.IsGrounded) 
            rot = 90;
        else
            rot = 0;

        leftWing.localRotation = Quaternion.Slerp(leftWing.localRotation, Quaternion.Euler(0f, 0f, -rot), Time.deltaTime * 25f);
        rightWing.localRotation = Quaternion.Slerp(rightWing.localRotation, Quaternion.Euler(0f, 0f, rot), Time.deltaTime * 25f);
    }
}
