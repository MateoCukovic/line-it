using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using System.Linq;

public class Generator : MonoBehaviour
{
    // Unity specific objects
    List<GameObject> nodesToDestroy;
    [SerializeField] private GameObject nodeGameObject;
    private TextMeshPro degreeText_TMPro;

    // Generator objects
    public static List<Node> listOfNodes;
    private Stack<Node> selectedNodeStack;
    public static bool isDoneGenerating;

    // Generator parameters
    private int level = 10;
    private float minDistance = 0.5f;
    private float maxDistance = 2.5f;
    private float minDistanceInNeighbourFinding = 0.5f;
    private float maxDistanceInNeighbourFinding = 3.5f;
    private float offsetOfNodeAvoidance = 1f;
    private float gameAreaX = 4.5f;
    private float gameAreaY = 8f;

    // Control the expansion
    private int numberOfConnections;

    public static Dictionary<string, float> boundaryPositions;
    public static Dictionary<string, Node> farthestNodes;

    // Execute before first frame
    private void Start()
    {
        // Initialization
        nodesToDestroy = new List<GameObject>();
        listOfNodes = new List<Node>();
        selectedNodeStack = new Stack<Node>();

        isDoneGenerating = false;
        numberOfConnections = 0;

        boundaryPositions = new Dictionary<string, float>();
        farthestNodes = new Dictionary<string, Node>();

        degreeText_TMPro = nodeGameObject.transform.GetChild(1).GetComponent<TextMeshPro>();

        GenerateGraph();
    }

    private void GenerateGraph()
    {
        // Generate first node
        FirstNode();

        // Generate all other nodes
        while (selectedNodeStack.Any())
        {
            // connections < degree
            if (selectedNodeStack.Peek().connections < selectedNodeStack.Peek().degree)
            {
                List<Node> neighbouringNodesItMayConnectTo = FindNeighboursInGivenDistance(selectedNodeStack.Peek());

                for(int connections = selectedNodeStack.Peek().connections; connections < selectedNodeStack.Peek().degree; connections++)
                {
                    // Check out the neighbours
                    if(neighbouringNodesItMayConnectTo != null && !selectedNodeStack.Peek().checkedOutNeighbours)
                    {                        
                        for(int connectionsWithNeighbours = 0; connectionsWithNeighbours < neighbouringNodesItMayConnectTo.Count; connectionsWithNeighbours++)
                        {
                            bool isMeetingRotationDegreeCriteria = MeetRotationDegreeBetweenAdjacentNodes(neighbouringNodesItMayConnectTo[connectionsWithNeighbours].x, neighbouringNodesItMayConnectTo[connectionsWithNeighbours].y, selectedNodeStack.Peek());

                            if (selectedNodeStack.Peek().connections < selectedNodeStack.Peek().degree && isMeetingRotationDegreeCriteria)
                            {
                                if(neighbouringNodesItMayConnectTo[connectionsWithNeighbours].degree - neighbouringNodesItMayConnectTo[connectionsWithNeighbours].connections >= 2 && selectedNodeStack.Peek().requiredDoubleConnections > 0)
                                {
                                    if (selectedNodeStack.Peek().degree - selectedNodeStack.Peek().connections >= 2)
                                    {
                                        selectedNodeStack.Peek().adjacentNodes.Add(neighbouringNodesItMayConnectTo[connectionsWithNeighbours]);
                                        selectedNodeStack.Peek().doubleAdjacentNodes.Add(neighbouringNodesItMayConnectTo[connectionsWithNeighbours]);
                                        selectedNodeStack.Peek().neighbouringNodesForLineDrawing.Add(neighbouringNodesItMayConnectTo[connectionsWithNeighbours]);
                                        selectedNodeStack.Peek().connections += 2;

                                        neighbouringNodesItMayConnectTo[connectionsWithNeighbours].adjacentNodes.Add(selectedNodeStack.Peek());
                                        neighbouringNodesItMayConnectTo[connectionsWithNeighbours].doubleAdjacentNodes.Add(selectedNodeStack.Peek());
                                        neighbouringNodesItMayConnectTo[connectionsWithNeighbours].neighbouringNodesForLineDrawing.Add(selectedNodeStack.Peek());
                                        neighbouringNodesItMayConnectTo[connectionsWithNeighbours].connections += 2;

                                        numberOfConnections += 2;
                                    }
                                }
                                else
                                {
                                    selectedNodeStack.Peek().adjacentNodes.Add(neighbouringNodesItMayConnectTo[connectionsWithNeighbours]);
                                    selectedNodeStack.Peek().singleAdjacentNodes.Add(neighbouringNodesItMayConnectTo[connectionsWithNeighbours]);
                                    selectedNodeStack.Peek().neighbouringNodesForLineDrawing.Add(neighbouringNodesItMayConnectTo[connectionsWithNeighbours]);
                                    selectedNodeStack.Peek().connections++;

                                    neighbouringNodesItMayConnectTo[connectionsWithNeighbours].adjacentNodes.Add(selectedNodeStack.Peek());
                                    neighbouringNodesItMayConnectTo[connectionsWithNeighbours].singleAdjacentNodes.Add(selectedNodeStack.Peek());
                                    neighbouringNodesItMayConnectTo[connectionsWithNeighbours].neighbouringNodesForLineDrawing.Add(selectedNodeStack.Peek());
                                    neighbouringNodesItMayConnectTo[connectionsWithNeighbours].connections++;

                                    numberOfConnections++;
                                }                               
                            }
                        }

                        selectedNodeStack.Peek().checkedOutNeighbours = true;
                    }
                    // Become parent to new node
                    else
                    {
                        NthNode(selectedNodeStack.Peek());
                    }
                }
            }
            // Reached limit in neighbours
            else if (selectedNodeStack.Peek().adjacentNodes.Count == selectedNodeStack.Peek().degree)
            {
                selectedNodeStack.Pop();
            }
            // Change degree
            else if(selectedNodeStack.Peek().adjacentNodes.Count > selectedNodeStack.Peek().degree)
            {
                selectedNodeStack.Peek().degree = selectedNodeStack.Peek().adjacentNodes.Count;
            }
            // Unfulfilled conditions with no possible connections
            else
            {
                PreventInfiniteLoop();
            }
        }
        
        // Fixing the degrees
        for(int numberOrderOfNode = 0; numberOrderOfNode < listOfNodes.Count; numberOrderOfNode++)
        {
            listOfNodes[numberOrderOfNode].degree = listOfNodes[numberOrderOfNode].connections;
        }

        if (listOfNodes.Count > 4)
        // GUI
        {
            SpawnGraph();
            isDoneGenerating = true;

            FindBoundaryPositions();
        }    
        else
        {
            ResetGenerationForDevelopmentPurposes();
            GenerateGraph();
        }
            
    }

    private void SpawnGraph()
    {
        for (int numberOrder = 0; numberOrder < listOfNodes.Count; numberOrder++)
        {
            // Set text of degree
            //degreeText_TMPro.SetText(listOfNodes[numberOrder].degree.ToString());
            degreeText_TMPro.text = listOfNodes[numberOrder].degree.ToString();

            // Spawn nodes
            GameObject nodeClone = Instantiate(nodeGameObject, new Vector3(listOfNodes[numberOrder].x, listOfNodes[numberOrder].y, 0), Quaternion.Euler(0, 0, 0));
            nodesToDestroy.Add(nodeClone);

            #region FutureFeature
            /*
            List<GameObject> gObj_lines = new List<GameObject>();
            List<LineRenderer> lRndr_lines = new List<LineRenderer>();

            int numberOrderOfLine = 0;

            
            // Drawing lines between child and parent
            if (numberOrder > 0)
            {
                if(listOfNodes[numberOrder].doubleAdjacentNodes.Any())
                {
                    for(int doubles = 0; doubles < listOfNodes[numberOrder].doubleAdjacentNodes.Count; doubles++)
                    {
                        if(listOfNodes[numberOrder].doubleAdjacentNodes[doubles] == listOfNodes[numberOrder].parent)
                        {
                            // line parent-child
                            float k = -((listOfNodes[numberOrder].parent.y - listOfNodes[numberOrder].y) / (listOfNodes[numberOrder].parent.x - listOfNodes[numberOrder].x));
                            float l = k * listOfNodes[numberOrder].x + listOfNodes[numberOrder].y;

                            // parallels of double lines
                            float l1 = 0.1f * Mathf.Sqrt(Mathf.Pow(k, 2) + 1) + l;
                            float l2 = -0.1f * Mathf.Sqrt(Mathf.Pow(k, 2) + 1) + l;

                            // determinant, inverse on line and normal
                            float determinant = k * k + 1;

                            float k_inverse = 1 / determinant * k;
                            float y_inverse = -1 / determinant;
                            float k_normal_inverse = 1 / determinant;
                            float y_normal_inverse = 1 / determinant * k;

                            // parent, child
                            float l_parentNormal = -listOfNodes[numberOrder].parent.x + k * listOfNodes[numberOrder].parent.y;
                            float l_childNormal = -listOfNodes[numberOrder].x + k * listOfNodes[numberOrder].y;

                            // x, y of intersection
                            float x1_parent = k_inverse * l1 + y_inverse * l_parentNormal;
                            float y1_parent = k_normal_inverse * l1 + y_normal_inverse * l_parentNormal;
                            float x2_parent = k_inverse * l2 + y_inverse * l_parentNormal;
                            float y2_parent = k_normal_inverse * l2 + y_normal_inverse * l_parentNormal;
                            float x1_child = k_inverse * l1 + y_inverse * l_childNormal;
                            float y1_child = k_normal_inverse * l1 + y_normal_inverse * l_childNormal;
                            float x2_child = k_inverse * l2 + y_inverse * l_childNormal;
                            float y2_child = k_normal_inverse * l2 + y_normal_inverse * l_childNormal;

                            gObj_lines.Add(new GameObject());
                            gObj_lines.Add(new GameObject());

                            gObj_lines[numberOrderOfLine].AddComponent<LineRenderer>();
                            gObj_lines[numberOrderOfLine + 1].AddComponent<LineRenderer>();

                            gObj_lines[numberOrderOfLine].transform.SetParent(nodeClone.transform);
                            gObj_lines[numberOrderOfLine + 1].transform.SetParent(nodeClone.transform);

                            lRndr_lines.Add(new LineRenderer());
                            lRndr_lines.Add(new LineRenderer());

                            lRndr_lines[numberOrderOfLine] = gObj_lines[numberOrderOfLine].GetComponent<LineRenderer>();
                            lRndr_lines[numberOrderOfLine + 1] = gObj_lines[numberOrderOfLine + 1].GetComponent<LineRenderer>();

                            lRndr_lines[numberOrderOfLine].startWidth = 0.07f;
                            lRndr_lines[numberOrderOfLine].endWidth = 0.07f;
                            lRndr_lines[numberOrderOfLine + 1].startWidth = 0.07f;
                            lRndr_lines[numberOrderOfLine + 1].endWidth = 0.07f;

                            lRndr_lines[numberOrderOfLine].positionCount = 2;
                            lRndr_lines[numberOrderOfLine + 1].positionCount = 2;

                            lRndr_lines[numberOrderOfLine].SetPosition(0, new Vector3(x1_child, y1_child, 0));
                            lRndr_lines[numberOrderOfLine].SetPosition(1, new Vector3(x1_parent, y1_parent, 0));

                            lRndr_lines[numberOrderOfLine + 1].SetPosition(0, new Vector3(x2_child, y2_child, 0));
                            lRndr_lines[numberOrderOfLine + 1].SetPosition(1, new Vector3(x2_parent, y2_parent, 0));

                            numberOrderOfLine += 2;
                        }
                    }
                }
                else
                {
                    gObj_lines.Add(new GameObject());

                    gObj_lines[numberOrderOfLine].AddComponent<LineRenderer>();

                    gObj_lines[numberOrderOfLine].transform.SetParent(nodeClone.transform);

                    lRndr_lines.Add(new LineRenderer());

                    lRndr_lines[numberOrderOfLine] = gObj_lines[numberOrderOfLine].GetComponent<LineRenderer>();

                    lRndr_lines[numberOrderOfLine].positionCount = 2;
                    lRndr_lines[numberOrderOfLine].startWidth = 0.07f;
                    lRndr_lines[numberOrderOfLine].endWidth = 0.07f;

                    lRndr_lines[numberOrderOfLine].SetPosition(0, new Vector3(listOfNodes[numberOrder].x, listOfNodes[numberOrder].y, 0));
                    lRndr_lines[numberOrderOfLine].SetPosition(1, new Vector3(listOfNodes[numberOrder].parent.x, listOfNodes[numberOrder].parent.y, 0));

                    numberOrderOfLine++;
                }               
            }
            
            // Drawing lines between neighbouring nodes
            if(listOfNodes[numberOrder].doubleAdjacentNodes.Any())
            {
                for(int doubles = 0; doubles < listOfNodes[numberOrder].doubleAdjacentNodes.Count; doubles++)
                {
                    // line start-end node
                    float k = -((listOfNodes[numberOrder].doubleAdjacentNodes[doubles].y - listOfNodes[numberOrder].y) / (listOfNodes[numberOrder].doubleAdjacentNodes[doubles].x - listOfNodes[numberOrder].x));
                    float l = k * listOfNodes[numberOrder].x + listOfNodes[numberOrder].y;

                    // parallels of double lines
                    float l1 = 0.1f * Mathf.Sqrt(Mathf.Pow(k, 2) + 1) + l;
                    float l2 = -0.1f * Mathf.Sqrt(Mathf.Pow(k, 2) + 1) + l;

                    // determinant, inverse on line and normal
                    float determinant = k * k + 1;

                    float k_inverse = 1 / determinant * k;
                    float y_inverse = -1 / determinant;
                    float k_normal_inverse = 1 / determinant;
                    float y_normal_inverse = 1 / determinant * k;

                    // start, end
                    float l_startNormal = -listOfNodes[numberOrder].doubleAdjacentNodes[doubles].x + k * listOfNodes[numberOrder].doubleAdjacentNodes[doubles].y;
                    float l_endNormal = -listOfNodes[numberOrder].x + k * listOfNodes[numberOrder].y;

                    // x, y of intersection
                    float x1_start = k_inverse * l1 + y_inverse * l_startNormal;
                    float y1_start = k_normal_inverse * l1 + y_normal_inverse * l_startNormal;
                    float x2_start = k_inverse * l2 + y_inverse * l_startNormal;
                    float y2_start = k_normal_inverse * l2 + y_normal_inverse * l_startNormal;
                    float x1_end = k_inverse * l1 + y_inverse * l_endNormal;
                    float y1_end = k_normal_inverse * l1 + y_normal_inverse * l_endNormal;
                    float x2_end = k_inverse * l2 + y_inverse * l_endNormal;
                    float y2_end = k_normal_inverse * l2 + y_normal_inverse * l_endNormal;

                    gObj_lines.Add(new GameObject());
                    gObj_lines.Add(new GameObject());

                    gObj_lines[numberOrderOfLine].AddComponent<LineRenderer>();
                    gObj_lines[numberOrderOfLine + 1].AddComponent<LineRenderer>();

                    gObj_lines[numberOrderOfLine].transform.SetParent(nodeClone.transform);
                    gObj_lines[numberOrderOfLine + 1].transform.SetParent(nodeClone.transform);

                    lRndr_lines.Add(new LineRenderer());
                    lRndr_lines.Add(new LineRenderer());

                    lRndr_lines[numberOrderOfLine] = gObj_lines[numberOrderOfLine].GetComponent<LineRenderer>();
                    lRndr_lines[numberOrderOfLine + 1] = gObj_lines[numberOrderOfLine + 1].GetComponent<LineRenderer>();

                    lRndr_lines[numberOrderOfLine].startWidth = 0.07f;
                    lRndr_lines[numberOrderOfLine].endWidth = 0.07f;
                    lRndr_lines[numberOrderOfLine + 1].startWidth = 0.07f;
                    lRndr_lines[numberOrderOfLine + 1].endWidth = 0.07f;

                    lRndr_lines[numberOrderOfLine].positionCount = 2;
                    lRndr_lines[numberOrderOfLine + 1].positionCount = 2;

                    lRndr_lines[numberOrderOfLine].SetPosition(0, new Vector3(x1_start, y1_start, 0));
                    lRndr_lines[numberOrderOfLine].SetPosition(1, new Vector3(x1_end, y1_end, 0));

                    lRndr_lines[numberOrderOfLine + 1].SetPosition(0, new Vector3(x2_start, y2_start, 0));
                    lRndr_lines[numberOrderOfLine + 1].SetPosition(1, new Vector3(x2_end, y2_end, 0));

                    numberOrderOfLine += 2;
                }
            }
            if (listOfNodes[numberOrder].singleAdjacentNodes.Any())
            {
                for (int singles = 0; singles < listOfNodes[numberOrder].singleAdjacentNodes.Count; singles++)
                {
                    gObj_lines.Add(new GameObject());

                    gObj_lines[numberOrderOfLine].AddComponent<LineRenderer>();

                    gObj_lines[numberOrderOfLine].transform.SetParent(nodeClone.transform);

                    lRndr_lines.Add(new LineRenderer());

                    lRndr_lines[numberOrderOfLine] = gObj_lines[numberOrderOfLine].GetComponent<LineRenderer>();

                    lRndr_lines[numberOrderOfLine].positionCount = 2;
                    lRndr_lines[numberOrderOfLine].startWidth = 0.07f;
                    lRndr_lines[numberOrderOfLine].endWidth = 0.07f;

                    lRndr_lines[numberOrderOfLine].SetPosition(0, new Vector3(listOfNodes[numberOrder].x, listOfNodes[numberOrder].y, 0));
                    lRndr_lines[numberOrderOfLine].SetPosition(1, new Vector3(listOfNodes[numberOrder].singleAdjacentNodes[singles].x, listOfNodes[numberOrder].singleAdjacentNodes[singles].y, 0));

                    numberOrderOfLine++;
                }
            }
            */
            #endregion
        }
    }

    private void FirstNode()
    {
        Node node = new Node
        {
            x = 0,
            y = 0,
            degree = UnityEngine.Random.Range(3, 9)
        };

        switch (node.degree)
        {
            case 5:
                node.requiredDoubleConnections = 1;
                break;
            case 6:
                node.requiredDoubleConnections = 2;
                break;
            case 7:
                node.requiredDoubleConnections = 3;
                break;
            case 8:
                node.requiredDoubleConnections = 4;
                break;
        }

        selectedNodeStack.Push(node);
        listOfNodes.Add(node);
    }

    private void NthNode(Node parent)
    {
        Node node = new Node();

        if(listOfNodes.Count >= 2)
        {
            Tuple<float, float> coordinatesToSpawn = SetNodePosition(parent);

            if (coordinatesToSpawn != null)
            {
                node.x = coordinatesToSpawn.Item1;
                node.y = coordinatesToSpawn.Item2;
            }
            else
            {
                PreventInfiniteLoop();
                return;
            }         
        }
        
        else
        {
            int directionX = UnityEngine.Random.Range(0, 2);
            int directionY = UnityEngine.Random.Range(0, 2);
            float distanceX = UnityEngine.Random.Range(minDistance, maxDistance);
            float distanceY = UnityEngine.Random.Range(minDistance, maxDistance);

            switch (directionX)
            {
                case 0:
                    node.x = parent.x - distanceX;
                    break;
                case 1:
                    node.x = parent.x + distanceX;
                    break;
            }

            switch (directionY)
            {
                case 0:
                    node.y = parent.y - distanceY;
                    break;
                case 1:
                    node.y = parent.y + distanceY;
                    break;
            }
        }

        if(parent.requiredDoubleConnections > 0 && parent.degree - parent.connections >= 2)
        {
            node.degree = UnityEngine.Random.Range(2, 9);
            numberOfConnections += 2;

            node.doubleAdjacentNodes.Add(parent);
            parent.doubleAdjacentNodes.Add(node);

            node.connections += 2;
            parent.connections += 2;

            parent.requiredDoubleConnections--;
        }      
        else
        {
            if (numberOfConnections > level * 2)
            {
                node.degree = UnityEngine.Random.Range(1, 3);
            }
            else
            {
                node.degree = UnityEngine.Random.Range(5, 9);
            }
                
            numberOfConnections++;

            node.singleAdjacentNodes.Add(parent);
            parent.singleAdjacentNodes.Add(node);

            node.connections++;
            parent.connections++;
        }

        switch (node.degree)
        {
            case 5:
                node.requiredDoubleConnections = 1;
                break;
            case 6:
                node.requiredDoubleConnections = 2;
                break;
            case 7:
                node.requiredDoubleConnections = 3;
                break;
            case 8:
                node.requiredDoubleConnections = 4;
                break;
        }

        node.parent = parent;
        
        listOfNodes.Add(node);
        selectedNodeStack.Push(node);
        node.adjacentNodes.Add(parent);
        parent.adjacentNodes.Add(node);
    }

    private List<Node> FindNeighboursInGivenDistance(Node node)
    {
        List<Node> neighbouringNodes = new List<Node>();
        
        for(int counterOfNodes = 0; counterOfNodes < listOfNodes.Count; counterOfNodes++)
        {
            if(listOfNodes[counterOfNodes].adjacentNodes.Count < listOfNodes[counterOfNodes].degree && node.parent != listOfNodes[counterOfNodes])
            {
                float distance = Mathf.Sqrt(Mathf.Pow(listOfNodes[counterOfNodes].y - node.y, 2) + Mathf.Pow(listOfNodes[counterOfNodes].x - node.x, 2));

                if (distance >= minDistanceInNeighbourFinding && distance <= maxDistanceInNeighbourFinding)
                {
                    neighbouringNodes.Add(listOfNodes[counterOfNodes]);
                }
            }
        }

        if (!neighbouringNodes.Any()) 
            return null;

        return neighbouringNodes;
    }

    private Tuple<float, float> SetNodePosition(Node parent)
    {
        List<Tuple<float, float>> listOfPotentialPlacements = new List<Tuple<float, float>>();

        float offsetOfForLoop;

        // top left corner to down right corner search
        // x
        for (float x = -gameAreaX; x <= gameAreaX; x += offsetOfForLoop)
        {
            offsetOfForLoop = UnityEngine.Random.Range(0.25f, 1f);
            // y
            for (float y = gameAreaY; y >= -gameAreaY; y -= offsetOfForLoop)
            {
                offsetOfForLoop = UnityEngine.Random.Range(0.25f, 1f);

                float distance = Mathf.Sqrt(Mathf.Pow(parent.y - y, 2) + Mathf.Pow(parent.x - x, 2));

                if(distance >= minDistance && distance <= maxDistance)
                {
                    bool isAvoidingNodes = AvoidNodes(x, y);
                    bool isAvoidingNodeLineIntersections = AvoidNodeLineConnectionIntersections(x, y, parent);
                    bool isMeetingRotationDegreeCriteria = MeetRotationDegreeBetweenAdjacentNodes(x, y, parent);

                    if (isAvoidingNodes && isAvoidingNodeLineIntersections && isMeetingRotationDegreeCriteria)
                    {
                        Tuple<float, float> coordinatePair = new Tuple<float, float>(x, y);

                        listOfPotentialPlacements.Add(coordinatePair);
                    }
                }               
            }
        }

        if (!listOfPotentialPlacements.Any()) 
            return null;
        
        int randomIndexOfPotentialPlacements = UnityEngine.Random.Range(0, listOfPotentialPlacements.Count);

        return listOfPotentialPlacements[randomIndexOfPotentialPlacements];
    }

    private bool AvoidNodes(float x, float y)
    {
        bool isAllowedToSpawn = false;

        for (int numberOrderOfNode = 0; numberOrderOfNode < listOfNodes.Count; numberOrderOfNode++)
        {
            float distance = Mathf.Sqrt(Mathf.Pow(listOfNodes[numberOrderOfNode].y - y, 2) + Mathf.Pow(listOfNodes[numberOrderOfNode].x - x, 2));

            if (distance < offsetOfNodeAvoidance)
            {
                isAllowedToSpawn = false;
                return isAllowedToSpawn;
            }
            else
            {
                isAllowedToSpawn = true;
            }
        }

        return isAllowedToSpawn;
    }

    private bool AvoidNodeLineConnectionIntersections(float destinationXCoordinate, float destinationYCoordinate, Node sourceNode)
    {
        bool isAllowedToSpawn = false;

        // -kx + y
        float k1 = -((sourceNode.y - destinationYCoordinate) / (sourceNode.x - destinationXCoordinate));
        float y1 = 1;

        if (Double.IsInfinity(k1))
        {
            isAllowedToSpawn = false;
            return isAllowedToSpawn;
        }      

        // l 
        float l1 = k1 * sourceNode.x + sourceNode.y;

        for (int numberOrderOfNode = 1; numberOrderOfNode < listOfNodes.Count; numberOrderOfNode++)
        {
            // distance from node
            float distanceFromNode = Mathf.Sqrt(Mathf.Pow(listOfNodes[numberOrderOfNode].y - destinationYCoordinate, 2) + Mathf.Pow(listOfNodes[numberOrderOfNode].x - destinationXCoordinate, 2));

            if (distanceFromNode < offsetOfNodeAvoidance)
            {               
                isAllowedToSpawn = false;
                return isAllowedToSpawn;
            }

            for (int counterOfAdjacentNodes = 0; counterOfAdjacentNodes < listOfNodes[numberOrderOfNode].adjacentNodes.Count; counterOfAdjacentNodes++)
            {
                // -kx + y
                float k2 = -((listOfNodes[numberOrderOfNode].adjacentNodes[counterOfAdjacentNodes].y - listOfNodes[numberOrderOfNode].y) / (listOfNodes[numberOrderOfNode].adjacentNodes[counterOfAdjacentNodes].x - listOfNodes[numberOrderOfNode].x));
                float y2 = 1;

                if (Double.IsInfinity(k2))
                {
                    isAllowedToSpawn = false;
                    return isAllowedToSpawn;
                }

                // l
                float l2 = k2 * listOfNodes[numberOrderOfNode].x + listOfNodes[numberOrderOfNode].y;

                // distance from line
                float distanceOfNodeFromLine = Mathf.Abs(k2 * destinationXCoordinate + y2 * destinationYCoordinate - l2) / Mathf.Sqrt(Mathf.Pow(k2, 2) + Mathf.Pow(y2, 2));                

                if(distanceOfNodeFromLine < offsetOfNodeAvoidance)
                {
                    isAllowedToSpawn = false;
                    return isAllowedToSpawn;
                }

                // determinant, inverse
                float determinant = k1 * y2 - y1 * k2;

                if (determinant == 0)
                {
                    isAllowedToSpawn = false;
                    return isAllowedToSpawn;
                }

                float k1_inverse = 1 / determinant * y2;
                float y1_inverse = 1 / determinant * -y1;
                float k2_inverse = 1 / determinant * -k2;
                float y2_inverse = 1 / determinant * k1;

                // x, y of intersection
                float x = k1_inverse * l1 + y1_inverse * l2;
                float y = k2_inverse * l1 + y2_inverse * l2;

                int counter_intersectionX_less = 0;
                int counter_intersectionX_more = 0;
                int counter_intersectionX_equal = 0;
                int counter_intersectionY_less = 0;
                int counter_intersectionY_more = 0;
                int counter_intersectionY_equal = 0;

                List<float> listToCompare_X = new List<float>();
                listToCompare_X.Add(destinationXCoordinate);
                listToCompare_X.Add(sourceNode.x);
                listToCompare_X.Add(listOfNodes[numberOrderOfNode].x);
                listToCompare_X.Add(listOfNodes[numberOrderOfNode].adjacentNodes[counterOfAdjacentNodes].x);

                List<float> listToCompare_Y = new List<float>();
                listToCompare_Y.Add(destinationYCoordinate);
                listToCompare_Y.Add(sourceNode.y);
                listToCompare_Y.Add(listOfNodes[numberOrderOfNode].y);
                listToCompare_Y.Add(listOfNodes[numberOrderOfNode].adjacentNodes[counterOfAdjacentNodes].y);

                for (int counterX = 0; counterX < listToCompare_X.Count; counterX++)
                {
                    if (x < listToCompare_X[counterX])
                        counter_intersectionX_less++;

                    else if (x > listToCompare_X[counterX])
                        counter_intersectionX_more++;

                    else if (x == listToCompare_X[counterX])
                        counter_intersectionX_equal++;
                }

                for (int counterY = 0; counterY < listToCompare_Y.Count; counterY++)
                {
                    if (y < listToCompare_Y[counterY])
                        counter_intersectionY_less++;

                    else if (y > listToCompare_Y[counterY])
                        counter_intersectionY_more++;

                    else if (y == listToCompare_Y[counterY])
                        counter_intersectionY_equal++;
                }

                if ((counter_intersectionX_less == 2 && counter_intersectionX_more == 2 && counter_intersectionY_less == 2 && counter_intersectionY_more == 2) || counter_intersectionX_equal > 0 || counter_intersectionY_equal > 0)
                {
                    isAllowedToSpawn = false;
                    return isAllowedToSpawn;
                }
                else
                {                  
                    isAllowedToSpawn = true;
                }
            }  
        }

        return isAllowedToSpawn;
    }

    private bool MeetRotationDegreeBetweenAdjacentNodes(float destinationXCoordinate, float destinationYCoordinate, Node sourceNode)
    {
        bool isAllowedToSpawn = false;

        float k1 = (destinationYCoordinate - sourceNode.y) / (destinationXCoordinate - sourceNode.x);

        for(int numberOrderOfAdjacentNodes = 0; numberOrderOfAdjacentNodes < sourceNode.adjacentNodes.Count; numberOrderOfAdjacentNodes++)
        {
            float k2 = (sourceNode.adjacentNodes[numberOrderOfAdjacentNodes].y - sourceNode.y) / (sourceNode.adjacentNodes[numberOrderOfAdjacentNodes].x - sourceNode.x);

            float theta = Mathf.Atan(Mathf.Abs((k2 - k1) / (1 + k1 * k2)));

            if(theta < Mathf.PI / 4f || theta > 2 * Mathf.PI - Mathf.PI / 4f || (theta > 3 * Mathf.PI / 4 && theta < 5 * Mathf.PI / 4))
            {
                isAllowedToSpawn = false;
                return isAllowedToSpawn;
            }
            else
            {
                isAllowedToSpawn = true;
            }
        }

        return isAllowedToSpawn;
    }

    private void PreventInfiniteLoop()
    {
        // remove requirement of one double connection
        if(selectedNodeStack.Peek().degree >= 5)
        {
            selectedNodeStack.Peek().requiredDoubleConnections--;
        }

        // set the degree as it is at the moment
        selectedNodeStack.Peek().degree = selectedNodeStack.Peek().adjacentNodes.Count;
    }

    private void ResetGenerationForDevelopmentPurposes()
    {
        for(int i = 0; i < nodesToDestroy.Count; i++)
        {
            Destroy(nodesToDestroy[i]);
        }

        listOfNodes.Clear();
        nodesToDestroy.Clear();
        selectedNodeStack.Clear();
    }

    private void FindBoundaryPositions()
    {
        float minX = listOfNodes[0].x;
        float minY = listOfNodes[0].y;
        float maxX = listOfNodes[0].x;
        float maxY = listOfNodes[0].y;

        Node farthestNodeMinX = listOfNodes[0];
        Node farthestNodeMinY = listOfNodes[0];
        Node farthestNodeMaxX = listOfNodes[0];   
        Node farthestNodeMaxY = listOfNodes[0];

        for (int i = 0; i < listOfNodes.Count; i++)
        {
            if (listOfNodes[i].x < minX)
            {
                minX = listOfNodes[i].x;
                farthestNodeMinX = listOfNodes[i];
            }

            if (listOfNodes[i].y < minY)
            {
                minY = listOfNodes[i].y;
                farthestNodeMinY = listOfNodes[i];
            }

            if (listOfNodes[i].x > maxX)
            {
                maxX = listOfNodes[i].x;
                farthestNodeMaxX = listOfNodes[i];
            }

            if (listOfNodes[i].y > maxY)
            {
                maxY = listOfNodes[i].y;
                farthestNodeMaxY = listOfNodes[i];
            }
        }

        boundaryPositions.Add("minX", minX);
        boundaryPositions.Add("minY", minY);
        boundaryPositions.Add("maxX", maxX);
        boundaryPositions.Add("maxY", maxY);

        farthestNodes.Add("farMinX", farthestNodeMinX);
        farthestNodes.Add("farMinY", farthestNodeMinY);
        farthestNodes.Add("farMaxX", farthestNodeMaxX);
        farthestNodes.Add("farMaxY", farthestNodeMaxY);
    }
}
