using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Return : MonoBehaviour
{
    private static CanvasGroup canvasGroup;
    private static GameObject transparentBackground;
    private static GameObject thisGameObject;

    private static int screenWidth;
    private int screenHeight;

    public static bool UI_showing_Return;

    private void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        transparentBackground = gameObject.transform.GetChild(0).gameObject;
        thisGameObject = gameObject;
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        LeanTween.moveY(thisGameObject.gameObject.transform.GetChild(1).gameObject, screenHeight * 0.65f, 0f);
        LeanTween.moveY(thisGameObject.gameObject.transform.GetChild(2).gameObject, screenHeight * 0.5f, 0f);
        LeanTween.moveY(thisGameObject.gameObject.transform.GetChild(3).gameObject, screenHeight * 0.5f, 0f);

        UI_showing_Return = false;

        HideUI();
    }

    private void HideUI()
    {
        LeanTween.alpha(transparentBackground, 0f, 0.5f);

        LeanTween.moveX(thisGameObject.gameObject.transform.GetChild(1).gameObject, screenWidth + screenWidth * 0.5f, 0.5f);
        LeanTween.moveX(thisGameObject.gameObject.transform.GetChild(2).gameObject, -screenWidth * 0.7f, 0.5f);
        LeanTween.moveX(thisGameObject.gameObject.transform.GetChild(3).gameObject, -screenWidth * 0.3f, 0.5f);

        StartCoroutine(UI_timeOut());
    }

    public static void ShowUI()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        UI_showing_Return = true;

        LeanTween.alpha(transparentBackground, 1f, 0.5f);

        LeanTween.moveX(thisGameObject.gameObject.transform.GetChild(1).gameObject, screenWidth * 0.5f, 0.5f);
        LeanTween.moveX(thisGameObject.gameObject.transform.GetChild(2).gameObject, screenWidth * 0.3f, 0.5f);
        LeanTween.moveX(thisGameObject.gameObject.transform.GetChild(3).gameObject, screenWidth * 0.7f, 0.5f);

        UI_InGame.HideUI();
    }

    public void Yes()
    {
        HideUI();

        StartCoroutine(ChangeSceneDelay(0));
    }

    public void No()
    {
        HideUI();

        UI_showing_Return = false;
    }

    private IEnumerator UI_timeOut()
    {
        yield return new WaitForSeconds(0.5f);

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator ChangeSceneDelay(int sceneIndex)
    {
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(sceneIndex);
    }
}
