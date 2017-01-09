using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathing {

	static private List<Vector2> path = new List<Vector2>();

	static public List<Vector2> BuildPathNoCollision(Vector2 from, Vector2 target) {
		Vector2 startPos = from;
		path.Clear();

		startPos.x = Mathf.Round(startPos.x);
		startPos.y = Mathf.Round(startPos.y);
		Vector2 dir = target - startPos;
		dir.x = dir.x / Mathf.Abs(dir.x);
		dir.y = dir.y / Mathf.Abs(dir.y);

		float x = startPos.x;
		float y = startPos.y;
		while (x != target.x || y != target.y) {
			Vector2 step = new Vector2(0, 0);
			if (x != target.x) {
				step += new Vector2(dir.x, 0);
				x += dir.x;
			}
			if (y != target.y) {
				step += new Vector2(0, dir.y);
				y += dir.y;
			}
			path.Add(step);
			if (path.Count > 100) {
				Debug.Log("Infinite x path building");
				return path;
			}
		}

		return path;
	}
		
	class Node : IEquatable<Node>, IComparable<Node> {
		public float score;
		public Vector2 pos;
		public Node parent;
		public Vector2 direction;

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
	static private Vector2[] directions = { new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1) };

	static private void StepTo(Node node) {
		closeNodes.Add(node);
		Node newNode = null;

		foreach (Vector2 direction in directions) {
			Vector2 pos = node.pos + direction;
			if (Tilemap.instance[pos]) {
				if (newNode == null)
					newNode = Node.Get();
				newNode.pos = pos;
				if (!closeNodes.Contains(newNode) && !openNodes.Contains(newNode)) {
					newNode.score = CalcScore(target, newNode.pos);
					newNode.parent = node;
					newNode.direction = direction;
					openNodes.Add(newNode);
					newNode = null;
				}
			}
		}

		if (newNode != null)
			newNode.Recycle();
	}

	static private float CalcScore(Vector2 src, Vector2 target) {
		return 1 + Vector2.Distance(src, target);
	}

	static private void TraverseBack(Node node) {
		while (node.parent != null) {
			path.Insert(0, node.direction);
			node = node.parent;
		}
	}

	static public List<Vector2> BuildPath(Vector2 from, Vector2 target) {
		Pathing.target = target;
		Node.Recycle(openNodes);
		Node.Recycle(closeNodes);
		path.Clear();
		if (from == target)
			return path;
		Node startNode = Node.Get();
		startNode.parent = null;
		startNode.pos = from;
		startNode.score = CalcScore(from, target);
		openNodes.Add(startNode);
		int iterCount = 0;
		while (openNodes.Count > 0) {
			openNodes.Sort();
			Node node = openNodes[0];
			if (!Tilemap.instance[target] && node.parent != null && node.score >= node.parent.score) {
				TraverseBack(node.parent);
				break;
			}
			if (node.pos == target) {
				TraverseBack(node);
				break;
			}
			openNodes.RemoveAt(0);
			StepTo(node);
			iterCount += 1;
			if (iterCount > 1000) {
				Debug.LogWarning("Too much path iterations");
				Debug.Break();
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

	static public void DebugDrawPath(Vector2 start, List<Vector2> path) {
		Vector3 pos = start;
		foreach (Vector3 step in path) {
			Debug.DrawLine(Iso.MapToWorld(pos), Iso.MapToWorld(pos + step));
			pos += step;
		}
	}
}
