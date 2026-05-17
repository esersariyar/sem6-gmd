using System.Collections;
using TMPro;
using UnityEngine;

public class TemporaryHintUI : MonoBehaviour
{
    public TMP_Text hintText;
    public string message = "Press Space or <color=#c52222>\u25CF</color> to jump";
    public float visibleDuration = 4f;

    void Awake()
    {
        if (hintText == null)
        {
            hintText = GetComponent<TMP_Text>();
        }
    }

    void OnEnable()
    {
        StartCoroutine(ShowThenHide());
    }

    IEnumerator ShowThenHide()
    {
        if (hintText != null)
        {
            hintText.text = message;
        }

        yield return new WaitForSeconds(visibleDuration);

        gameObject.SetActive(false);
    }
}
