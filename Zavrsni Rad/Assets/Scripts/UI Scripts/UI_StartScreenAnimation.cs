using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartScreenAnimation : MonoBehaviour
{
    private int screenWidth;
    private int screenHeight;

    public Sprite nodeGreen;
    public Sprite nodeRed;

    public Sprite lineGreen;
    public Sprite lineRed;
    public Sprite lineGreenRed;

    public GameObject letters;
    public GameObject nodeLeft;
    public GameObject nodeRight;
    public GameObject lineLeft;
    public GameObject lineRight;
    public GameObject lineT;
    public GameObject lineConnect;

    void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        StartScreenAnimation();
    }

    private void StartScreenAnimation()
    {
        letters.transform.position = new Vector3(screenWidth * 1.5f, screenHeight * 0.75f);
        nodeLeft.transform.position = new Vector3(screenWidth * 1.45f, screenHeight * 0.8f);
        nodeRight.transform.position = new Vector3(screenWidth * 1.6f, screenHeight * 0.8f);

        lineLeft.transform.position = new Vector3(screenWidth * 0.315f, screenHeight * 0.5f);
        lineRight.transform.position = new Vector3(screenWidth * 0.715f, screenHeight * 0.5f);
        lineT.transform.position = new Vector3(screenWidth * 2f, screenHeight * 0.8f);
        lineConnect.transform.position = new Vector3(screenWidth * 0.34f, screenHeight * 0.8f);

        LeanTween.moveX(letters, screenWidth * 0.5f, 0.25f);
        LeanTween.moveX(nodeLeft, screenWidth * 0.315f, 0.25f);
        LeanTween.moveX(nodeRight, screenWidth * 0.715f, 0.25f);

        LeanTween.alphaCanvas(lineLeft.GetComponent<CanvasGroup>(), 1f, 0.5f);
        LeanTween.alphaCanvas(lineRight.GetComponent<CanvasGroup>(), 1f, 0.5f);
        LeanTween.alphaCanvas(lineT.GetComponent<CanvasGroup>(), 1f, 0.5f);
        LeanTween.moveY(lineLeft, screenHeight * 0.735f, 0.5f);
        LeanTween.moveY(lineRight, screenHeight * 0.735f, 0.5f);
        LeanTween.moveX(lineT, screenWidth * 0.8f, 0.5f);

        StartCoroutine(LineWaveDelay());
    }

    private IEnumerator LineWaveDelay()
    {
        yield return new WaitForSeconds(0.5f);

        nodeRight.GetComponent<Image>().sprite = nodeGreen;
        lineRight.GetComponent<Image>().sprite = lineGreen;
        lineT.GetComponent<Image>().sprite = lineGreen;

        yield return new WaitForSeconds(0.2f);

        LeanTween.scaleX(lineConnect, 1.5f, 0.3f);

        yield return new WaitForSeconds(0.3f);

        nodeLeft.GetComponent<Image>().sprite = nodeGreen;
        nodeRight.GetComponent<Image>().sprite = nodeRed;
        lineLeft.GetComponent<Image>().sprite = lineGreen;
        lineRight.GetComponent<Image>().sprite = lineRed;
        lineT.GetComponent<Image>().sprite = lineRed;
        lineConnect.GetComponent<Image>().sprite = lineGreenRed;
    }
}
