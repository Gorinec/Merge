using UnityEngine;
using UnityEngine.UI;

public class MergeEffect : MonoBehaviour
{
    public float duration = 0.4f;
    public float maxScale = 1.5f;

    private Image image;
    private RectTransform rectTransform;
    private float timer;

    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        timer = 0f;
        rectTransform.localScale = Vector3.one * 0.3f;
        Color c = image.color;
        c.a = 1f;
        image.color = c;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;

        if (t >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        // Расширение
        rectTransform.localScale = Vector3.one * Mathf.Lerp(0.3f, maxScale, t);

        // Затухание
        Color c = image.color;
        c.a = Mathf.Lerp(1f, 0f, t);
        image.color = c;
    }
}