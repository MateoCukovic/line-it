using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private RawImage videoUI;

    private void Start()
    {
        videoUI.enabled = true;

        LeanTween.moveX(gameObject, 540f, 1f).setEaseInQuad();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
}
