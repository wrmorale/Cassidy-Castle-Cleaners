using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentGameManager : MonoBehaviour
{
    public static PersistentGameManager instance { get; private set; }

    public Player player;
    public GameManager gameManager;
    public MusicManager musicManager;

    // Game settings
    public bool infiniteManaCheat = false;
    public float dustPileReward = 20f;
    public float bleachBombCost = 20f;
    public float dusterCost = 10f;
    public float soapBarCost = 10f;
    public float mopCost = 20f;

    private List<float> lastPlayerHealthValues = new List<float>();
    private List<float> lastPlayerManaValues = new List<float>();
    public float health;
    public float mana;


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
        gameManager = FindObjectOfType<GameManager>();

        SceneManager.sceneLoaded += OnSceneChanged;
        Debug.LogWarning("Persistent Game Manager awake!");

        
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneChanged;
    }

    private void Start()
    {
        Debug.LogWarning("Persistent Game Manager start!");
        player = FindObjectOfType<Player>();
        gameManager = FindObjectOfType<GameManager>();


    }

    private void Update()
    {
        if (player != null)
        {
            health = player.health;
        }
        
        if (gameManager != null)
        {
            mana = gameManager.mana;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Title_Scene") // Replace "MenuScene" with the actual name of your menu scene
        {
            musicManager.StopMusic();
        }
        else{
            if (!musicManager.IsMusicPlaying()) // Check if music is not already playing
            {
                musicManager.playBattleMusic(); // Play the battle music
            }
        }
    }

    private void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single)
        {
            // Retrieve the player object in the new scene
            player = FindObjectOfType<Player>();
            gameManager = FindObjectOfType<GameManager>();
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

                if (lastPlayerManaValues.Count > 0)
                {
                    int lastIndex = lastPlayerManaValues.Count - 1;
                    float lastMana = lastPlayerManaValues[lastIndex];
                    gameManager.mana = lastMana;
                }
                else
                {
                    gameManager.mana = 0f; // Default mana value if the array is empty
                }
            }
        }
    }

    // Called by GameManager to push the current player's health to the array
    public void PushLastPlayerHealth(float healthValue, float manaValue)
    {
        lastPlayerHealthValues.Add(healthValue);
        lastPlayerManaValues.Add(manaValue);
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

    public float GetLastPlayerMana()
    {
        if (lastPlayerManaValues.Count > 0)
        {
            int lastIndex = lastPlayerManaValues.Count - 1;
            return lastPlayerManaValues[lastIndex];
        }
        else
        {
            return 0f; // Default mana value if the array is empty
        }
    }

    public void resetPlayerStats()
    {
        lastPlayerHealthValues.Clear();
        lastPlayerManaValues.Clear();
    }

}
