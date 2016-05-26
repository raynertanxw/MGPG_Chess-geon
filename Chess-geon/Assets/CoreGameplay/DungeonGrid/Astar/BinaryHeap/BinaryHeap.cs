using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HeapType
{
	MinHeap,
	MaxHeap
}

public class NodeBinaryHeap
{
	List<Node> mNodes;
	HashSet<Node> mHashNodes;

	public HeapType MinOrMax { get; private set; }
	public int Count { get { return mNodes.Count; } }

	public Node Root
	{
		get { return mNodes[0]; }
	}

	public NodeBinaryHeap(HeapType type)
	{
		mNodes = new List<Node>();
		mHashNodes = new HashSet<Node>();
		this.MinOrMax = type;
	}

	public void Insert(Node _node)
	{
		mNodes.Add(_node);
		mHashNodes.Add(_node);

		int index = mNodes.Count - 1;

		bool flag = true;
		if (MinOrMax == HeapType.MaxHeap)
			flag = false;

		while(index > 0)
		{
			if ((mNodes[index].CompareTo(mNodes[(index - 1) / 2]) > 0) ^ flag)
			{
				Node temp = mNodes[index];
				mNodes[index] = mNodes[(index - 1) / 2];
				mNodes[(index - 1) / 2] = temp;
				index = (index - 1) / 2;
			}
			else
				break;
		}
	}

	private void DeleteRoot()
	{
		int index = mNodes.Count - 1;

		mNodes[0] = mNodes[index];
		mNodes.RemoveAt(index);

		index = 0;

		bool flag = true;
		if (MinOrMax == HeapType.MaxHeap)
			flag = false;

		while(true)
		{
			int leftIndex = 2 * index + 1;
			int rightIndex = 2 * index + 2;
			int largest = index;

			if (leftIndex < mNodes.Count)
			{
				if ((mNodes[leftIndex].CompareTo(mNodes[largest]) > 0) ^ flag)
					largest = leftIndex;
			}

			if (rightIndex < mNodes.Count)
			{
				if ((mNodes[rightIndex].CompareTo(mNodes[largest]) > 0) ^ flag)
					largest = rightIndex;
			}

			if (largest != index)
			{
				Node temp = mNodes[largest];
				mNodes[largest] = mNodes[index];
				mNodes[index] = temp;
				index = largest;
			}
			else
				break;
		}
	}

	public Node PopRoot()
	{
		Node result = mNodes[0];
		mHashNodes.Remove(result);

		DeleteRoot();

		return result;
	}

	public void Clear()
	{
		mNodes.Clear();
		mHashNodes.Clear();
	}

	public bool Contains(Node _node)
	{
		return mHashNodes.Contains(_node);
	}
}
