using UnityEngine;

public class RandomSlider : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    public Axis slideAxis = Axis.X;
    public bool useLocalSpace = true;
    public float maxOffset = 3f;
    public float minSpeed = 1.5f;
    public float maxSpeed = 4f;
    public float minPauseAtEnd = 0f;
    public float maxPauseAtEnd = 0.6f;
    public bool randomizeSpeedEachLeg = true;
    public bool randomizeStartDirection = true;

    private Vector3 startPosition;
    private float currentOffset;
    private float direction = 1f;
    private float currentSpeed;
    private float pauseUntil;

    void Awake()
    {
        startPosition = useLocalSpace ? transform.localPosition : transform.position;

        if (randomizeStartDirection && Random.value < 0.5f)
        {
            direction = -1f;
        }

        currentSpeed = Random.Range(minSpeed, maxSpeed);
        currentOffset = Random.Range(-maxOffset, maxOffset);
        ApplyOffset();
    }

    void Update()
    {
        if (Time.time < pauseUntil)
        {
            return;
        }

        currentOffset += direction * currentSpeed * Time.deltaTime;

        if (currentOffset >= maxOffset)
        {
            currentOffset = maxOffset;
            direction = -1f;
            OnHitEnd();
        }
        else if (currentOffset <= -maxOffset)
        {
            currentOffset = -maxOffset;
            direction = 1f;
            OnHitEnd();
        }

        ApplyOffset();
    }

    void OnHitEnd()
    {
        if (randomizeSpeedEachLeg)
        {
            currentSpeed = Random.Range(minSpeed, maxSpeed);
        }

        float pause = Random.Range(minPauseAtEnd, maxPauseAtEnd);
        if (pause > 0f)
        {
            pauseUntil = Time.time + pause;
        }
    }

    void ApplyOffset()
    {
        Vector3 axisVector = slideAxis switch
        {
            Axis.Y => Vector3.up,
            Axis.Z => Vector3.forward,
            _ => Vector3.right
        };

        Vector3 newPos = startPosition + axisVector * currentOffset;

        if (useLocalSpace)
        {
            transform.localPosition = newPos;
        }
        else
        {
            transform.position = newPos;
        }
    }
}
