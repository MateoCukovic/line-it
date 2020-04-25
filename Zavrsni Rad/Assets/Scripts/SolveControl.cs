using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolveControl : MonoBehaviour
{
    private Sprite nodeWhite;
    private Sprite nodeRed;
    private Sprite nodeGreen;

    void Start()
    {
        nodeWhite = Resources.Load<Sprite>("Imports/nodeWhite");
        nodeRed = Resources.Load<Sprite>("Imports/nodeRed");
        nodeGreen = Resources.Load<Sprite>("Imports/nodeGreen");
    }

    void Update()
    {
        if(TouchControl.node_gameObjects != null)
        {
            CheckForConnectionsAmount();
        }    
        
        if(ConnectionsFilled())
        {
            if(DFS())
            {
                Debug.Log("Level passed");
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
            }
            else if(Generator.listOfNodes[numberOrderOfNode].connections > Generator.listOfNodes[numberOrderOfNode].degree)
            {
                TouchControl.node_gameObjects[numberOrderOfNode].gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = nodeRed;
            }
            else
            {
                TouchControl.node_gameObjects[numberOrderOfNode].gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = nodeWhite;
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
}
