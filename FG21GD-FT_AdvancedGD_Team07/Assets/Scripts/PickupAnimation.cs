using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAnimation : MonoBehaviour
{

    [SerializeField] private float floatTime = 5;
    [SerializeField] private float floatDistance = 0.5f;

    [SerializeField] private float rotateTime = 3;
    [SerializeField] private float rotateDistance = 360;

    [SerializeField] private float shrinkTime = 0.5f;

    private PickupManager pickupManager;

    private PickupLightAnimation pickupLightAnimation;

    private AudioSource audioSource;

    private ParticleSystem particles;

    private bool pickedUp = false;

    [SerializeField] private Transform mesh;

    private float t;

    // Start is called before the first frame update
    void Start()
    {
        pickupManager = FindObjectOfType<PickupManager>();
        pickupLightAnimation = GetComponentInChildren<PickupLightAnimation>();
        audioSource = GetComponent<AudioSource>();
        particles = GetComponentInChildren<ParticleSystem>();
    }
    private void Update() => Animate();

    public void Animate()
    {
        t += Time.deltaTime;
        mesh.localPosition = new Vector3(0f, Mathf.Sin(t * floatTime) * floatDistance, 0f);
        mesh.localRotation = Quaternion.Euler(45f, t * rotateTime, 45f);
    }

    public void Shrink()
    {
        LeanTween.scale(gameObject, Vector3.zero, shrinkTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !pickedUp)
        {
            pickedUp = true;
            Shrink();
            audioSource.Play();
            particles.Play();
            pickupLightAnimation.TurnOff();
            pickupManager.pickupPickup();
            Destroy(gameObject, 3);

        }
    }
}
