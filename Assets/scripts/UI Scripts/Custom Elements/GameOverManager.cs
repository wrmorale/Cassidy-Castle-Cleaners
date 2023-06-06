using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("Title_Scene");
    }

    public void ToCredits()
    {
        SceneManager.LoadScene("Credits_Scene");
    }
}
