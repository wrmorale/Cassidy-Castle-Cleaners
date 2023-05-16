using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
    }

    private void Start()
    {
        health = player.maxHealth;
    }

    // Additional functions for PersistentGameManager can be added here
}
