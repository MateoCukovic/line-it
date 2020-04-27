using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SolveControl : MonoBehaviour
{
    private Sprite nodeGrey;
    private Sprite nodeRed;
    private Sprite nodeGreen;

    private Color grey = new Color(0.52f, 0.55f, 0.55f);
    private Color red = new Color(0.71f, 0.02f, 0.02f);
    private Color green = new Color(0.20f, 0.71f, 0.015f);

    private bool UI_happened = false;

    void Start()
    {
        nodeGrey = Resources.Load<Sprite>("Imports/nodeGrey");
        nodeRed = Resources.Load<Sprite>("Imports/nodeRed");
        nodeGreen = Resources.Load<Sprite>("Imports/nodeGreen");
    }

    void Update()
    {
        if(TouchControl.loadingFinished)
        {
            CheckForConnectionsAmount();
        }    
        
        if(ConnectionsFilled())
        {
            if(DFS() && !UI_happened)
            {
                StartCoroutine(UI_Display());  
            }
        }
    }

    private void CheckForConnectionsAmount()
    {
        for(int numberOrderOfNode = 0; numberOrderOfNode < TouchControl.node_gameObjects.Length; numberOrderOfNode++)
        {
            if(Generator.listOfNodes[numberOrderOfNode].connections == Generator.listOfNodes[numberOrderOfNode].degree)
            {
                TouchControl.node_gameObjects[numberOrderOfNode].gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = nodeGreen;

                LineColoring(numberOrderOfNode, green);
            }
            else if(Generator.listOfNodes[numberOrderOfNode].connections > Generator.listOfNodes[numberOrderOfNode].degree)
            {
                TouchControl.node_gameObjects[numberOrderOfNode].gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = nodeRed;

                LineColoring(numberOrderOfNode, red);
            }
            else
            {
                TouchControl.node_gameObjects[numberOrderOfNode].gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = nodeGrey;

                LineColoring(numberOrderOfNode, grey);
            }
        }
    }

    private bool ConnectionsFilled()
    {
        bool isConnected = false;

        for(int numberOrder = 0; numberOrder < Generator.listOfNodes.Count; numberOrder++)
        {
            if(Generator.listOfNodes[numberOrder].connections == Generator.listOfNodes[numberOrder].degree)
            {
                isConnected = true;
            }
            else
            {
                return false;
            }
        }

        return isConnected;
    }

    private bool DFS()
    {
        bool isCompleted = false;

        Stack<Node> stackOfNodes = new Stack<Node>();
        HashSet<Node> seenNodes = new HashSet<Node>();

        stackOfNodes.Push(Generator.listOfNodes[0]);

        while(stackOfNodes.Any())
        {
            Node currentNode = stackOfNodes.Pop();

            if(!seenNodes.Contains(currentNode))
            {
                seenNodes.Add(currentNode);
            }

            for(int adjacentNodes = 0; adjacentNodes < currentNode.adjacentPlayerSetNodes.Count; adjacentNodes++)
            {
                if(!seenNodes.Contains(currentNode.adjacentPlayerSetNodes[adjacentNodes]))
                {
                    stackOfNodes.Push(currentNode.adjacentPlayerSetNodes[adjacentNodes]);
                }
            }
        }

        for(int i = 0; i < Generator.listOfNodes.Count; i++)
        {
            if(seenNodes.Contains(Generator.listOfNodes[i]))
            {
                isCompleted = true;
            }
            else
            {
                return false;
            }
        }

        return isCompleted;
    }

    private void LineColoring(int numberOrder, Color color)
    {
        Gradient gradient = new Gradient();

        for (int i = 0; i < TouchControl.lineRenderers.Count; i++)
        {
            if (TouchControl.node_gameObjects[numberOrder].GetComponent<CircleCollider2D>().OverlapPoint(TouchControl.lineRenderers[i].GetPosition(0)))
            {
                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(TouchControl.lineRenderers[i].endColor, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
                );

                TouchControl.lineRenderers[i].colorGradient = gradient;
            }
            else if (TouchControl.node_gameObjects[numberOrder].GetComponent<CircleCollider2D>().OverlapPoint(TouchControl.lineRenderers[i].GetPosition(1)))
            {
                TouchControl.lineRenderers[i].endColor = color;

                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(TouchControl.lineRenderers[i].startColor, 0.0f), new GradientColorKey(color, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
                );

                TouchControl.lineRenderers[i].colorGradient = gradient;
            }
        }
    }

    private IEnumerator UI_Display()
    {
        yield return new WaitForSeconds(1f);
        UI_LevelPassed.ShowUI();
    }
}
