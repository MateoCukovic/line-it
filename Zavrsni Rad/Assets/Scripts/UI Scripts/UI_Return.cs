using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Return : MonoBehaviour
{
    private static CanvasGroup canvasGroup;
    private static SpriteRenderer transparentBackground;

    public static bool UI_showing_Return;

    private void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        transparentBackground = GameObject.FindGameObjectWithTag("backgroundReturn").GetComponent<SpriteRenderer>();
        UI_showing_Return = false;

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
        UI_showing_Return = true;
    }

    public void Yes()
    {
        SceneManager.LoadScene(0);
    }

    public void No()
    {
        HideUI();
        UI_showing_Return = false;
    }
}
