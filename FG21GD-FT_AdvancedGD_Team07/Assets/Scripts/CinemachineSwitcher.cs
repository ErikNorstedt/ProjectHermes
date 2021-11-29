using UnityEngine;

public class CinemachineSwitcher : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera[] cam;

    public int currentCam = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentCam++;

            if (currentCam >= cam.Length)
                currentCam = 0;

            for (int i = 0; i < cam.Length; i++)
            {
                if (i == currentCam)
                    cam[i].Priority = 1;
                else
                    cam[i].Priority = 0;
            }
        }
    }
}
