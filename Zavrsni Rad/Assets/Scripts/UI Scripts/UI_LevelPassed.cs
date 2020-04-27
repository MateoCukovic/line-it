using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelPassed : MonoBehaviour
{
    private static CanvasGroup canvasGroup;
    private static SpriteRenderer transparentBackground;

    public static bool UI_showing_Passed;

    private void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        transparentBackground = GameObject.FindGameObjectWithTag("backgroundPassed").GetComponent<SpriteRenderer>();
        UI_showing_Passed = false;

        HideUI();
    }

    private void HideUI()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        transparentBackground.enabled = false;
    }

    public static void ShowUI()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transparentBackground.enabled = true;
        UI_showing_Passed = true;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
