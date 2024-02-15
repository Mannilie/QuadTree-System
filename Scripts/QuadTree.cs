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

    public void UpdatePlayerLocation(Vector2 newPlayerPosition, float subdivideThreshold)
    {
        root.RemovePlayer(); // Remove player from current position

        // Find the difference between the new position and the root's center
        float diffX = newPlayerPosition.x - root.bounds.center.x;
        float diffY = newPlayerPosition.y - root.bounds.center.y;

        // Calculate the offset based on the cell size
        float offsetX = Mathf.Round(diffX / CellSize) * CellSize;
        float offsetY = Mathf.Round(diffY / CellSize) * CellSize;

        // Calculate the new X and Y for the root
        float newX = root.bounds.xMin + offsetX;
        float newY = root.bounds.yMin + offsetY;

        // Only update the root if the new position is outside its current bounds
        if (!root.bounds.Contains(newPlayerPosition))
        {
            QuadNode newRoot = new QuadNode(new Rect(newX, newY, root.bounds.width, root.bounds.height), 0, MaxDepth);
            newRoot.Subdivide();

            int index = (newPlayerPosition.x < root.bounds.center.x ? 0 : 1) + (newPlayerPosition.y < root.bounds.center.y ? 0 : 2);
            newRoot.children[index] = root;

            root = newRoot; // Update root reference
        }

        root.InsertPlayer(newPlayerPosition, subdivideThreshold); // Insert player into new position
        OnPlayerLocationUpdated?.Invoke(newPlayerPosition);
    }




    public List<Rect> QueryNearbyCells(Vector2 position, float radius)
    {
        List<Rect> result = new List<Rect>();
        root.Query(position, radius, result);
        return result;
    }
}