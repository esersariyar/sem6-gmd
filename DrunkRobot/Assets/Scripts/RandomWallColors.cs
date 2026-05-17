using UnityEngine;

public class RandomWallColors : MonoBehaviour
{
    public Color[] colors =
    {
        new Color(0.2f, 0.55f, 1f, 1f),
        new Color(1f, 0.25f, 0.25f, 1f),
        new Color(0.25f, 1f, 0.45f, 1f),
        new Color(1f, 0.85f, 0.2f, 1f),
        new Color(0.85f, 0.25f, 1f, 1f),
        new Color(1f, 0.45f, 0.15f, 1f)
    };

    public bool randomizeOnStart = true;
    public bool repeatRandomize = false;
    public float repeatInterval = 2f;
    public Renderer[] ignoredRenderers;
    public Transform[] ignoredRoots;

    private Renderer[] renderers;
    private float timer;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>(true);

        if (randomizeOnStart)
        {
            Randomize();
        }
    }

    void Update()
    {
        if (!repeatRandomize)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= repeatInterval)
        {
            timer = 0f;
            Randomize();
        }
    }

    public void Randomize()
    {
        if (colors == null || colors.Length == 0)
        {
            return;
        }

        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>(true);
        }

        foreach (Renderer renderer in renderers)
        {
            if (ShouldIgnore(renderer))
            {
                continue;
            }

            Color color = colors[Random.Range(0, colors.Length)];
            Material[] materials = renderer.materials;

            foreach (Material material in materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor("_BaseColor", color);
                }
                else
                {
                    material.color = color;
                }
            }
        }
    }

    bool ShouldIgnore(Renderer renderer)
    {
        if (ignoredRenderers != null)
        {
            foreach (Renderer ignoredRenderer in ignoredRenderers)
            {
                if (renderer == ignoredRenderer)
                {
                    return true;
                }
            }
        }

        if (ignoredRoots != null)
        {
            foreach (Transform ignoredRoot in ignoredRoots)
            {
                if (ignoredRoot != null && renderer.transform.IsChildOf(ignoredRoot))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
