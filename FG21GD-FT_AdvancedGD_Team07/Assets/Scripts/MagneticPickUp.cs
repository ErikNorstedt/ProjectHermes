using UnityEngine;

public class MagneticPickUp : MonoBehaviour
{
    public Transform playerPosition;
    private float speed;
    private Transform pillTransform;

    private void Start() => pillTransform = transform.parent.transform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerPosition = other.transform;
    }
    private void Update()
    {
        if (playerPosition == null) return;

        speed += Time.deltaTime;
        pillTransform.position = Vector3.MoveTowards(pillTransform.position, playerPosition.position, speed);
    }
}
