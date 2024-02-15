using UnityEngine;
using System.Collections.Generic;

public class QuadNode
{
    public Rect bounds;
    public QuadNode[] children;
    public Vector2? playerPosition;
    private int depth;
    private int maxDepth;

    public QuadNode(Rect bounds, int depth, int maxDepth)
    {
        this.bounds = bounds;
        this.depth = depth;
        this.maxDepth = maxDepth;
    }

    public void InsertPlayer(Vector2 position, float subdivideThreshold)
    {
        if (depth < maxDepth)
        {
            float distToPlayer = Vector3.Distance(bounds.center, position);
            if (distToPlayer / bounds.size.magnitude < subdivideThreshold)
            {
                Subdivide();
            }
     
            if (children != null)
            {
                foreach (var child in children)
                {
                    child.InsertPlayer(position, subdivideThreshold);
                }
            }
            else
            {
                playerPosition = position;
            }
        }
        else
        {
            playerPosition = position;
        }
    }

    public void Subdivide()
    {
        children = new QuadNode[4];
        float halfWidth = bounds.width / 2f;
        float halfHeight = bounds.height / 2f;

        children[0] = new QuadNode(new Rect(bounds.x, bounds.y, halfWidth, halfHeight), depth + 1, maxDepth);
        children[1] = new QuadNode(new Rect(bounds.x + halfWidth, bounds.y, halfWidth, halfHeight), depth + 1, maxDepth);
        children[2] = new QuadNode(new Rect(bounds.x, bounds.y + halfHeight, halfWidth, halfHeight), depth + 1, maxDepth);
        children[3] = new QuadNode(new Rect(bounds.x + halfWidth, bounds.y + halfHeight, halfWidth, halfHeight), depth + 1, maxDepth);
    }

    public void RemovePlayer()
    {
        playerPosition = null;
        if (children != null)
        {
            foreach (var child in children)
            {
                child.RemovePlayer();
            }

            // Check if all children are empty
            if (AllChildrenEmpty())
            {
                children = null; // Remove child nodes
            }
        }
    }

    private bool AllChildrenEmpty()
    {
        foreach (var child in children)
        {
            if (child.playerPosition != null || child.children != null)
            {
                return false;
            }
        }

        return true;
    }


    public QuadNode FindOrExpand(Vector2 position, QuadTree tree)
    {
        return bounds.Contains(position) ? this : null;
    }


    public void Query(Vector2 position, float radius, List<Rect> result)
    {
        if (!bounds.Overlaps(new Rect(position.x - radius, position.y - radius, radius * 2, radius * 2)))
            return;

        if (children != null)
        {
            foreach (var child in children)
            {
                child.Query(position, radius, result);
            }
        }
        else if (playerPosition != null)
        {
            result.Add(bounds);
        }
    }
}