using UnityEngine;
using TMPro;
using System.Collections;

public class DoorInteract : MonoBehaviour
{
    public Transform teleportTarget;
    public TextMeshProUGUI interactText;
    public FadeController fadeController;

    public bool isExitDoor; // CHECK this on exit doors

    private bool playerInRange;

    void Start()
    {
        interactText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(TeleportWithFade());
        }
    }

    IEnumerator TeleportWithFade()
    {
        yield return StartCoroutine(fadeController.FadeOut());

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = teleportTarget.position;

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(fadeController.FadeIn());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactText.gameObject.SetActive(true);

            if (isExitDoor)
                interactText.text = "Press E to Exit";
            else
                interactText.text = "Press E to Enter";
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactText.gameObject.SetActive(false);
        }
    }
}
