using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue
{
    public List<Node> nodes = new List<Node>();

    public int Length { get { return this.nodes.Count; } }

    public bool Contains(Node _node)
    {
        return this.nodes.Contains(_node);
    }

    public Node First()
    {
        if (this.nodes.Count > 0)
            return this.nodes[0];

        return null;
    }

    public void Push(Node _node)
    {
        this.nodes.Add(_node);
		this.nodes.Sort((n1, n2) => n1.totalCost.CompareTo(n2.totalCost));
    }

    public void Remove(Node _node)
    {
        this.nodes.Remove(_node);
		this.nodes.Sort((n1, n2) => n1.totalCost.CompareTo(n2.totalCost));
    }
}
