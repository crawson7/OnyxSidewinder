using UnityEngine;
using System.Collections;

public static class Utilities
{
    public static float AngleDirection2(Vector2 A, Vector2 B)
    {
		// Returns -1 when the target direction is left, +1 when it is right and 0 if the direction is straight ahead or behind.
        float dir = A.x * B.y + A.y * B.x;
        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    public static float AngleDirection3(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
		// Returns -1 when the target direction is left, +1 when it is right and 0 if the direction is straight ahead or behind.
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

	public static bool CrossesBorder(Circle c, float b, float buffer=0.1f)
	{
		if(c.Center.x < b)
		{
			return (b - c.Center.x) < c.Radius - buffer;
		}
		else
		{
			return (c.Center.x - b) < c.Radius - buffer;
		}
	}

	public static bool CirclesOverlap(Circle c1, Circle c2, float buffer = 0.1f)
	{
		float dist = Vector2.Distance(c1.Center, c2.Center);
		return dist + buffer < (c1.Radius + c2.Radius);
	}

	// Find the points where the two circles intersect.
	public static int GetCircleIntersections(Circle circleA, Circle circleB,
		out Vector2 intersection1, out Vector2 intersection2)
	{
		float cxA = circleA.Center.x;
		float cyA = circleA.Center.y;
		float radiusA = circleA.Radius;
		float cxB = circleB.Center.x;
		float cyB = circleB.Center.y;
		float radiusB = circleB.Radius;

		// Find the distance between the centers.
		float dx = cxA - cxB;
		float dy = cyA - cyB;
		float dist = Mathf.Sqrt(dx * dx + dy * dy);

		// See how many solutions there are.
		if (dist > radiusA + radiusB)
		{
			// No solutions, the circles are too far apart.
			intersection1 = new Vector2(float.NaN, float.NaN);
			intersection2 = new Vector2(float.NaN, float.NaN);
			return 0;
		}
		else if (dist < Mathf.Abs(radiusA - radiusB))
		{
			// No solutions, one circle contains the other.
			intersection1 = new Vector2(float.NaN, float.NaN);
			intersection2 = new Vector2(float.NaN, float.NaN);
			return 0;
		}
		else if ((dist == 0) && (radiusA == radiusB))
		{
			// No solutions, the circles coincide.
			intersection1 = new Vector2(float.NaN, float.NaN);
			intersection2 = new Vector2(float.NaN, float.NaN);
			return 0;
		}
		else
		{
			// Find a and h.
			float a = (radiusA * radiusA -
				radiusB * radiusB + dist * dist) / (2 * dist);
			float h = Mathf.Sqrt(radiusA * radiusA - a * a);

			// Find P2.
			float cx2 = cxA + a * (cxB - cxA) / dist;
			float cy2 = cyA + a * (cyB - cyA) / dist;

			// Get the points P3.
			intersection1 = new Vector2(
				(float)(cx2 + h * (cyB - cyA) / dist),
				(float)(cy2 - h * (cxB - cxA) / dist));
			intersection2 = new Vector2(
				(float)(cx2 - h * (cyB - cyA) / dist),
				(float)(cy2 + h * (cxB - cxA) / dist));

			// See if we have 1 or 2 solutions.
			if (dist == radiusA + radiusB) return 1;
			return 2;
		}
	}
    
	public static T ParseEnum<T>(string s, bool safe=true)
	{
		if (safe) {
			try {
				T val = (T)System.Enum.Parse (typeof(T), s, true);
				return val;
			} catch {
				return (T)GetDefaultEnum<T> ();
			}
		} else {
			T val = (T)System.Enum.Parse (typeof(T), s, true);
			return val;
		}
	}

	public static T GetRandomEnum<T>()
	{
		System.Array A = System.Enum.GetValues(typeof(T));
		T V = (T)A.GetValue(UnityEngine.Random.Range(0,A.Length));
		return V;
	}

	public static T GetNextEnum<T>(int prev)
	{
		System.Array A = System.Enum.GetValues(typeof(T));
		int prevIndex = (int)prev;
		int nextIndex = ((prevIndex+1)<A.Length)? prevIndex+1 : 0;
		T value = (T)A.GetValue(nextIndex);
		return value;
	}

	public static T GetDefaultEnum<T>()
	{
		T[] vals = (T[]) System.Enum.GetValues(typeof(T));
		return vals[0];
	}
}
