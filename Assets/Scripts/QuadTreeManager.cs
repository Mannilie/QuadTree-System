using UnityEngine;
using System.Collections.Generic;

public class QuadTreeManager : MonoBehaviour
{
    public Transform playerTransform;
    public float cellSize = 200f;
    public int maxDepth = 4;
    public float queryRadius = 10f;
    public float subdivideThreshold = 2f;

    private QuadTree quadTree;

    void Start()
    {
        quadTree = new QuadTree(cellSize, maxDepth);
        quadTree.OnPlayerLocationUpdated += PlayerLocationUpdatedHandler;
    }

    void Update()
    {
        Vector2 playerPosition =
            new Vector2(playerTransform.position.x, playerTransform.position.z); // Using Z instead of Y
        quadTree.UpdatePlayerLocation(playerPosition, subdivideThreshold);
    }

    void OnDrawGizmos()
    {
        if (quadTree != null)
        {
            DrawQuadNode(quadTree.Root);
            DrawQueryArea();
        }
    }

    private void DrawQuadNode(QuadNode node)
    {
        if (node == null)
            return;

        Gizmos.DrawWireCube(new Vector3(node.bounds.center.x, 0f, node.bounds.center.y),
            new Vector3(node.bounds.size.x, 1f, node.bounds.size.y)); // Using X and Z coordinates

        if (node.children != null)
        {
            foreach (var child in node.children)
            {
                DrawQuadNode(child); // Recursively draw child nodes
            }
        }
    }


    private void DrawQueryArea()
    {
        Vector2 playerPosition =
            new Vector2(playerTransform.position.x, playerTransform.position.z); // Using Z instead of Y
        List<Rect> nearbyCells = quadTree.QueryNearbyCells(playerPosition, queryRadius);
        foreach (var cell in nearbyCells)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(cell.center.x, 0f, cell.center.y),
                new Vector3(cell.size.x, 1f, cell.size.y)); // Using X and Z coordinates
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(playerPosition.x, 0f, playerPosition.y),
            queryRadius); // Using X and Z coordinates
    }

    private void PlayerLocationUpdatedHandler(Vector2 position)
    {
        // You can implement logic here that responds to the player's location update.
    }
}