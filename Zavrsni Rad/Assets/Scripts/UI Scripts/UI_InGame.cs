using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_InGame : MonoBehaviour
{
    public static bool restartLevel;

    private void Start()
    {
        restartLevel = false;
    }

    public void RestartLevel()
    {
        restartLevel = true;
    }

    public void ReturnToMainMenu()
    {
        UI_Return.ShowUI();
    }
}
