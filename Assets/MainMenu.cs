using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayStart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ToCredits()
    {
        SceneManager.LoadScene("Credits_Scene");
    }

    public void ToControls()
    {
        SceneManager.LoadScene("Controls_Scene");
    }
}
