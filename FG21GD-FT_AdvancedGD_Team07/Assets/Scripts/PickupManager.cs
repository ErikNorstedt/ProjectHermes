using UnityEngine;

public class PickupManager : MonoBehaviour
{
    private GameObject[] pickups;

    private int amountOfPickups;
    private int pickedUp;

    private PickupText pickupText;

    private void Awake()
    {
        pickups = new GameObject[transform.childCount];

        for (int i = 0; i < pickups.Length; i++)
            pickups[i] = transform.GetChild(i).gameObject;
    }

    void Start()
    {
        pickupText = FindObjectOfType<PickupText>();
        amountOfPickups = pickups.Length;
        pickupText.SetPickupText(pickedUp + "/" + amountOfPickups);
    }

    public void pickupPickup()
    {
        pickedUp++;
        pickupText.SetPickupText(pickedUp + "/" + amountOfPickups);
    }
}
