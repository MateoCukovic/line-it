using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Return : MonoBehaviour
{
    private static CanvasGroup canvasGroup;
    private static GameObject transparentBackground;
    private static GameObject thisGameObject;

    public static bool UI_showing_Return;

    private void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        transparentBackground = gameObject.transform.GetChild(0).gameObject;
        thisGameObject = gameObject;

        UI_showing_Return = false;

        HideUI();
    }

    private void HideUI()
    {
        LeanTween.alpha(transparentBackground, 0f, 0.5f);

        LeanTween.moveX(thisGameObject.gameObject.transform.GetChild(1).gameObject, 1620f, 0.5f);
        LeanTween.moveX(thisGameObject.gameObject.transform.GetChild(2).gameObject, -780f, 0.5f);
        LeanTween.moveX(thisGameObject.gameObject.transform.GetChild(3).gameObject, -300f, 0.5f);

        StartCoroutine(UI_timeOut());
    }

    public static void ShowUI()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        UI_showing_Return = true;

        LeanTween.alpha(transparentBackground, 1f, 0.5f);

        LeanTween.moveX(thisGameObject.gameObject.transform.GetChild(1).gameObject, 540f, 0.5f);
        LeanTween.moveX(thisGameObject.gameObject.transform.GetChild(2).gameObject, 300f, 0.5f);
        LeanTween.moveX(thisGameObject.gameObject.transform.GetChild(3).gameObject, 780f, 0.5f);

        UI_InGame.HideUI();
    }

    public void Yes()
    {
        HideUI();

        SceneManager.LoadScene(0);
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
}
