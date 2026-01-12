using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 1.5f;

    void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator FadeOut()
    {
        while (fadeImage.color.a < 1)
        {
            fadeImage.color += new Color(0, 0, 0, Time.deltaTime * fadeSpeed);
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        while (fadeImage.color.a > 0)
        {
            fadeImage.color -= new Color(0, 0, 0, Time.deltaTime * fadeSpeed);
            yield return null;
        }
    }
}
