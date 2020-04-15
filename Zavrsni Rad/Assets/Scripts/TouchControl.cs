using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
    public GameObject[] nodeGameObjects;

    public enum Touching
    {
        None,
        Node,
        Area
    }

    private Touch touch;

    public static Touching isTouching;
    public static bool nodeTouchEnded;
    public static bool areaTouchEnded;

    // Execute before first frame
    void Start()
    {
        isTouching = Touching.None;
    }

    // Execute every frame
    void Update()
    {
        if (Generator.isDoneGenerating)
        {
            nodeGameObjects = GameObject.FindGameObjectsWithTag("Node");
        }

        if (nodeGameObjects != null && Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

            for (int numberOrderOfNode = 0; numberOrderOfNode < nodeGameObjects.Length; numberOrderOfNode++)
            {
                if (nodeGameObjects[numberOrderOfNode].GetComponent<CircleCollider2D>().OverlapPoint(touchPosition) && areaTouchEnded)
                {
                    isTouching = Touching.Node;
                    nodeTouchEnded = false;
                }
                else if (numberOrderOfNode == nodeGameObjects.Length - 1 && nodeTouchEnded)
                {
                    isTouching = Touching.Area;
                    areaTouchEnded = false;
                }
            }
        }

        if (touch.phase == TouchPhase.Ended || isTouching == Touching.None)
        {
            nodeTouchEnded = true;
            areaTouchEnded = true;
        }
    }
}
