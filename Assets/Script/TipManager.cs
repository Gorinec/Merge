using UnityEngine;
using TMPro;
using System.Collections;

public class TipManager : MonoBehaviour
{
    public static TipManager Instance;

    public TMP_Text tipText;

    private Coroutine hideCoroutine;

    private void Awake()
    {
        Instance = this;
        tipText.gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        tipText.text = message;
        tipText.gameObject.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay(2f));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        tipText.gameObject.SetActive(false);
    }
}