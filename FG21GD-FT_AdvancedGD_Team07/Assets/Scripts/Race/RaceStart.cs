using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceStart : MonoBehaviour
{

    [SerializeField] private Race race;

    private ParticleSystem particles;

    private BoxCollider boxCollider;

    private void Start()
    {
        particles = gameObject.GetComponentInChildren<ParticleSystem>();
        boxCollider = gameObject.GetComponent<BoxCollider>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            race.StartRace();
            particles.Stop();
            boxCollider.enabled = false;

        }
    }

}
