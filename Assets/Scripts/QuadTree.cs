using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    public QuadNode Root => root;

    private QuadNode root;
    public float CellSize { get; private set; }
    public int MaxDepth { get; private set; }

    public delegate void PlayerLocationUpdated(Vector2 position);

    public event PlayerLocationUpdated OnPlayerLocationUpdated;

    public QuadTree(float cellSize, int maxDepth)
    {
        this.CellSize = cellSize;
        this.MaxDepth = maxDepth;
        root = new QuadNode(new Rect(-cellSize / 2, -cellSize / 2, cellSize, cellSize), 0, maxDepth);
    }

    public void UpdatePlayerLocation(Vector2 newPlayerPosition)
    {
        root.RemovePlayer(); // Remove player from current position

        // Recenter the root if the new position is outside its bounds
        while (!root.bounds.Contains(newPlayerPosition))
        {
            float newWidth = root.bounds.width;
            float newHeight = root.bounds.height;

            // Calculate the new X and Y based on the direction the player has moved relative to the current root bounds
            float newX = root.bounds.xMin;
            if (newPlayerPosition.x < root.bounds.xMin)
                newX -= newWidth;
            else if (newPlayerPosition.x > root.bounds.xMax)
                newX += newWidth;

            float newY = root.bounds.yMin;
            if (newPlayerPosition.y < root.bounds.yMin)
                newY -= newHeight;
            else if (newPlayerPosition.y > root.bounds.yMax)
                newY += newHeight;

            QuadNode newRoot = new QuadNode(new Rect(newX, newY, newWidth, newHeight), 0, MaxDepth);
            newRoot.Subdivide();

            int index = (newPlayerPosition.x < root.bounds.center.x ? 0 : 1) + (newPlayerPosition.y < root.bounds.center.y ? 0 : 2);
            newRoot.children[index] = root;

            root = newRoot; // Update root reference
        }

        root.InsertPlayer(newPlayerPosition); // Insert player into new position
        OnPlayerLocationUpdated?.Invoke(newPlayerPosition);
    }



    public List<Rect> QueryNearbyCells(Vector2 position, float radius)
    {
        List<Rect> result = new List<Rect>();
        root.Query(position, radius, result);
        return result;
    }
}