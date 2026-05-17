using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LaserHazard : MonoBehaviour
{
    public float moveDistance = 2f;
    public float moveSpeed = 1.5f;
    public bool startGoingUp = true;
    public bool useMinHeight = true;
    public float minHeight = 0.5f;

    [Header("Proximity Sound")]
    public AudioClip laserSound;
    public Transform player;
    public float hearingRange = 15f;
    public float maxVolume = 1f;
    [Range(0f, 1f)] public float spatialBlend = 1f;
    public float clipStartTrim = 0.2f;
    public float clipEndTrim = 1f;

    private Vector3 startPosition;
    private float direction;
    private AudioSource audioSource;
    private Collider laserCollider;
    private Transform audioAnchor;

    void Awake()
    {
        startPosition = transform.position;
        direction = startGoingUp ? 1f : -1f;

        laserCollider = GetComponent<Collider>();

        GameObject anchorGO = new GameObject("LaserAudioAnchor");
        audioAnchor = anchorGO.transform;
        audioAnchor.position = transform.position;

        audioSource = anchorGO.AddComponent<AudioSource>();

        audioSource.clip = laserSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = spatialBlend;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = hearingRange;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.volume = 0f;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (laserSound != null)
        {
            audioSource.Play();
            audioSource.time = Mathf.Clamp(clipStartTrim, 0f, Mathf.Max(0f, laserSound.length - 0.01f));
        }
    }

    void OnEnable()
    {
        PlayerRespawn.Respawned += ResetToStart;
    }

    void OnDisable()
    {
        PlayerRespawn.Respawned -= ResetToStart;
    }

    void OnDestroy()
    {
        if (audioAnchor != null)
        {
            Destroy(audioAnchor.gameObject);
        }
    }

    void Update()
    {
        float offset = transform.position.y - startPosition.y;
        float lowerLimit = useMinHeight ? Mathf.Max(-moveDistance, minHeight - startPosition.y) : -moveDistance;

        if (offset >= moveDistance && direction > 0f)
        {
            direction = -1f;
        }
        else if (offset <= lowerLimit && direction < 0f)
        {
            direction = 1f;
        }

        Vector3 nextPos = transform.position + Vector3.up * direction * moveSpeed * Time.deltaTime;
        float minY = startPosition.y + lowerLimit;
        float maxY = startPosition.y + moveDistance;
        nextPos.y = Mathf.Clamp(nextPos.y, minY, maxY);
        transform.position = nextPos;

        UpdateProximityVolume();
        UpdateAudioLoop();
    }

    void UpdateAudioLoop()
    {
        if (audioSource == null || laserSound == null)
        {
            return;
        }

        float endTime = Mathf.Max(clipStartTrim + 0.05f, laserSound.length - clipEndTrim);
        if (audioSource.time >= endTime)
        {
            audioSource.time = Mathf.Clamp(clipStartTrim, 0f, laserSound.length - 0.01f);
        }
    }

    void UpdateProximityVolume()
    {
        if (audioSource == null || laserSound == null || player == null)
        {
            return;
        }

        Vector3 referencePoint = laserCollider != null
            ? laserCollider.ClosestPoint(player.position)
            : transform.position;

        float distance = Vector3.Distance(referencePoint, player.position);
        float t = Mathf.Clamp01(1f - (distance / hearingRange));
        audioSource.volume = t * maxVolume;

        if (audioAnchor != null)
        {
            audioAnchor.position = referencePoint;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        TryHitPlayer(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        TryHitPlayer(other.gameObject);
    }

    void TryHitPlayer(GameObject hitObject)
    {
        if (!hitObject.CompareTag("Player"))
        {
            return;
        }

        PlayerRespawn respawn = hitObject.GetComponent<PlayerRespawn>();
        if (respawn == null)
        {
            respawn = hitObject.GetComponentInParent<PlayerRespawn>();
        }

        if (respawn != null)
        {
            respawn.Respawn();
        }
    }

    void ResetToStart()
    {
        transform.position = startPosition;
        direction = startGoingUp ? 1f : -1f;
    }
}
