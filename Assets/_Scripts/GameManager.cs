using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    public Player player;
    public CameraFollow cameraFollow;

    public Slider sliderThermometer;
    public TextMeshProUGUI thermoText;

    [SerializeField]
    public int maxTemperature = 37;
    [SerializeField]
    public int minTemperature = 21;
    [SerializeField]
    public float currentTemperature = 37;
    public float freezingInterval = 0.1f;

    private void Awake()
    {
        singleton = this;
    }
    void Start()
    {
        StartCoroutine(Freezer());
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

    }
    public void EndGame()
    {
        player.enabled = false;
        cameraFollow.enabled = false;
    }
    public void RestartGame()
    {
        player.transform.position = Vector3.zero;
        StartGame();
    }
}