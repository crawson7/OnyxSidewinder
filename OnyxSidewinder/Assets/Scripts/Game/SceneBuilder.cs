using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneBuilder
{
    private Bounds _bounds;
    private Vector3 _startPosition;

    // Planet generation
    private float _minGapDist;
    private float _maxGapDist;
    private float _minBodySize;
    private float _maxBodySize;
    private float _minGravityDepth;
    private float _maxGravityDepth;

    public List<PlanetData> LoadPlanets(Rect rect, Vector3 sp, float minBod, float maxBod, float minGrav, float maxGrav, float minGap, float maxGap)
    {
        // TODO: Fix alignment of the first planet.

        //List<PlanetController> planets = new List<PlanetController>();
        List<PlanetData> planets = new List<PlanetData>();

        // Set Bounds
        _startPosition = sp;
        _bounds = new Bounds();
        _bounds.center = rect.position;
        _bounds.extents = new Vector3(rect.width * 0.5f, rect.height * 0.5f, 0);
        _minGapDist = minGap;
        _maxGapDist = maxGap;
        _minBodySize = minBod;
        _maxBodySize = maxBod;
        _minGravityDepth = minGrav;
        _maxGravityDepth = maxGrav;

        // Place Planets
        PlanetData pc = PlacePlanet(0.2f, 3.0f, new Vector3(0, 4, 0), planets);
        if (pc == null) { return planets; }

        PlanetData next = GetNextBranch(planets);
        int count = 0;
        while (next != null && next.Pos.y < _bounds.max.y && count < 100)
        {
            Logger.Log("Branch " + count + " from Planet " + next.Type, 1);
            Branch(next, planets);
            next = GetNextBranch(planets);
            count++;
        }
        return planets;
    }

    private PlanetData GetNextBranch(List<PlanetData> planets)
    {
        for (int i = 0; i < planets.Count; i++)
        {
            if (!planets[i].full)
            {
                return planets[i];
            }
        }
        return null;
    }

    private PlanetData CalculateNewPlanetData()
    {
        float body = UnityEngine.Random.Range(_minBodySize, _maxBodySize);
        float gravity = UnityEngine.Random.Range(_minGravityDepth, _maxGravityDepth) + body;
        float gap = UnityEngine.Random.Range(_minGapDist, _maxGapDist) + gravity;
        return new PlanetData(gap, gravity, body);
    }


    private void Branch(PlanetData pc, List<PlanetData> planets)
    {
        PlanetData newPlanet = CalculateNewPlanetData();
        // float newPlanetRadius = UnityEngine.Random.Range(2.0f, 3.5f);
        // Logger.Log("New Planet Radius is " + newPlanetRadius);

        // Find What other planets are in range
        List<PlanetData> closePlanets = new List<PlanetData>();
        bool anyPlanetToLeft = false;
        for (int i = 0; i < planets.Count; i++)
        {
            if (planets[i] == pc) { continue; } // Planet is this planet.
            if (planets[i].Pos.y + planets[i].Gravity < pc.Pos.y - pc.Gravity) { continue; } // Planet is too far below this one.

            if (planets[i].Pos.x < pc.Pos.x)
            {
                // Planet is to the left, include it.
                closePlanets.Add(planets[i]);
                anyPlanetToLeft = true;
            }
            else if (planets[i].Pos.y > pc.Pos.y)
            {
                //Planet is right and above
                closePlanets.Add(planets[i]);
                anyPlanetToLeft = true;
            }
        }

        // Determine which Close planet to use for placement.
        PlanetData closest = null;

        for (int i = 0; i < closePlanets.Count; i++)
        {
            PlanetData temp = closePlanets[i];
            if (temp.Pos.x > pc.Pos.x && temp.Pos.y <= pc.Pos.y) { continue; }

            if (temp.Pos.x < pc.Pos.x)
            {
                // Left of source, Pick largest y
                if (closest == null || temp.Pos.y > closest.Pos.y)
                {
                    closest = temp;
                }
            }
            else
            {
                //Right of source, pick largest x
                if (closest == null || temp.Pos.x > closest.Pos.x)
                {
                    closest = temp;
                }
            }
        }

        if (closest != null && anyPlanetToLeft)
        {
            // Calculate desired position of new planet branch.
            float ON_Length = pc.Gravity + newPlanet.Gap;
            float BN_Length = closest.Gravity + newPlanet.Gap;
            Circle A = new Circle(pc.Pos, ON_Length);
            Circle B = new Circle(closest.Pos, BN_Length);
            Vector2 intersect1, intersect2;
            int intersections = Utilities.GetCircleIntersections(A, B, out intersect1, out intersect2);
            Vector3 newPosition = Vector3.zero;
            if (intersections == 0 || intersections == 1)
            {
                Logger.Log("Error Placing Planet. Planet " + pc.Type + " is Full.", 3);
                pc.full = true;
                return;
            }
            else
            {
                // Need to make sure its getting the intersection to the right.
                //Vector3 baseLine = pc.gameObject.transform.InverseTransformPoint(closest.Pos);

                //Vector3 angle1 = pc.gameObject.transform.InverseTransformPoint(intersect1);
                //Vector3 angle2 = pc.gameObject.transform.InverseTransformPoint(intersect2);
                Vector3 baseLine = RelativePoint(pc.Pos, closest.Pos);

                Vector3 angle1 = RelativePoint(pc.Pos, intersect1);
                Vector3 angle2 = RelativePoint(pc.Pos, intersect2);

                float angle1Direction = Utilities.AngleDirection3(baseLine, angle1, -Vector3.forward);
                float angle2Direction = Utilities.AngleDirection3(baseLine, angle2, -Vector3.forward);
                Logger.Log("BaseLineVector " + baseLine + "angle1: " + angle1 + "angle2: " + angle2 + "angle1Dir: " + angle1Direction + "angle2Dir: " + angle2Direction, 1);
                if (angle1Direction > 0)
                {
                    Logger.Log("Intersect 1 is to the right.", 1);
                    newPosition = intersect1;
                }
                else if (angle2Direction > 0)
                {
                    Logger.Log("Intersect 2 is to the right.", 1);
                    newPosition = intersect2;
                }
                else
                {
                    Logger.Log("Error Placing Planet. No Valid Angle Direction. Planet " + pc.Type + " is Full.", 3);
                    pc.full = true;
                    return;
                }

            }

            Circle newPlanetCircle = new Circle(newPosition, newPlanet.Gap);
            Logger.Log("Closest Branch Planet is: " + closest.Type, 1);
            Logger.Log("Attempting to branch new planet at " + newPosition, 1);


            bool canPlace = CanPlace(newPlanetCircle, planets);
            if (canPlace)
            {
                // Place new planet relative to Closest.
                Logger.Log("New planet placement Successful.", 1);
                PlacePlanet(newPlanet.Body, newPlanet.Gravity, newPosition, planets);
                return;
            }
            else
            {
                if (!CreatePlanetOnLeftEdge(pc, newPlanet, planets))
                {
                    Logger.Log("There is no space against the wall. Planet " + pc.Type + " is full.", 1);
                    pc.full = true;
                }
            }
        }
        else // No planets are in range
        {
            Logger.Log("There are no valid Branching Planets from Planet " + pc.Type, 1);
            if (!CreatePlanetOnLeftEdge(pc, newPlanet, planets))
            {
                Logger.Log("There is no space against the wall. Planet " + pc.Type + " is full.", 1);
                pc.full = true;
            }
        }
    }

    private Vector3 RelativePoint(Vector3 source, Vector3 target)
    {
        return source - target;
    }

    private bool CreatePlanetOnLeftEdge(PlanetData pc, PlanetData newPlanet, List<PlanetData> planets)
    {
        float spaceBetweenPlanetAndBounds = pc.Pos.x - pc.Gravity - _bounds.min.x;
        Vector3 newPos;
        if (spaceBetweenPlanetAndBounds >= newPlanet.Gap * 2)
        {
            //Try Placing new planet directly to the left.
            newPos = pc.Pos - new Vector2(pc.Gravity + newPlanet.Gap, 0);
            bool canPlaceToLeft = CanPlace(new Circle(newPos, newPlanet.Gap), planets);
            if (canPlaceToLeft)
            {
                Logger.Log("Placing ne planet directly to the left of planet " + pc.Type, 1);
                PlacePlanet(newPlanet.Body, newPlanet.Gravity, newPos, planets);
            }
            else
            {
                return false;
            }
        }
        else
        {
            // Place Against the Left wall above this planet
            float newXpos = _bounds.min.x + newPlanet.Gap;
            float x = pc.Pos.x - newXpos;
            float h = pc.Gravity + newPlanet.Gap;
            float newYpos = (Mathf.Sqrt((h * h) - (x * x)) + pc.Pos.y);
            newPos = new Vector2(newXpos, newYpos);
            bool canPlace = CanPlace(new Circle(newPos, newPlanet.Gap), planets);
            if (canPlace)
            {
                Logger.Log("Placing new planet against the left border at " + newXpos + ", " + newYpos, 1);
                PlacePlanet(newPlanet.Body, newPlanet.Gravity, new Vector3(newXpos, newYpos, 0), planets);
            }
            else
            {
                Logger.Log("There is no space against the wall. Planet " + pc.Type + " is full.", 1);
                return false;
            }
        }
        return true;
    }

    private void CreateRefBall(Vector3 pos, string name, float rad = .1f)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(rad * 2.0f, rad * 2.0f, rad * 2.0f);
        go.name = name;
    }

    private bool CanPlace(Circle newPlanetCircle, List<PlanetData> planets)
    {
        bool canPlace = true;
        // checks if this circle can be placed on the board without overlapping any borders or any other planets.
        if (Utilities.CrossesBorder(newPlanetCircle, _bounds.max.x))
        {
            Logger.Log("New planet overlaps Right wall.", 1);
            canPlace = false;
        }
        if (Utilities.CrossesBorder(newPlanetCircle, _bounds.min.x))
        {
            Logger.Log("New planet overlaps Left wall.", 1);
            canPlace = false;
        }
        // If it overlaps any other circle, its not valid.
        for (int i = 0; i < planets.Count; i++)
        {
            Circle existingPlanet = new Circle(planets[i].Pos, planets[i].Gravity);
            if (Utilities.CirclesOverlap(newPlanetCircle, existingPlanet))
            {
                Logger.Log("New planet overlaps the existing planet: " + planets[i].Type, 1);
                canPlace = false;
                break;
            }
        }
        return canPlace;
    }

    private PlanetData PlacePlanet(float body, float gravity, Vector3 pos, List<PlanetData> planets)
    {
        PlanetType type = GetPlanetType(body);

        GameObject prefab = Resources.Load("Game/Planet") as GameObject;
        //return null;
        
        GameObject planet5 = GameObject.Instantiate<GameObject>(prefab);
        if (planet5 == null) { return null; }
        PlanetController pc5 = planet5.GetComponent<PlanetController>();
        pc5.Initialize(body, gravity, type);
        //planets.Add(pc5);
        PlanetData pd = new PlanetData(PlanetType.Bouncer, pos, 0, gravity, body);
        planets.Add(pd);
        pc5 = planet5.GetComponent<PlanetController>();
        pc5.gameObject.transform.position = pos;
        pc5.gameObject.transform.SetParent(SceneManager.Instance.GameParent.transform, false);
        pc5.ID = planets.Count;
        return pd;
        
    }

    private PlanetType GetPlanetType(float bodyRadius)
    {
        if (bodyRadius > 1.25)
        {
            return PlanetType.Chomper;
        }
        else if (bodyRadius > 0.75)
        {
            return PlanetType.Bouncer;
        }
        else
        {
            return PlanetType.Popper;
        }
    }
}
