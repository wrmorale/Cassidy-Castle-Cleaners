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

    // Mana related variables
    public float mana = 0;
    public float maxMana = 100f;

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
        health = player.health;

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
                player.health = health;
            }
        }
    }

    // Additional functions for PersistentGameManager can be added here
}
