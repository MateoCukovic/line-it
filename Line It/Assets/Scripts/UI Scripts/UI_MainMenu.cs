using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    private int screenWidth;
    private int screenHeight;

    private void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        LeanTween.moveY(gameObject, screenHeight * 0.5f, 0f);
        LeanTween.moveX(gameObject, screenWidth * 0.5f, 1f);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
}
