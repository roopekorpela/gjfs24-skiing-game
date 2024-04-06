using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    public Player player;
    public CameraFollow cameraFollow;

    //UI
    public Slider sliderThermometer;
    public TextMeshProUGUI thermoText;
    public GameObject replayButton;
    public TextMeshProUGUI timerText;

    [SerializeField]
    public int maxTemperature = 37;
    [SerializeField]
    public int minTemperature = 21;
    [SerializeField]
    public float currentTemperature = 37;
    public float freezingInterval = 0.1f;


    //Stats:
    private float totalDistance = 0f;
    private bool stopTimer = false;
    private float highscoreDistance = 0f;

    private int nextWallSpawnTriggerPoint = -20;
    private int nextWallSpawnPostionY = -48;

    public GameObject wallPrefab;
    public Grid grid;

    private void Awake()
    {
        singleton = this;
    }

    private void Update()
    {
        if (!stopTimer)
        {
            totalDistance += Time.deltaTime;
            DisplayDistance(totalDistance);
        }

        //Wall Generation:
        if (player.transform.position.y <= nextWallSpawnTriggerPoint)
        {
            SpawnWalls();
        }
    }

    public void SpawnWalls()
    {
        Debug.Log("Spawning wall");
        GameObject newWall = Instantiate(wallPrefab, grid.transform);
        newWall.transform.position = new Vector2(0, nextWallSpawnPostionY);
        nextWallSpawnPostionY += nextWallSpawnPostionY;
        nextWallSpawnTriggerPoint = nextWallSpawnPostionY + 20;
    }

    void DisplayDistance(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliSeconds = (timeToDisplay % 1) * 100;
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliSeconds);
    }

    IEnumerator Freezer()
    {
        while (currentTemperature > minTemperature)
        {
            yield return new WaitForSeconds(freezingInterval);
            currentTemperature -= 0.1f;
            sliderThermometer.value = currentTemperature;
            int roundedTemperature = Mathf.RoundToInt(currentTemperature);
            thermoText.text = roundedTemperature + "°C";
        }
        if(currentTemperature <= minTemperature)
        {
            thermoText.text = "dead";
            EndGame();
        }
    }

    public void StartGame()
    {
        player.enabled = true;
        cameraFollow.enabled = true;
        StartCoroutine(Freezer());
        stopTimer = false;
    }

    public void EndGame()
    {
        StopCoroutine(Freezer());
        stopTimer = true;
        totalDistance = 0;
        player.enabled = false;
        replayButton.SetActive(true);

        if(totalDistance > highscoreDistance)
        {
            totalDistance = highscoreDistance;
            Debug.Log("New Best! " + highscoreDistance);
        }
        else
        {
            Debug.Log("You ascended " + highscoreDistance);
        }
    }
    public void Revive()
    {
        currentTemperature = maxTemperature;
        player.transform.position = new Vector3(0,0.5f,0);
        player.leftTrail.Clear();
        player.rightTrail.Clear();
    }
}