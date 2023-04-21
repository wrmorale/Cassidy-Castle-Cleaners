using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControllerMenu : MonoBehaviour
{
    public void ToMainMenu()
    {
        SceneManager.LoadScene("Title_Scene");
    }
}