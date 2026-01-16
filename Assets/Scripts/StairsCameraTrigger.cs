using UnityEngine;
using Unity.Cinemachine;

public class StairsCameraZone : MonoBehaviour
{
    public CinemachineCamera stairsCam;
    public CinemachineCamera normalCam;

    public string playerTag = "Player";
    public int activePriority = 20;
    public int inactivePriority = 0;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (normalCam != null) normalCam.Priority = inactivePriority;
        if (stairsCam != null) stairsCam.Priority = activePriority;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (stairsCam != null) stairsCam.Priority = inactivePriority;
        if (normalCam != null) normalCam.Priority = activePriority;
    }
}
