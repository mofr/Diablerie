using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathing {

	public struct Step {
		public Vector2 direction;
		public int directionIndex;
		public Vector2 pos;
	}

	static private List<Step> path = new List<Step>();

	class Node : IEquatable<Node>, IComparable<Node> {
		public float gScore;
		public float score;
		public Vector2 pos;
		public Node parent;
		public Vector2 direction;
		public int directionIndex;

		private Node() {
		}

		public int CompareTo(Node other) {
			return score.CompareTo(other.score);
		}

		public bool Equals(Node other) {
			return this.pos == other.pos;
		}
			
		static private List<Node> pool = new List<Node>();
		static public Node Get() {
			if (pool.Count > 0) {
				Node node = pool[0];
				pool.RemoveAt(0);
				return node;
			} else {
				return new Node();
			}
		}

		static public void Recycle(List<Node> nodes) {
			pool.AddRange(nodes);
			nodes.Clear();
		}

		public void Recycle() {
			pool.Add(this);
		}
	}
		
	static private Vector2 target;
	static private List<Node> openNodes = new List<Node>();
	static private List<Node> closeNodes = new List<Node>();
	static private Vector2[] directions;
	static private Vector2[] directions8 = { new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1) };
	static private Vector2[] directions16 = { new Vector2(-1, -1), new Vector2(-2, -1), new Vector2(-1, 0), new Vector2(-2, 1), new Vector2(-1, 1), new Vector2(-1, 2), new Vector2(0, 1), new Vector2(1, 2), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 0), new Vector2(2, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -1), new Vector2(-1, -2) };

	static private void StepTo(Node node) {
		closeNodes.Add(node);
		Node newNode = null;

		for (int i = 0; i < directions.Length; ++i) {
			Vector2 direction = directions[i];
			Vector2 pos = node.pos + direction;
			if (Tilemap.instance[pos]) {
				if (newNode == null)
					newNode = Node.Get();
				newNode.pos = pos;
				if (!closeNodes.Contains(newNode) && !openNodes.Contains(newNode)) {
					newNode.parent = node;
					newNode.direction = direction;
					newNode.directionIndex = i;
					newNode.gScore = node.gScore + direction.magnitude;
					newNode.score = newNode.gScore + Vector2.Distance(target, newNode.pos);
					openNodes.Add(newNode);
					newNode = null;
				}
			}
		}

		if (newNode != null)
			newNode.Recycle();
	}

	static private void TraverseBack(Node node) {
		while (node.parent != null) {
			Step step = new Step();
			step.direction = node.direction;
			step.directionIndex = node.directionIndex;
			step.pos = node.pos;
			path.Insert(0, step);
			node = node.parent;
		}
	}

	static public List<Step> BuildPath(Vector2 from, Vector2 target, int directionCount = 8, float minRange = 0.1f) {
        from = Iso.Snap(from);
        target = Iso.Snap(target);
        path.Clear();
        if (from == target)
            return path;
		Node.Recycle(openNodes);
		Node.Recycle(closeNodes);

        directions = directionCount == 8 ? directions8 : directions16;
        Pathing.target = target;
        Node startNode = Node.Get();
		startNode.parent = null;
		startNode.pos = from;
		startNode.gScore = 0;
		startNode.score = 999;
		openNodes.Add(startNode);
		int iterCount = 0;
		while (openNodes.Count > 0) {
			openNodes.Sort();
			Node node = openNodes[0];
			if (!Tilemap.instance[target] && node.parent != null && node.score > node.parent.score) {
				TraverseBack(node.parent);
				break;
			}
			if (Vector2.Distance(node.pos, target) <= minRange) {
				TraverseBack(node);
				break;
			}
			openNodes.RemoveAt(0);
			StepTo(node);
			iterCount += 1;
			if (iterCount > 100) {
                TraverseBack(node);
				break;
			}
		}
		foreach (Node node in closeNodes) {
			Iso.DebugDrawTile(node.pos, Color.magenta, 0.3f);
		}
		foreach (Node node in openNodes) {
			Iso.DebugDrawTile(node.pos, Color.green, 0.3f);
		}
		return path;
	}

	static public void DebugDrawPath(List<Step> path) {
		for (int i = 0; i < path.Count - 1; ++i) {
			Debug.DrawLine(Iso.MapToWorld(path[i].pos), Iso.MapToWorld(path[i + 1].pos));
		}
	}
}
