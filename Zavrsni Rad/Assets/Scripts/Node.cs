using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public float x, y;

    public List<Node> adjacentNodes = new List<Node>();
    public List<Node> singleAdjacentNodes = new List<Node>();
    public List<Node> doubleAdjacentNodes = new List<Node>();

    public int connections;
    public int requiredDoubleConnections;

    public List<Node> neighbouringNodesForLineDrawing = new List<Node>();
    public bool checkedOutNeighbours = false;

    public Node parent;

    public int degree;
}
