using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class UI : MonoBehaviour
{

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI numberOfEnemies;
    public GameObject gameScreen;
    public GameObject key;
    public GameObject miniMap;

    public static UI instance;

    private void Awake()
    {
        instance = this;
        // confine the cursor to the center of the game mode and disable the cursor (press ESC to make the cursor visible again)
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Start is called before the first frame update

    public void UpdateHealth(int curHealth, int maxHealth)
    {
        healthText.text = "Health : " + curHealth + "/" + maxHealth;
    }

    public void UpdateNumberOfEnemies(int nbOfEnemies)
    {
        numberOfEnemies.text = "Enemies: " + nbOfEnemies;
    }
    public void UpdateScore(int scoreToAdd)
    {
        Player.instance.score += scoreToAdd;
        scoreText.text = "Score: " + Player.instance.score;
    }
    public void UpdateLevel(int level)
    {
        levelText.text = "Level : " + level;
    }

    public void ToggleKeyIcon(bool toggle)
    {
        key.SetActive(toggle);
    }
}
