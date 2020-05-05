using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
    public static GameObject[] node_gameObjects;

    public enum Touching
    {
        No,
        Node,
        Area
    }

    private enum Drawing
    {
        No,
        Yes
    }

    private Touch touch;

    public static Touching isTouching;
    public static bool nodeTouchEnded;
    public static bool areaTouchEnded;

    private Drawing isDrawing;

    private List<BoxCollider2D> boxColliders2D;
    public static List<LineRenderer> lineRenderers;
    private List<GameObject> line_gameObjects;

    public static bool loadingFinished;

    // Execute before first frame
    void Start()
    {
        // Initialization
        boxColliders2D = new List<BoxCollider2D>();
        lineRenderers = new List<LineRenderer>();
        line_gameObjects = new List<GameObject>();

        if(node_gameObjects != null)
            Array.Clear(node_gameObjects, 0, node_gameObjects.Length);

        loadingFinished = false;

        isTouching = Touching.No;
        isDrawing = Drawing.No;
    }

    // Execute every frame
    void Update()
    {
        if(UI_InGame.restartLevel)
        {
            ResetLevel();
            UI_InGame.restartLevel = false;
            loadingFinished = true;
        }

        if (Generator.isDoneGenerating)
        {
            node_gameObjects = GameObject.FindGameObjectsWithTag("Node");
            ClearNodeObjectSettings();
            Generator.isDoneGenerating = false;
            loadingFinished = true;
        }

        // If UI not present
        if(!UI_LevelPassed.UI_showing_Passed && !UI_Return.UI_showing_Return)
        {
            // Drawing lines on touching the nodes
            if (node_gameObjects != null && Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                for (int numberOrderOfNode = 0; numberOrderOfNode < node_gameObjects.Length; numberOrderOfNode++)
                {
                    if (node_gameObjects[numberOrderOfNode].GetComponent<CircleCollider2D>().OverlapPoint(touchPosition) && areaTouchEnded && isDrawing == Drawing.No)
                    {
                        isTouching = Touching.Node;
                        nodeTouchEnded = false;

                        StartCoroutine(DrawLine(node_gameObjects[numberOrderOfNode], numberOrderOfNode));
                    }
                    else if (numberOrderOfNode == node_gameObjects.Length - 1 && nodeTouchEnded)
                    {
                        isTouching = Touching.Area;
                        areaTouchEnded = false;
                    }
                }
            }

            // Removing lines on touch
            if (boxColliders2D != null && Input.touchCount > 0 && touch.phase == TouchPhase.Ended)
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                for (int numberOrder = 0; numberOrder < boxColliders2D.Count; numberOrder++)
                {
                    if (boxColliders2D[numberOrder].GetComponent<BoxCollider2D>().OverlapPoint(touchPosition) && isDrawing == Drawing.No)
                    {
                        List<GameObject> nodeOverlaps = new List<GameObject>();

                        // Remove double line if exists
                        for (int i = 0; i < node_gameObjects.Length; i++)
                        {
                            if (node_gameObjects[i].GetComponent<CircleCollider2D>().OverlapPoint(lineRenderers[numberOrder].GetPosition(0)) ||
                               node_gameObjects[i].GetComponent<CircleCollider2D>().OverlapPoint(lineRenderers[numberOrder].GetPosition(1)))
                            {
                                nodeOverlaps.Add(node_gameObjects[i]);
                            }

                            if (i == node_gameObjects.Length - 1)
                            {
                                for (int j = 0; j < lineRenderers.Count; j++)
                                {
                                    if ((nodeOverlaps[0].GetComponent<CircleCollider2D>().OverlapPoint(lineRenderers[j].GetPosition(0)) ||
                                        nodeOverlaps[0].GetComponent<CircleCollider2D>().OverlapPoint(lineRenderers[j].GetPosition(1))) &&
                                        (nodeOverlaps[1].GetComponent<CircleCollider2D>().OverlapPoint(lineRenderers[j].GetPosition(0)) ||
                                        nodeOverlaps[1].GetComponent<CircleCollider2D>().OverlapPoint(lineRenderers[j].GetPosition(1))) &&
                                        !ReferenceEquals(lineRenderers[numberOrder], lineRenderers[j]))
                                    {
                                        RemoveNodeConnections(lineRenderers[j]);

                                        Destroy(boxColliders2D[j]);
                                        Destroy(lineRenderers[j].material);
                                        Destroy(lineRenderers[j]);
                                        Destroy(line_gameObjects[j]);

                                        boxColliders2D.RemoveAt(j);
                                        lineRenderers.RemoveAt(j);
                                        line_gameObjects.RemoveAt(j);
                                    }
                                }
                            }
                        }

                        RemoveNodeConnections(lineRenderers[numberOrder]);

                        Destroy(boxColliders2D[numberOrder]);
                        Destroy(lineRenderers[numberOrder].material);
                        Destroy(lineRenderers[numberOrder]);
                        Destroy(line_gameObjects[numberOrder]);

                        boxColliders2D.RemoveAt(numberOrder);
                        lineRenderers.RemoveAt(numberOrder);
                        line_gameObjects.RemoveAt(numberOrder);
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended || isTouching == Touching.No)
            {
                nodeTouchEnded = true;
                areaTouchEnded = true;
            }
        }
    }

    private IEnumerator DrawLine(GameObject node_gameObject, int numberOrder)
    {
        isDrawing = Drawing.Yes;

        GameObject line_gameObject = new GameObject();
        line_gameObject.AddComponent<LineRenderer>();
        line_gameObject.transform.SetParent(node_gameObject.transform);

        LineRenderer lineRenderer = line_gameObject.GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.07f;
        lineRenderer.endWidth = 0.07f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(0.52f, 0.55f, 0.55f);
        lineRenderer.endColor = new Color(0.52f, 0.55f, 0.55f);
        lineRenderer.useWorldSpace = false;

        lineRenderer.SetPosition(0, new Vector3(node_gameObject.transform.position.x, node_gameObject.transform.position.y, 0.105f));
        
        // Run until connected or finger released
        while(true)
        {
            Vector3 fingerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            fingerPosition.z = 0.105f;

            // Follow the finger
            lineRenderer.SetPosition(1, fingerPosition);

            for (int numberOrderOfNode = 0; numberOrderOfNode < node_gameObjects.Length; numberOrderOfNode++)
            {
                // Touched another node
                if (node_gameObjects[numberOrderOfNode].GetComponent<CircleCollider2D>().OverlapPoint(fingerPosition) && !ReferenceEquals(node_gameObjects[numberOrderOfNode], node_gameObject))
                {
                    while (true)
                    {
                        fingerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        fingerPosition.z = 0;

                        // Follow the finger
                        lineRenderer.SetPosition(1, fingerPosition);

                        if (touch.phase == TouchPhase.Ended)
                        {
                            if (lineRenderers.Any())
                            {
                                bool singleHappened = false;

                                for (int lineOrder = 0; lineOrder < lineRenderers.Count; lineOrder++)
                                {
                                    // Double line
                                    if ((lineRenderer.GetPosition(0) == lineRenderers[lineOrder].GetPosition(0) ||
                                        lineRenderer.GetPosition(0) == lineRenderers[lineOrder].GetPosition(1)) && 
                                        (node_gameObjects[numberOrderOfNode].GetComponent<CircleCollider2D>().OverlapPoint(lineRenderers[lineOrder].GetPosition(0)) || 
                                        node_gameObjects[numberOrderOfNode].GetComponent<CircleCollider2D>().OverlapPoint(lineRenderers[lineOrder].GetPosition(1))))
                                    {
                                        // line start-end node
                                        float k = -((lineRenderer.GetPosition(0).y - node_gameObjects[numberOrderOfNode].transform.position.y) / 
                                            (lineRenderer.GetPosition(0).x - node_gameObjects[numberOrderOfNode].transform.position.x));

                                        float l = k * node_gameObjects[numberOrderOfNode].transform.position.x + node_gameObjects[numberOrderOfNode].transform.position.y;

                                        // parallels of double lines
                                        float l1 = 0.12f * Mathf.Sqrt(Mathf.Pow(k, 2) + 1) + l;
                                        float l2 = -0.12f * Mathf.Sqrt(Mathf.Pow(k, 2) + 1) + l;

                                        // determinant, inverse on line and normal
                                        float determinant = k * k + 1;

                                        float k_inverse = 1 / determinant * k;
                                        float y_inverse = -1 / determinant;
                                        float k_normal_inverse = 1 / determinant;
                                        float y_normal_inverse = 1 / determinant * k;

                                        // start, end
                                        float l_startNormal = -lineRenderer.GetPosition(0).x + k * lineRenderer.GetPosition(0).y;
                                        float l_endNormal = -node_gameObjects[numberOrderOfNode].transform.position.x + k * node_gameObjects[numberOrderOfNode].transform.position.y;

                                        // x, y of intersection
                                        float x1_start = k_inverse * l1 + y_inverse * l_startNormal;
                                        float y1_start = k_normal_inverse * l1 + y_normal_inverse * l_startNormal;
                                        float x2_start = k_inverse * l2 + y_inverse * l_startNormal;
                                        float y2_start = k_normal_inverse * l2 + y_normal_inverse * l_startNormal;
                                        float x1_end = k_inverse * l1 + y_inverse * l_endNormal;
                                        float y1_end = k_normal_inverse * l1 + y_normal_inverse * l_endNormal;
                                        float x2_end = k_inverse * l2 + y_inverse * l_endNormal;
                                        float y2_end = k_normal_inverse * l2 + y_normal_inverse * l_endNormal;

                                        lineRenderer.SetPosition(0, new Vector3(x1_start, y1_start, 0.105f));
                                        lineRenderer.SetPosition(1, new Vector3(x1_end, y1_end, 0.105f));

                                        lineRenderers[lineOrder].SetPosition(0, new Vector3(x2_start, y2_start, 0.105f));
                                        lineRenderers[lineOrder].SetPosition(1, new Vector3(x2_end, y2_end, 0.105f));

                                        AddColliderToLine(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1), line_gameObject);

                                        lineRenderers.Add(lineRenderer);

                                        PopulateNodeObjectSettings(node_gameObject, node_gameObjects[numberOrderOfNode]);

                                        line_gameObjects.Add(line_gameObject);

                                        break;
                                    }

                                    // Single line
                                    else if (node_gameObjects[numberOrderOfNode].GetComponent<CircleCollider2D>().OverlapPoint(fingerPosition) && lineOrder == lineRenderers.Count - 1)
                                    {
                                        lineRenderer.SetPosition(1, new Vector3(node_gameObjects[numberOrderOfNode].transform.position.x, node_gameObjects[numberOrderOfNode].transform.position.y, 0.105f));

                                        AddColliderToLine(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1), line_gameObject);

                                        lineRenderers.Add(lineRenderer);

                                        PopulateNodeObjectSettings(node_gameObject, node_gameObjects[numberOrderOfNode]);

                                        line_gameObjects.Add(line_gameObject);

                                        Generator.listOfNodes[numberOrder].adjacentPlayerSetNodes.Add(Generator.listOfNodes[numberOrderOfNode]);
                                        Generator.listOfNodes[numberOrderOfNode].adjacentPlayerSetNodes.Add(Generator.listOfNodes[numberOrder]);

                                        singleHappened = true;

                                        break;
                                    }

                                    // DEBUGGING
                                    if(singleHappened)
                                    {
                                        if(lineRenderer.GetPosition(1).x != node_gameObjects[numberOrderOfNode].transform.position.x || lineRenderer.GetPosition(1).y != node_gameObjects[numberOrderOfNode].transform.position.y)
                                        {
                                            Debug.Log("Overreach");

                                            lineRenderer.SetPosition(1, new Vector3(node_gameObjects[numberOrderOfNode].transform.position.x, node_gameObjects[numberOrderOfNode].transform.position.y, 0.105f));
                                        }
                                    }
                                }

                                break;
                            }

                            // First line, Single line
                            else if (node_gameObjects[numberOrderOfNode].GetComponent<CircleCollider2D>().OverlapPoint(fingerPosition))
                            {
                                lineRenderer.SetPosition(1, new Vector3(node_gameObjects[numberOrderOfNode].transform.position.x, node_gameObjects[numberOrderOfNode].transform.position.y, 0));

                                AddColliderToLine(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1), line_gameObject);

                                lineRenderers.Add(lineRenderer);

                                PopulateNodeObjectSettings(node_gameObject, node_gameObjects[numberOrderOfNode]);

                                line_gameObjects.Add(line_gameObject);

                                Generator.listOfNodes[numberOrder].adjacentPlayerSetNodes.Add(Generator.listOfNodes[numberOrderOfNode]);
                                Generator.listOfNodes[numberOrderOfNode].adjacentPlayerSetNodes.Add(Generator.listOfNodes[numberOrder]);

                                break;
                            }

                            break;
                        }
                        // Finger moved away from node
                        else if(!node_gameObjects[numberOrderOfNode].GetComponent<CircleCollider2D>().OverlapPoint(fingerPosition))
                        {
                            break;
                        }

                        yield return null;
                    }

                    break;
                }

                else if(touch.phase == TouchPhase.Ended && !node_gameObjects[numberOrderOfNode].GetComponent<CircleCollider2D>().OverlapPoint(fingerPosition))
                {
                    Destroy(line_gameObject);
                    Destroy(lineRenderer.material);
                    Destroy(lineRenderer);

                    break;
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                isDrawing = Drawing.No;

                yield break;
            }

            yield return null;
        }
    }

    private void ClearNodeObjectSettings()
    {
        for(int i = 0; i < Generator.listOfNodes.Count; i++)
        {
            Generator.listOfNodes[i].connections = 0;
        }
    }

    private void AddColliderToLine(Vector3 startPosition, Vector3 endPosition, GameObject line_gameObject)
    {
        BoxCollider2D boxCollider2D = new GameObject("LineCollider").AddComponent<BoxCollider2D>();
        boxCollider2D.transform.parent = line_gameObject.transform;

        float lineLength = Vector3.Distance(startPosition, endPosition);
        boxCollider2D.size = new Vector3(lineLength, 0.175f, 0);

        Vector3 middlePoint = (startPosition + endPosition) / 2;
        boxCollider2D.transform.position = middlePoint;

        float angle = Mathf.Abs((startPosition.y - endPosition.y) / (startPosition.x - endPosition.x));

        if ((startPosition.y < endPosition.y && startPosition.x > endPosition.x) || (endPosition.y < startPosition.y && endPosition.x > startPosition.x))
        {
            angle *= -1;
        }

        angle = Mathf.Rad2Deg * Mathf.Atan(angle);
        boxCollider2D.transform.Rotate(0, 0, angle);

        boxColliders2D.Add(boxCollider2D);
    }

    private void PopulateNodeObjectSettings(GameObject node1, GameObject node2)
    {
        for(int i = 0; i < node_gameObjects.Length; i++)
        {
            if(node1 == node_gameObjects[i])
            {
                Generator.listOfNodes[i].connections++;
            }
            else if (node2 == node_gameObjects[i])
            {
                Generator.listOfNodes[i].connections++;
            }
        }
    }

    private void RemoveNodeConnections(LineRenderer lineRenderer)
    {
        for(int i = 0; i < node_gameObjects.Length; i++)
        {
            if(node_gameObjects[i].GetComponent<CircleCollider2D>().OverlapPoint(lineRenderer.GetPosition(0)) ||
                node_gameObjects[i].GetComponent<CircleCollider2D>().OverlapPoint(lineRenderer.GetPosition(1)))
            {
                Generator.listOfNodes[i].connections--;
            }
        }
    }

    private void ResetLevel()
    {
        for(int i = 0; i < boxColliders2D.Count; i++)
        {
            RemoveNodeConnections(lineRenderers[i]);

            Destroy(boxColliders2D[i]);
            Destroy(lineRenderers[i].material);
            Destroy(lineRenderers[i]);
            Destroy(line_gameObjects[i]);
        }

        boxColliders2D.Clear();
        lineRenderers.Clear();
        line_gameObjects.Clear();
    }
}
