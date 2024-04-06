using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float maxSpeed = 10.0f;
    public float acceleration = 5.0f;
    public float deceleration = 10.0f;
    public float gravitationalPull = 2.0f; // Gravitational pull to simulate downhill force
    public float brakingZoneStart = -30f; // Start of the braking zone near 0 degrees
    public float brakingZoneEnd = -150f; // Start of the braking zone near -180 degrees

    private SpriteRenderer spriteRenderer;
    private float currentSpeed = 0.0f;
    public Transform playerFeet;
    public SpriteRenderer skis;
    public SpriteRenderer leftLeg, rightLeg;
    public TrailRenderer leftTrail;
    public TrailRenderer rightTrail;

    private bool isMouseBelow;
    private bool isBreakingZone;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        MoveAndRotatePlayer();
        DrawAccelerationIndicator();
    }

    void MoveAndRotatePlayer()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        Vector3 mouseDirection = (mousePosition - playerFeet.position).normalized;
        Vector3 downwardForceDirection = Vector3.down;
        isMouseBelow = mousePosition.y < playerFeet.position.y;

        if (isMouseBelow)
        {
            float angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
            // Limit ski rotation
            float limitedAngle = Mathf.Clamp(angle + 90, -90, 90);
            skis.transform.rotation = Quaternion.Euler(0, 0, limitedAngle);
            leftLeg.transform.rotation = Quaternion.Euler(0f, 0f, -10f);
            rightLeg.transform.rotation = Quaternion.Euler(0f, 0f, 10f);

            isBreakingZone = angle > brakingZoneStart || angle < brakingZoneEnd;
        }

        if (isMouseBelow && !isBreakingZone)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);

            transform.position += downwardForceDirection * gravitationalPull * Time.deltaTime;
        }
        else
        {
            //braking
            currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.deltaTime, 0);

            if (currentSpeed > 0)
            {
                transform.position += downwardForceDirection * (currentSpeed / maxSpeed) * gravitationalPull * Time.deltaTime;
            }
        }

        Vector3 directionalForce = mouseDirection * (currentSpeed / maxSpeed) * moveSpeed * Time.deltaTime;
        if (!isMouseBelow && directionalForce.y > 0)
        {
            directionalForce.y = 0;
        }
        transform.position += directionalForce;
    }

    void DrawAccelerationIndicator()
    {
        Debug.DrawLine(new Vector2(-10,playerFeet.position.y), new Vector2(+10, playerFeet.position.y), Color.green);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collided with: " + other.gameObject.name);

        if (other.CompareTag("Obstacle"))
        {
            GameManager.singleton.currentTemperature = GameManager.singleton.minTemperature;
            enabled = false;
        }
        else if (other.CompareTag("HotCocoa"))
        {
            if(GameManager.singleton.currentTemperature <= 32)
            {
                GameManager.singleton.currentTemperature += 2;
            }
            else if(GameManager.singleton.currentTemperature >= 32)
            {
                GameManager.singleton.currentTemperature = GameManager.singleton.maxTemperature;
            }
            Destroy(other.gameObject);
        }
    }
}