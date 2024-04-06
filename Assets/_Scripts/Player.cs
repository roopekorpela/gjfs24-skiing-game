using Unity.VisualScripting;
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
    public bool brakingSoundPlayed = false;

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

        Vector3 playerToMouseDirection = mousePosition - playerFeet.position;
        float angle = Vector3.SignedAngle(Vector3.right, playerToMouseDirection, Vector3.forward);
        float limitedAngle = Mathf.Clamp(angle + 90, -90, 90);
        isBreakingZone = angle > brakingZoneStart || angle < brakingZoneEnd;

        float distanceToPlayer = Vector3.Distance(mousePosition, transform.position);

        isMouseBelow = mousePosition.y < playerFeet.position.y;

        //Rotation
        if (isMouseBelow && distanceToPlayer > 0.5f)
        {
            skis.transform.rotation = Quaternion.Euler(0, 0, limitedAngle);
            leftLeg.transform.rotation = Quaternion.Euler(0f, 0f, -10f);
            rightLeg.transform.rotation = Quaternion.Euler(0f, 0f, 10f);
        }
        if(distanceToPlayer < 0.5f)
        {
            if(limitedAngle > 0)
            {
                skis.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else
            {
                skis.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
            leftLeg.transform.rotation = Quaternion.Euler(0f, 0f, -10f);
            rightLeg.transform.rotation = Quaternion.Euler(0f, 0f, 10f);
        }

        if (isMouseBelow && !isBreakingZone && distanceToPlayer > 1f)
        {
            brakingSoundPlayed = false;

            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);

            transform.position += Vector3.down * gravitationalPull * Time.deltaTime;
        }
        else
        {
            //braking   
            if (!brakingSoundPlayed)
            {
                AudioManager.singleton.Play("Ski1");
                Debug.Log("braking");
                brakingSoundPlayed = true;
            }
            currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.deltaTime, 0);

            if (currentSpeed > 0)
            {
                transform.position += Vector3.down * (currentSpeed / maxSpeed) * gravitationalPull * Time.deltaTime;
            }
        }

        Vector3 directionalForce = playerToMouseDirection.normalized * (currentSpeed / maxSpeed) * moveSpeed * Time.deltaTime;
        if (!isMouseBelow && directionalForce.y > 0)
        {
            directionalForce.y = 0;
        }
        transform.position += directionalForce;
    }



    void RotateSkisAndLegs(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float limitedAngle = Mathf.Clamp(angle + 90, -90, 90);
        skis.transform.rotation = Quaternion.Euler(0, 0, limitedAngle);
        leftLeg.transform.rotation = Quaternion.Euler(0, 0, -10);
        rightLeg.transform.rotation = Quaternion.Euler(0, 0, 10);

        isBreakingZone = angle > brakingZoneStart || angle < brakingZoneEnd;
    }

    void Accelerate()
    {
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        transform.position += Vector3.down * gravitationalPull * Time.deltaTime;
    }

    void Brake()
    {
        if (!brakingSoundPlayed)
        {
            AudioManager.singleton.Play("Ski1");
            Debug.Log("braking");
            brakingSoundPlayed = true;
        }
        currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.deltaTime, 0);
        if (currentSpeed > 0)
        {
            transform.position += Vector3.down * (currentSpeed / maxSpeed) * gravitationalPull * Time.deltaTime;
        }
    }

    void ApplyDirectionalForce(Vector3 direction)
    {
        Vector3 force = direction * (currentSpeed / maxSpeed) * moveSpeed * Time.deltaTime;
        if (!isMouseBelow && force.y > 0)
        {
            force.y = 0;
        }
        transform.position += force;
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