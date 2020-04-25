using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    // Generator and TouchControl dependent
    public float x, y;
    public int connections;

    // Generating the graph
    public List<Node> adjacentNodes = new List<Node>();
    public bool checkedOutNeighbours = false;
    public Node parent;
    public int degree;
    public int requiredDoubleConnections;

    // Lines for drawing
    public List<Node> singleAdjacentNodes = new List<Node>();
    public List<Node> doubleAdjacentNodes = new List<Node>();
    public List<Node> neighbouringNodesForLineDrawing = new List<Node>();

    // Player control
    public List<Node> adjacentPlayerSetNodes = new List<Node>();
}
