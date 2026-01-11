using TMPro;
using UnityEngine;
using System.Collections;

public class DamageTextScript : MonoBehaviour
{
    [Header("Animation")]
    public float showDuration = 0.8f;
    public Vector2 moveUpBy = new Vector2(0, 20);

    private TextMeshProUGUI tmp;
    private RectTransform rect;
    private Vector2 startPos;
    private Coroutine routine;

    void Awake()
    {
        // Grab components on the SAME object
        tmp = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();

        if (tmp == null)
            Debug.LogError("DamageTextScript needs TextMeshProUGUI on the same GameObject.");

        startPos = rect.anchoredPosition;

        gameObject.SetActive(false);
    }

    public void ShowCost(int amount)
    {
        if (tmp == null) return;

        // Restart animation cleanly
        if (routine != null) StopCoroutine(routine);

        tmp.text = "-" + amount;
        rect.anchoredPosition = startPos;

        gameObject.SetActive(true);
        routine = StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float t = 0f;

        while (t < showDuration)
        {
            float p = t / showDuration;
            rect.anchoredPosition = startPos + Vector2.Lerp(Vector2.zero, moveUpBy, p);

            t += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
        routine = null;
    }
}
