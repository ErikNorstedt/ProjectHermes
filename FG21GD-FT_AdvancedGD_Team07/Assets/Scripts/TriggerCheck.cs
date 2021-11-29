using UnityEngine.Events;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    public LayerMask mask;
    public int colliderID;

    public UnityEvent<int> onTriggerEnter;
    public UnityEvent<int> onTriggerStay;
    public UnityEvent<int> onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if ((mask.value & (1 << other.transform.gameObject.layer)) > 0)
            onTriggerEnter?.Invoke(colliderID);
    }

    private void OnTriggerStay(Collider other)
    {
        if ((mask.value & (1 << other.transform.gameObject.layer)) > 0)
            onTriggerStay?.Invoke(colliderID);
    }

    private void OnTriggerExit(Collider other)
    {
        if ((mask.value & (1 << other.transform.gameObject.layer)) > 0)
            onTriggerExit?.Invoke(colliderID);
    }
}
