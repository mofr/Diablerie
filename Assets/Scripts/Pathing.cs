using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathing {

	public struct Step {
		public Vector2 direction;
		public Vector2 pos;
	}

	class Node : IEquatable<Node>, IComparable<Node>
    {
        const int InitialCapacity = 10 * 1024;

		public int gScore;
        public int hScore;
        public int score;
		public Vector2i pos;
		public Node parent;
        public int directionIndex;

		private Node()
        {
		}

		public int CompareTo(Node other)
        {
			return score.CompareTo(other.score);
		}

		public bool Equals(Node other)
        {
			return pos == other.pos;
		}

        override public int GetHashCode()
        {
            return (pos.x * 73856093) ^ (pos.y * 83492791);
        }

        static private List<Node> pool = new List<Node>(InitialCapacity);

        static Node()
        {
            for(int i = 0; i < InitialCapacity; ++i)
            {
                pool.Add(new Node());
            }
        }

		static public Node Get()
        {
            Node node;
            if (pool.Count > 0)
            {
                int last = pool.Count - 1;
				node = pool[last];
				pool.RemoveAt(last);
			}
            else
            {
				node = new Node();
			}
            return node;
        }

		static public void Recycle(ICollection<Node> nodes)
        {
            pool.AddRange(nodes);
			nodes.Clear();
        }

		public void Recycle()
        {
            pool.Add(this);
        }
	}

    static private List<Step> path = new List<Step>();
    static private Vector2i target;
	static private BinaryHeap<Node> openNodes = new BinaryHeap<Node>(4096);
	static private HashSet<Node> closeNodes = new HashSet<Node>();
	static private Vector2i[] directions = {
        new Vector2i(1, 0), new Vector2i(1, 1), new Vector2i(0, 1), new Vector2i(-1, 1),
        new Vector2i(-1, 0), new Vector2i(-1, -1), new Vector2i(0, -1), new Vector2i(1, -1),
    };
    static private int directionCount;
    static private GameObject self;

    static private void StepTo(Node node)
    {
		Node newNode = null;

        int dirStart;
        int dirEnd;
        if (node.directionIndex == -1)
        {
            dirStart = 0;
            dirEnd = 8;
        }
        else if (node.directionIndex % 2 == 0)
        {
            dirStart = ((node.directionIndex - 1) + 8) % 8;
            dirEnd = dirStart + 3;
        }
        else
        {
            dirStart = ((node.directionIndex - 2) + 8) % 8;
            dirEnd = dirStart + 5;
        }

        for (int i = dirStart; i < dirEnd; ++i)
        {
            int dir = i % 8;
            Vector2i pos = node.pos + directions[dir];
            bool passable = Tilemap.PassableTile(pos, 2, ignore: self);

            if (passable)
            {
                if (newNode == null)
                    newNode = Node.Get();
                newNode.pos = pos;
                
                bool closed = closeNodes.Contains(newNode);
                if (!closed)
                { 
					newNode.parent = node;
                    newNode.gScore = node.gScore + 1;
                    newNode.hScore = Vector2i.manhattanDistance(target, newNode.pos);
                    newNode.score = newNode.gScore + newNode.hScore;
                    newNode.directionIndex = dir;
                    openNodes.Add(newNode);
                    closeNodes.Add(newNode);
                    newNode = null;
				}
            }
        }

        if (newNode != null)
			newNode.Recycle();
    }

    static private void Collapse(Node node)
    {
        while (node.parent != null && node.parent.parent != null)
        {
            if (Tilemap.Raycast(node.pos, node.parent.parent.pos, ignore: self))
            {
                break;
            }

            node.parent = node.parent.parent;
        }
    }

	static private void TraverseBack(Node node)
    {
        UnityEngine.Profiling.Profiler.BeginSample("TraverseBack");
        while (node.parent != null)
        {
            Collapse(node);
            Step step = new Step();
            step.direction = node.pos - node.parent.pos;
            step.pos = node.pos;
            path.Insert(0, step);
            node = node.parent;
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

	static public List<Step> BuildPath(Vector2 from_, Vector2 target_, int directionCount = 8, float minRange = 0.1f, GameObject self = null, int depth = 100)
    {
        UnityEngine.Profiling.Profiler.BeginSample("BuildPath");
        Vector2i from = (Vector2i)Iso.Snap(from_);
        target = (Vector2i)Iso.Snap(target_);
        path.Clear();
        if (from == target)
        {
            UnityEngine.Profiling.Profiler.EndSample();
            return path;
        }
        openNodes.Clear();
        Node.Recycle(closeNodes);

        Pathing.directionCount = directionCount;
        Pathing.self = self;
        Node startNode = Node.Get();
		startNode.parent = null;
		startNode.pos = from;
		startNode.gScore = 0;
        startNode.hScore = Vector2i.manhattanDistance(from, target);
        startNode.score = int.MaxValue;
        startNode.directionIndex = -1;
        openNodes.Add(startNode);
        closeNodes.Add(startNode);
        int iterCount = 0;
        Node bestNode = startNode;
        while (openNodes.Count > 0)
        {
            Node node = openNodes.Take();

            if (node.hScore < bestNode.hScore)
                bestNode = node;
            if (node.hScore <= minRange)
            {
                TraverseBack(node);
				break;
			}
            StepTo(node);
            iterCount += 1;
			if (iterCount > depth)
            {
                TraverseBack(bestNode);
                break;
			}
		}
        //foreach (Node node in closeNodes)
        //{
        //    Iso.DebugDrawTile(node.pos, Color.magenta, 0.3f);
        //}
        //foreach (Node node in openNodes)
        //{
        //    Iso.DebugDrawTile(node.pos, Color.green, 0.3f);
        //}
        UnityEngine.Profiling.Profiler.EndSample();
        return path;
	}

	static public void DebugDrawPath(Vector2 from, List<Step> path) {
        if (path.Count > 0)
        {
            Debug.DrawLine(Iso.MapToWorld(from), Iso.MapToWorld(path[0].pos), Color.grey);
        }
		for (int i = 0; i < path.Count - 1; ++i)
        {
			Debug.DrawLine(Iso.MapToWorld(path[i].pos), Iso.MapToWorld(path[i + 1].pos));
		}
        if (path.Count > 0)
        {
            var center = Iso.MapToWorld(path[path.Count - 1].pos);
            Debug.DrawLine(center + Iso.MapToWorld(new Vector2(0, 0.15f)), center + Iso.MapToWorld(new Vector2(0, -0.15f)));
            Debug.DrawLine(center + Iso.MapToWorld(new Vector2(-0.15f, 0)), center + Iso.MapToWorld(new Vector2(0.15f, 0)));
        }
	}
}
