using UnityEngine;
using Cinemachine;
public class CameraYRotation : MonoBehaviour
{
    [SerializeField] private float sensitivityY;
    [SerializeField] private float maxYOrbit = 20;
    [SerializeField] private float minYOrbit = -2f;
    private CinemachineOrbitalTransposer vcam;
    private float y;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineOrbitalTransposer>();
        y = 3;
    }
    void Update()
    {
        y -= +Input.GetAxis("Mouse Y") * sensitivityY * Time.timeScale;
        y = Mathf.Clamp(y, minYOrbit, maxYOrbit);
        vcam.m_FollowOffset.y = y;
    }
}