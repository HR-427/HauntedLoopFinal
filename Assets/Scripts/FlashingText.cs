using UnityEngine;
using TMPro;

public class FlashingText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Color colorA = Color.yellow;
    public Color colorB = Color.red;
    public float speed = 1f;

    void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        text.color = Color.Lerp(colorA, colorB, t);
    }
}
