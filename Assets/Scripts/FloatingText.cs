using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float riseSpeed = 1f;
    public float lifetime = 1f;

    SpriteRenderer sr;
    Color c;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        c = sr.color;
    }

    void Update()
    {
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        c.a -= Time.deltaTime / lifetime;
        sr.color = c;

        if (c.a <= 0f)
            Destroy(gameObject);
    }
}
