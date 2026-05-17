using UnityEngine;

[ExecuteAlways]
public class FanWind : MonoBehaviour
{
    public Transform spinningPart;
    public Vector3 localSpinAxis = Vector3.forward;
    public float spinSpeed = 720f;
    public Vector3 worldWindDirection = Vector3.forward;
    public float corridorWindLength = 115f;
    public float corridorWindHalfWidth = 7f;
    public float corridorLongRangeWindSpeed = 19f;
    public float corridorCloseRangeWindSpeed = 32f;
    public float corridorCloseRangeLength = 75f;
    public LayerMask windBlockMask = ~0;
    public bool autoFindSpinningPart = true;
    public bool createVisualSpinner = true;
    public bool useBrightVisualSpinner = false;
    public float visualSpinnerRadius = 1.25f;
    public float visualSpinnerDepth = 0.12f;
    public float visualSpinnerForwardOffset = 0.35f;
    public Color visualSpinnerColor = new Color(0.1607843f, 0.1686275f, 0.1843137f, 1f);
    public bool addBlockCollider = true;

    void Awake()
    {
        Setup();
    }

    void OnEnable()
    {
        Setup();
    }

    void Update()
    {
        if (spinningPart == null)
        {
            return;
        }

        spinningPart.Rotate(localSpinAxis.normalized, spinSpeed * Time.deltaTime, Space.Self);
    }

    void Setup()
    {
        if (spinningPart == null && autoFindSpinningPart)
        {
            spinningPart = FindSpinningPart();
        }

        if (spinningPart == null && createVisualSpinner)
        {
            spinningPart = CreateVisualSpinner();
        }

        if (spinningPart != null && spinningPart.name == "FanVisualSpinner")
        {
            RefreshVisualSpinner(spinningPart);
            ForceSpinnerMaterials(spinningPart);
        }

        if (addBlockCollider)
        {
            EnsureBlockCollider();
        }
    }

    void FixedUpdate()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Bounds bounds = GetRendererBounds();
        Vector3 origin = bounds.center;
        ApplyFanWindToTaggedPlayer(origin, worldWindDirection.normalized);
    }

    void ApplyFanWindToTaggedPlayer(Vector3 origin, Vector3 windDirection)
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            return;
        }

        Rigidbody playerRigidbody = playerObject.GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            playerRigidbody = playerObject.GetComponentInParent<Rigidbody>();
        }

        PlayerMovement playerMovement = playerObject.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            playerMovement = playerObject.GetComponentInParent<PlayerMovement>();
        }

        if (playerRigidbody == null || playerMovement == null)
        {
            return;
        }

        Vector3 toPlayer = playerRigidbody.worldCenterOfMass - origin;
        toPlayer.y = 0f;
        float forwardDistance = Vector3.Dot(toPlayer, windDirection);
        if (forwardDistance < 0f || forwardDistance > corridorWindLength)
        {
            return;
        }

        Vector3 closestPoint = origin + windDirection * forwardDistance;
        Vector3 sideOffset = playerRigidbody.worldCenterOfMass - closestPoint;
        sideOffset.y = 0f;
        if (sideOffset.magnitude > corridorWindHalfWidth)
        {
            return;
        }

        Vector3 rayOrigin = origin + windDirection * 1.5f;
        float rayDistance = Mathf.Max(0f, forwardDistance - 1.5f);
        if (Physics.Raycast(rayOrigin, windDirection, out RaycastHit hit, rayDistance, windBlockMask, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Player") && !hit.collider.transform.IsChildOf(transform))
            {
                return;
            }
        }

        float closeFactor = 1f - Mathf.Clamp01(forwardDistance / corridorCloseRangeLength);
        float windSpeed = Mathf.Lerp(corridorLongRangeWindSpeed, corridorCloseRangeWindSpeed, closeFactor);
        playerMovement.SetExternalVelocity(windDirection * windSpeed);
    }

    Transform FindSpinningPart()
    {
        Transform best = null;

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child == transform)
            {
                continue;
            }

            string childName = child.name.ToLowerInvariant();
            if (childName.Contains("fanvisualspinner") || childName.Contains("fanvisualblade"))
            {
                continue;
            }

            if (childName.Contains("fan") || childName.Contains("blade") || childName.Contains("rotor") || childName.Contains("propeller") || childName.Contains("turbine"))
            {
                best = child;
            }
        }

        return best;
    }

    Transform CreateVisualSpinner()
    {
        Transform existing = transform.Find("FanVisualSpinner");
        if (existing != null)
        {
            return existing;
        }

        GameObject spinner = new GameObject("FanVisualSpinner");
        spinner.transform.SetParent(transform, false);
        spinner.transform.localPosition = GetVisualSpinnerLocalPosition();
        spinner.transform.localRotation = Quaternion.identity;
        spinner.transform.localScale = Vector3.one;

        RefreshVisualSpinner(spinner.transform);
        return spinner.transform;
    }

    void RefreshVisualSpinner(Transform spinner)
    {
        Material material = CreateSpinnerMaterial();

        EnsureBlade(spinner.transform, material, 0, 0f);
        EnsureBlade(spinner.transform, material, 1, 60f);
        EnsureBlade(spinner.transform, material, 2, 120f);
    }

    Material CreateSpinnerMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }

        if (shader == null)
        {
            shader = Shader.Find("Universal Render Pipeline/Lit");
        }

        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        Material material = new Material(shader);
        Color color = new Color(0.1607843f, 0.1686275f, 0.1843137f, 1f);
        material.color = color;

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }

        material.DisableKeyword("_EMISSION");
        if (material.HasProperty("_EmissionColor"))
        {
            material.SetColor("_EmissionColor", Color.black);
        }

        return material;
    }

    void ForceSpinnerMaterials(Transform spinner)
    {
        Material material = CreateSpinnerMaterial();
        Renderer[] renderers = spinner.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterial = material;
        }
    }

    Vector3 GetVisualSpinnerLocalPosition()
    {
        Bounds bounds = GetRendererBounds();
        Vector3 worldCenter = bounds.center;
        worldCenter += worldWindDirection.normalized * visualSpinnerForwardOffset;
        return transform.InverseTransformPoint(worldCenter);
    }

    void EnsureBlade(Transform parent, Material material, int index, float angle)
    {
        Transform existing = parent.Find("FanVisualBlade" + index);
        GameObject blade;
        if (existing == null)
        {
            blade = new GameObject("FanVisualBlade" + index);
            blade.transform.SetParent(parent, false);
            blade.AddComponent<MeshFilter>();
            blade.AddComponent<MeshRenderer>();
        }
        else
        {
            blade = existing.gameObject;
        }

        blade.transform.SetParent(parent, false);
        blade.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        blade.transform.localPosition = Vector3.zero;
        blade.transform.localScale = Vector3.one;

        MeshFilter meshFilter = blade.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = CreateBladeMesh();

        Renderer renderer = blade.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
    }

    Mesh CreateBladeMesh()
    {
        float halfLength = visualSpinnerRadius;
        float halfWidth = visualSpinnerDepth;
        Mesh mesh = new Mesh();
        mesh.vertices = new[]
        {
            new Vector3(-halfLength, -halfWidth, 0f),
            new Vector3(halfLength, -halfWidth, 0f),
            new Vector3(halfLength, halfWidth, 0f),
            new Vector3(-halfLength, halfWidth, 0f)
        };
        mesh.triangles = new[]
        {
            0, 1, 2,
            0, 2, 3,
            2, 1, 0,
            3, 2, 0
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    void EnsureBlockCollider()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null)
        {
            box = gameObject.AddComponent<BoxCollider>();
        }

        Bounds bounds = GetRendererBounds();
        Vector3 localCenter = transform.InverseTransformPoint(bounds.center);
        Vector3 localSize = transform.InverseTransformVector(bounds.size);
        box.center = localCenter;
        box.size = new Vector3(Mathf.Abs(localSize.x), Mathf.Abs(localSize.y), Mathf.Abs(localSize.z));
        box.isTrigger = false;
    }

    Bounds GetRendererBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        Bounds bounds = new Bounds(transform.position, Vector3.one);
        bool hasBounds = false;

        for (int i = 0; i < renderers.Length; i++)
        {
            string rendererName = renderers[i].name.ToLowerInvariant();
            if (rendererName.Contains("fanvisualspinner") || rendererName.Contains("fanvisualblade"))
            {
                continue;
            }

            if (!hasBounds)
            {
                bounds = renderers[i].bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
        }

        return bounds;
    }
}
