using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentGameManager : MonoBehaviour
{
    public static PersistentGameManager instance { get; private set; }

    public Player player;

    // Game settings
    public bool infiniteManaCheat = false;
    public float dustPileReward = 20f;
    public float bleachBombCost = 50f;
    public float dusterCost = 10f;

    private List<float> lastPlayerHealthValues = new List<float>();
    public float health;

    // Other persistent data or game settings can be added here

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        player = FindObjectOfType<Player>();

        SceneManager.sceneLoaded += OnSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneChanged;
    }

    private void Start()
    {
        //Debug.Log(health);
    }

    private void Update()
    {
        health = player.health;
    }

    private void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single)
        {
            // Retrieve the player object in the new scene
            player = FindObjectOfType<Player>();

            // Update the player's health with the value from the PersistentGameManager
            if (player != null)
            {
                if (lastPlayerHealthValues.Count > 0)
                {
                    int lastIndex = lastPlayerHealthValues.Count - 1;
                    float lastHealth = lastPlayerHealthValues[lastIndex];
                    player.health = lastHealth;
                }
                else
                {
                    player.health = 25f; // Default health value if the array is empty
                }
            }
        }
    }

    // Called by GameManager to push the current player's health to the array
    public void PushLastPlayerHealth(float healthValue)
    {
        lastPlayerHealthValues.Add(healthValue);
        Debug.Log(healthValue);
    }

    // Called by GameManager to get the last player's health from the array
    public float GetLastPlayerHealth()
    {
        if (lastPlayerHealthValues.Count > 0)
        {
            int lastIndex = lastPlayerHealthValues.Count - 1;
            return lastPlayerHealthValues[lastIndex];
        }
        else
        {
            return 25f; // Default health value if the array is empty
        }
    }
}
