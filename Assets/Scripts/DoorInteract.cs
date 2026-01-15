using UnityEngine;
using TMPro;
using System.Collections;

public class DoorInteract : MonoBehaviour
{
    [Header("Teleport Points")]
    public Transform insidePoint;
    public Transform outsidePoint;

    [Header("Door Rules")]
    public bool allowExit = true;

    [Header("UI & Fade")]
    public TextMeshProUGUI interactText;
    public FadeController fadeController;

    private bool playerInRange;
    private bool isTeleporting;

    private bool playerIsOutside;

    private Rigidbody playerRb;
    private Transform player;

    void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.transform;
        playerRb = playerObj.GetComponentInChildren<Rigidbody>();
    }

    void Start()
    {
        interactText.gameObject.SetActive(false);

        float distOutside = Vector3.Distance(player.position, outsidePoint.position);
        float distInside = Vector3.Distance(player.position, insidePoint.position);
        playerIsOutside = distOutside < distInside;
    }

    void Update()
    {
        if (!playerInRange || isTeleporting)
            return;

        UpdatePrompt();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!allowExit && !playerIsOutside)
                return;

            StartCoroutine(TeleportRoutine());
        }
    }

    IEnumerator TeleportRoutine()
    {
        isTeleporting = true;
        interactText.gameObject.SetActive(false);

        yield return StartCoroutine(fadeController.FadeOut());

        Transform target = playerIsOutside ? insidePoint : outsidePoint;

        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
        playerRb.position = target.position;

        playerIsOutside = !playerIsOutside;

        yield return new WaitForFixedUpdate();
        yield return StartCoroutine(fadeController.FadeIn());

        isTeleporting = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;
        UpdatePrompt();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        interactText.gameObject.SetActive(false);
    }

    void UpdatePrompt()
    {
        if (!allowExit && !playerIsOutside)
        {
            interactText.gameObject.SetActive(false);
            return;
        }

        interactText.gameObject.SetActive(true);
        interactText.text = playerIsOutside
            ? "Press E to Enter"
            : "Press E to Exit";
    }
}
