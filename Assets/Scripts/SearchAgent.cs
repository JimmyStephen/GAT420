using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAgent : Agent
{

    [SerializeField] protected Node initialNode;

    public Node targetNode { get; set; }
    public GraphNode sourceNode { get; set; }
    public GraphNode destinationNode { get; set; }

    List<GraphNode> path = new List<GraphNode>();

    private void Start()
    {
        //get a random source and destination
        sourceNode = Node.GetRandomNode<GraphNode>();
        destinationNode = Node.GetRandomNode<GraphNode>();
        //set the target to the source
        targetNode = sourceNode;

        //Generate the path (who would have thought)
        GeneratePath();
    }

    void Update()
    {
/*        Debug.DrawLine(transform.position, sourceNode.transform.position, Color.green);
        Debug.DrawLine(transform.position, destinationNode.transform.position, Color.red);
        Debug.DrawLine(transform.position + Vector3.up, targetNode.transform.position + Vector3.up, Color.yellow);*/
        if (targetNode != null)
        {
            movement.MoveTowards(targetNode.transform.position);
        }
    }

    public GraphNode GetNextNode(GraphNode graphNode)
    {
        if (path.Count == 0) return null;
        int index = path.FindIndex(node => node == graphNode);
        if(index == path.Count - 1)
        {
            sourceNode = destinationNode;
            do
            {
                destinationNode = Node.GetRandomNode<GraphNode>();
            } while (sourceNode == destinationNode);
            GeneratePath();
            index = 0;
        }
        GraphNode nextNode = path[index];
        return nextNode;
    }

    private void GeneratePath()
    {
        GraphNode.ResetNodes();
        Search.BuildPath(Search.AStar, sourceNode, destinationNode, ref path);
    }
}
