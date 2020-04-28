using UnityEngine;

public class UI_InGame : MonoBehaviour
{
    private static CanvasGroup canvasGroup;
    private static GameObject thisGameObject;

    public static bool restartLevel;

    private void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        thisGameObject = gameObject;

        restartLevel = false;

        ShowUI();
    }

    private void Update()
    {
        if(!UI_LevelPassed.UI_showing_Passed && !UI_Return.UI_showing_Return)
        {
            ShowUI();
        }
    }

    public static void HideUI()
    {
        LeanTween.moveX(thisGameObject.transform.GetChild(0).gameObject, 1290f, 0.5f);
        LeanTween.moveX(thisGameObject.transform.GetChild(1).gameObject, -210f, 0.5f);
    }

    public void ShowUI()
    {
        LeanTween.moveX(thisGameObject.transform.GetChild(0).gameObject, 1025f, 0.5f);
        LeanTween.moveX(thisGameObject.transform.GetChild(1).gameObject, 55f, 0.5f);
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
