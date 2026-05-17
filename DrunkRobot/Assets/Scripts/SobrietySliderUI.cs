using UnityEngine;
using UnityEngine.UI;

public class SobrietySliderUI : MonoBehaviour
{
    public MouseLook mouseLook;
    public Slider slider;
    public GameObject visualRoot;
    public bool hideHandle = true;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        if (visualRoot == null && slider != null)
        {
            visualRoot = slider.gameObject;
        }

        if (visualRoot != null)
        {
            canvasGroup = visualRoot.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = visualRoot.AddComponent<CanvasGroup>();
            }
        }

        HideHandle();
        SetVisible(false);
    }

    void Update()
    {
        if (mouseLook == null || slider == null)
        {
            SetVisible(false);
            return;
        }

        bool shouldShow = mouseLook.IsSobering;
        SetVisible(shouldShow);

        if (!shouldShow)
        {
            return;
        }

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = mouseLook.SoberProgress;
    }

    void SetVisible(bool visible)
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;

        if (visible && visualRoot != null)
        {
            visualRoot.transform.SetAsLastSibling();
        }
    }

    void HideHandle()
    {
        if (!hideHandle || slider == null)
        {
            return;
        }

        if (slider.handleRect != null)
        {
            slider.handleRect.gameObject.SetActive(false);
            slider.handleRect = null;
        }

        if (slider.targetGraphic != null && slider.targetGraphic.name.ToLowerInvariant().Contains("handle"))
        {
            slider.targetGraphic.gameObject.SetActive(false);
            slider.targetGraphic = null;
        }
    }
}
