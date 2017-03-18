using System;
using UnityEngine;

[Serializable]
public struct Vector2i
{ 
	public int x;
	public int y;
	
// Indexer
	public int this [int index]
	{
		get
		{ 
			switch (index)
			{
			case X_INDEX:
				return this.x;
			case Y_INDEX:
				return this.y;
			default:
				throw new IndexOutOfRangeException ("Invalid Vector2i index!");
			}
		}
		set
		{
			switch (index)
			{
			case X_INDEX:
				this.x = value;
				break;
			case Y_INDEX:
				this.y = value;
				break;
			default:
				throw new IndexOutOfRangeException ("Invalid Vector2i index!");
			}
		}
	}

// Constructors
	public Vector2i (int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	
// Properties
	public float sqrMagnitude
	{
		get { return (float)x * x + (float)y * y; }
	}
		
	public float magnitude
	{
		get { return Mathf.Sqrt (sqrMagnitude); }
	}
		
	public bool IsWithinBounds (Vector2i from, Vector2i to)
	{
		return this.x >= from.x && this.x < to.x &&
				this.y >= from.y && this.y < to.y;
	}
		
// Set
	public void Set (int new_x, int new_y)
	{
		this.x = new_x;
		this.y = new_y;
	}
	
// Scaling
	public void Scale (Vector2i scale)
	{
		x *= scale.x;
		y *= scale.y;
	}
	
	public static Vector2i Scale (Vector2i a, Vector2i b)
	{
		return new Vector2i (
			a.x * b.x,
			a.y * b.y
		);
	}
    
// Rotations
    public void RotateCW()
    {
        int old_x = x;
        x = y;
        y = -old_x;
    }

    public void RotateCCW()
    {
        int old_x = x;
        x = -y;
        y = old_x;
    }

    public static Vector2i RotateCW(Vector2i a)
    {
        return new Vector2i(a.y, -a.x);
    }

    public static Vector2i RotateCCW(Vector2i a)
    {
        return new Vector2i(-a.y, a.x);
    }
    
	public override string ToString ()
	{
		return string.Format ("({0}, {1})", x ,y);
	}

// Operators
	public static Vector2i operator + (Vector2i a, Vector2i b)
	{
		return new Vector2i (
			a.x + b.x,
			a.y + b.y
		); 
	}

	public static Vector2i operator - (Vector2i a) {
		return new Vector2i (
			-a.x,
			-a.y
		);
	}

	public static Vector2i operator - (Vector2i a, Vector2i b)
	{
		return a + (-b);
	}

	public static Vector2i operator * (int d, Vector2i a)
	{
		return new Vector2i(
			d * a.x,
			d * a.y
		);
	}

	public static Vector2i operator * (Vector2i a, int d)
	{
		return d * a;
	}

	public static Vector2i operator / (Vector2i a, int d)
	{
		return new Vector2i(
			a.x / d,
			a.y / d
		);
	}
		
// Equality
	public static bool operator == (Vector2i lhs, Vector2i rhs)
	{
		return lhs.x == rhs.x && lhs.y == rhs.y;
	}

	public static bool operator != (Vector2i lhs, Vector2i rhs)
	{
		return !(lhs == rhs);
	}

	public override bool Equals (object other)
	{
		if (!(other is Vector2i))
		{
			return false;
		}
		return this == (Vector2i)other;
	}
		
	public bool Equals (Vector2i other)
	{
		return this == other;
	}
		
	public override int GetHashCode ()
	{
		return (x.GetHashCode() << 6) ^ y.GetHashCode();
	}

// Static methods
	
	public static float Distance (Vector2i a, Vector2i b)
	{
		return (a - b).magnitude;
	}

    public static int manhattanDistance(Vector2i a, Vector2i b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

    public static Vector2i Min(Vector2i lhs, Vector2i rhs)
	{
		return new Vector2i (
			Mathf.Min (lhs.x, rhs.x),
			Mathf.Min (lhs.y, rhs.y)
		);
	}

	public static Vector2i Max(Vector2i a, Vector2i b)
	{
		return new Vector2i (
			Mathf.Max (a.x, b.x),
			Mathf.Max (a.y, b.y)
		);
	}

	public static int Dot (Vector2i lhs, Vector2i rhs)
	{
		return lhs.x * rhs.x +
				lhs.y * rhs.y;
	}
	
	public static float Magnitude(Vector2i a)
	{
		return a.magnitude;
	}

	public static float SqrMagnitude(Vector2i a)
	{
		return a.sqrMagnitude;
	}

// Default values
	public static Vector2i down
	{
		get { return new Vector2i (0,-1); }
	}

	public static Vector2i up
	{
		get { return new Vector2i (0,+1); }
	}

	public static Vector2i left
	{
		get { return new Vector2i (-1,0); }
	}

	public static Vector2i right 
	{
		get { return new Vector2i (+1,0); }
	}

	public static Vector2i one
	{
		get { return new Vector2i (+1,+1); }
	}

	public static Vector2i zero
	{
		get { return new Vector2i (0,0); }
	}
	
// Conversions
	public static explicit operator Vector2i (Vector2 source)
	{
		return new Vector2i ((int)source.x, (int)source.y);
	}

	public static implicit operator Vector2 (Vector2i source)
	{
		return new Vector2 (source.x, source.y);
	}

	public static explicit operator Vector2i (Vector3 source)
	{
		return new Vector2i ((int)source.x, (int)source.y);
	}

	public static implicit operator Vector3 (Vector2i source)
	{
		return new Vector3 (source.x, source.y, 0);
	}
	
// Constants
	public const int X_INDEX = 0;
	public const int Y_INDEX = 1;
}
