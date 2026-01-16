using UnityEngine;

public class WhatAmIBlockedBy : MonoBehaviour
{
    public float distance = 1.2f;

    void Update()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out RaycastHit hit, distance))
        {
            Debug.Log("Blocked by: " + hit.collider.name + " | Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
        }
    }
}
