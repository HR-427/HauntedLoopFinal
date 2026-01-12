using System.Collections;
using TMPro;
using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    [Header("Prefab + Parent")]
    public TextMeshProUGUI textPrefab;
    public RectTransform parentCanvas; // usually your Canvas rect

    [Header("Animation")]
    public float lifetime = 0.8f;
    public Vector2 moveUpBy = new Vector2(0, 30);

    public void ShowCost(int amount)
    {
        if (textPrefab == null)
        {
            Debug.LogError("DamageTextSpawner: textPrefab is missing!");
            return;
        }

        // If parentCanvas not set, use this object's parent canvas
        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        // Spawn as UI element under the canvas
        TextMeshProUGUI t = Instantiate(textPrefab, parentCanvas);
        RectTransform r = t.rectTransform;

        // Put it at the spawner's anchored position (top-right)
        r.anchorMin = ((RectTransform)transform).anchorMin;
        r.anchorMax = ((RectTransform)transform).anchorMax;
        r.pivot = ((RectTransform)transform).pivot;
        r.anchoredPosition = ((RectTransform)transform).anchoredPosition;

        t.text = "-" + amount;

        StartCoroutine(AnimateAndDestroy(r, t));
    }

    IEnumerator AnimateAndDestroy(RectTransform r, TextMeshProUGUI t)
    {
        Vector2 start = r.anchoredPosition;
        Vector2 end = start + moveUpBy;

        float time = 0f;
        Color c = t.color;

        while (time < lifetime)
        {
            float p = time / lifetime;
            r.anchoredPosition = Vector2.Lerp(start, end, p);

            // fade out
            c.a = 1f - p;
            t.color = c;

            time += Time.deltaTime;
            yield return null;
        }

        Destroy(t.gameObject);
    }
}
