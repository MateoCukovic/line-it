using System.Collections;
using UnityEngine;

public class UI_InGame : MonoBehaviour
{
    private static GameObject thisGameObject;
    private static int screenWidth;
    private int screenHeight;

    public static bool restartLevel;

    private static bool UI_Display;

    private void Start()
    {
        thisGameObject = gameObject;
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        StartCoroutine(UI_scaling());

        restartLevel = false;
        UI_Display = false;

        ShowUI();
    }

    private void Update()
    {
        if(!UI_LevelPassed.UI_showing_Passed && !UI_Return.UI_showing_Return && !UI_Display)
        {
            ShowUI();
        }
    }

    public static void HideUI()
    {
        LeanTween.moveX(thisGameObject.transform.GetChild(0).gameObject, screenWidth + screenWidth * 0.2f, 0.5f);
        LeanTween.moveX(thisGameObject.transform.GetChild(1).gameObject, -screenWidth * 0.2f, 0.5f);

        UI_Display = false;
    }

    public void ShowUI()
    {
        LeanTween.moveX(thisGameObject.transform.GetChild(0).gameObject, screenWidth * 0.95f, 0.5f);
        LeanTween.moveX(thisGameObject.transform.GetChild(1).gameObject, screenWidth * 0.05f, 0.5f);

        UI_Display = true;
    }

    public void RestartLevel()
    {
        restartLevel = true;
    }

    public void ReturnToMainMenu()
    {
        UI_Return.ShowUI();
    }

    private IEnumerator UI_scaling()
    {
        gameObject.transform.localScale = new Vector3(screenWidth, screenHeight);
        gameObject.transform.GetChild(0).localScale = new Vector3(1f / screenWidth, 1f / screenHeight);
        gameObject.transform.GetChild(1).localScale = new Vector3(1f / screenWidth, 1f / screenHeight);

        LeanTween.moveX(gameObject.transform.GetChild(0).gameObject, screenWidth + screenWidth * 0.2f, 0f);
        LeanTween.moveY(gameObject.transform.GetChild(0).gameObject, screenWidth * 0.05f, 0f);
        LeanTween.moveX(gameObject.transform.GetChild(1).gameObject, -screenWidth * 0.2f, 0f);
        LeanTween.moveY(gameObject.transform.GetChild(1).gameObject, screenHeight - screenWidth * 0.05f, 0f);

        yield return new WaitForSeconds(0.5f);
    }
}
