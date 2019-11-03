using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD instance;
    Text healthText;
    Text scoreText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            scoreText = transform.Find("ScoreText").GetComponent<Text>();
            healthText = transform.Find("HealthText").GetComponent<Text>();
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
    }

    public void DisplayHealth(int health)
    {
        healthText.text = string.Format("Health {0}", health);
    }

    public void DisplayScore(int score)
    {
        scoreText.text = string.Format("Score {0}", score);
    }

}