using UnityEngine;

public class CarFollower : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private float followSpeed, rotationSpeed;
    public bool ingoreZ = false;

    Quaternion rot;

    public void LateUpdate()
    {
        if (followTarget == null) return;

        transform.position = Damp(transform.position, followTarget.position, followSpeed, Time.deltaTime);

        if (ingoreZ)
            rot = Quaternion.Euler(followTarget.eulerAngles.x, followTarget.eulerAngles.y, 0);
        else
            rot = followTarget.rotation;

        transform.rotation = Damp(transform.rotation, rot, rotationSpeed, Time.deltaTime);
    }

    public static Vector3 Damp(Vector3 a, Vector3 b, float lambda, float dt) => Vector3.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    public static Quaternion Damp(Quaternion a, Quaternion b, float lambda, float dt) => Quaternion.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
}
