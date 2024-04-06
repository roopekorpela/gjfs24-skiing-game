using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    public Player player;
    public CameraFollow cameraFollow;

    //UI
    public Slider sliderThermometer;
    public TextMeshProUGUI thermoText;
    public GameObject replayButton;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI highscoreText;

    [SerializeField]
    public int maxTemperature = 37;
    [SerializeField]
    public int minTemperature = 21;
    [SerializeField]
    public float currentTemperature = 37;
    public float freezingInterval = 0.1f;


    //Stats:
    private int totalDistance = 0;
    private bool stopTimer = false;

    private int nextWallSpawnTriggerPoint = -20;
    private int nextWallSpawnPostionY = -48;

    public GameObject wallPrefab;
    public Grid grid;

    private void Awake()
    {
        singleton = this;
    }
    private void Start()
    {
        highscoreText.text = PlayerPrefs.GetInt("HighScore").ToString() + "m";
    }

    private void Update()
    {
        if (!stopTimer)
        {
            totalDistance = Mathf.RoundToInt(-player.transform.position.y);
            distanceText.text = totalDistance + "m"; // Display distance in meters
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
        player.enabled = false;
        replayButton.SetActive(true);

        if(totalDistance > PlayerPrefs.GetInt("HighScore"))
        {
            Debug.Log("New Best! " + totalDistance);
            PlayerPrefs.SetInt("HighScore", totalDistance);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("You ascended " + totalDistance);
        }
    }
    public void Revive()
    {
        currentTemperature = maxTemperature;
        player.transform.position = new Vector3(0,0.5f,0);
        player.leftTrail.Clear();
        player.rightTrail.Clear();
        totalDistance = 0;
        SceneManager.LoadScene("Gameplay");
    }
}