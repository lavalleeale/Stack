using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    GameObject currentPlatform;
    GameObject lastPlatform;

    public GameObject platformPrefab;
    public GameObject mainCamera;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;
    public GameObject startButton;

    public float speed = 10;
    bool goingLeft = false;
    int height = 0;
    float startingHeight;
    float size = 10;
    int highScore;
    bool running = false;

    void Start()
    {
        highScore = PlayerPrefs.GetInt("highscore", 0);
        highscoreText.text = $"Highscore: {highScore}";
        startingHeight = mainCamera.transform.position.y;
    }

    public void StartGame()
    {
        size = 10;
        height = 0;
        foreach (var platform in GameObject.FindGameObjectsWithTag("platform"))
        {
            Destroy(platform);
        }
        startButton.SetActive(false);
        scoreText.text = $"Score: {height}";
        currentPlatform = GameObject.Find("Platform");
        NewPlatform();
        running = true;
    }

    void NewPlatform()
    {
        lastPlatform = currentPlatform;
        currentPlatform = Instantiate(platformPrefab);
        currentPlatform.transform.localScale = new Vector3(x: size, y: 1, z: 10);
        currentPlatform.GetComponent<Renderer>().material.color = Color.HSVToRGB(H: Random.Range(0f, 1f), 0.5f, 1);
        if (Random.Range(0, 2) == 1)
        {
            currentPlatform.transform.position = new Vector3(x: lastPlatform.transform.position.x + size, y: height, z: 0);
        }
        else
        {
            currentPlatform.transform.position = new Vector3(x: lastPlatform.transform.position.x - size, y: height, z: 0);
        }
        height++;
    }

    void Update()
    {
        if (running)
        {
            mainCamera.transform.position += new Vector3(x: 0, y: startingHeight + height - mainCamera.transform.position.y, z: 0) * Time.deltaTime;
            var lastLeftBound = lastPlatform.transform.position.x - lastPlatform.transform.localScale.x / 2;
            var lastRightBound = lastPlatform.transform.position.x + lastPlatform.transform.localScale.x / 2;

            var currentLeftBound = currentPlatform.transform.position.x - currentPlatform.transform.localScale.x / 2;
            var currentRightBound = currentPlatform.transform.position.x + currentPlatform.transform.localScale.x / 2;

            var leftDiff = lastLeftBound - currentLeftBound;
            var rightDiff = currentRightBound - lastRightBound;

            if (currentLeftBound > lastRightBound + 1)
            {
                goingLeft = true;
            }
            else if (currentRightBound < lastLeftBound - 1)
            {
                goingLeft = false;
            }
            if (goingLeft)
            {
                currentPlatform.transform.position = currentPlatform.transform.position + new Vector3(x: -Time.deltaTime * speed, y: 0, z: 0);
            }
            else
            {
                currentPlatform.transform.position = currentPlatform.transform.position + new Vector3(x: Time.deltaTime * speed, y: 0, z: 0);
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GameObject dropper;
                if (leftDiff > size || rightDiff > size)
                {
                    var currentRigid = currentPlatform.GetComponent<Rigidbody>();
                    currentRigid.useGravity = true;
                    currentRigid.freezeRotation = false;
                    currentRigid.constraints = new RigidbodyConstraints();
                    running = false;
                    startButton.SetActive(true);
                    return;
                }
                else if (leftDiff > 0)
                {
                    var missAmount = leftDiff;
                    size -= missAmount;
                    currentPlatform.transform.localScale = new Vector3(x: size, y: 1, z: 10);
                    currentPlatform.transform.position += new Vector3(x: missAmount / 2, y: 0, z: 0);
                    dropper = Instantiate(platformPrefab);
                    dropper.GetComponent<Renderer>().material.color = currentPlatform.GetComponent<Renderer>().material.color;
                    dropper.transform.localScale = new Vector3(x: missAmount, y: 1, z: 10);
                    dropper.transform.position = new Vector3(x: currentLeftBound + missAmount / 2, height - 1, 0);
                }
                else
                {
                    var missAmount = rightDiff;
                    size -= missAmount;
                    currentPlatform.transform.localScale = new Vector3(x: size, y: 1, z: 10);
                    currentPlatform.transform.position -= new Vector3(x: missAmount / 2, y: 0, z: 0);
                    dropper = Instantiate(platformPrefab);
                    dropper.GetComponent<Renderer>().material.color = currentPlatform.GetComponent<Renderer>().material.color;
                    dropper.transform.localScale = new Vector3(x: missAmount, y: 1, z: 10);
                    dropper.transform.position = new Vector3(x: currentRightBound - missAmount / 2, height - 1, 0);
                }
                scoreText.text = $"Score: {height}";
                if (highScore < height)
                {
                    highScore = height;
                    PlayerPrefs.SetInt("highscore", height);
                    PlayerPrefs.Save();
                    highscoreText.text = $"Highscore: {height}";
                }
                var rigid = dropper.GetComponent<Rigidbody>();
                rigid.useGravity = true;
                rigid.freezeRotation = false;
                rigid.constraints = new RigidbodyConstraints();
                NewPlatform();
            }
        }
    }
}
