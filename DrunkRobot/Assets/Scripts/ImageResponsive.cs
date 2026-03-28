using UnityEngine;
using UnityEngine.UI;

public class BackgroundScaler : MonoBehaviour
{
    public Image image;

    void Start()
    {
        RectTransform rt = image.rectTransform;

        float screenRatio = (float)Screen.width / Screen.height;
        float imageRatio = image.sprite.bounds.size.x / image.sprite.bounds.size.y;

        if (screenRatio >= imageRatio)
        {
            rt.sizeDelta = new Vector2(Screen.width, Screen.width / imageRatio);
        }
        else
        {
            rt.sizeDelta = new Vector2(Screen.height * imageRatio, Screen.height);
        }
    }
}