using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerMenu : MonoBehaviour
{
    public void ToMainMenu()
    {
        SceneManager.LoadScene("Title_Scene");
    }
}