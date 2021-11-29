using UnityEngine;

public class RaceArrow : MonoBehaviour
{
    public Transform nextRing;
    public Material arrowMaterial;
    public Material arrowTopMaterial;

    private void Awake()
    {
        arrowMaterial.SetFloat("_Alpha", 0);
        arrowTopMaterial.SetFloat("_Alpha", 0);
    }

    private void OnEnable()
    {
        Race.onRaceStarted += FadeInArrow;
        Race.onRaceEnded += FadeOutArrow;
        Ring.onRingEntered += SetDestination;
    }

    private void OnDisable()
    {
        Race.onRaceStarted -= FadeInArrow;
        Race.onRaceEnded -= FadeOutArrow;
        Ring.onRingEntered -= SetDestination;
    }

    void Update()
    {
        if (nextRing == null) return;

        transform.LookAt(nextRing);
    }

    public void SetDestination(Transform nextPos) => nextRing = nextPos;

    public void FadeInArrow()
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, 0f, 1f, 0.3f).setOnUpdate((float val)=> { arrowMaterial.SetFloat("_Alpha", val); arrowTopMaterial.SetFloat("_Alpha", val); });
    }

    public void FadeOutArrow()
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, 1f, 0f, 0.3f).setOnUpdate((float val) => { arrowMaterial.SetFloat("_Alpha", val); arrowTopMaterial.SetFloat("_Alpha", val); });
    }
}
