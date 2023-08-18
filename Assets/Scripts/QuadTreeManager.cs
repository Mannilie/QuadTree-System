using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class QuadTreeManager : MonoBehaviour
{
    public Transform playerTransform;
    public float queryRadius = 10f;

    private QuadTree quadTree = new QuadTree(200f, 8);

    void Start()
    {
        quadTree.OnPlayerLocationUpdated += PlayerLocationUpdatedHandler;
    }

    void Update()
    {
        Vector2 playerPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
        quadTree.UpdatePlayerLocation(playerPosition);
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

        Gizmos.DrawWireCube(node.bounds.center, node.bounds.size);

        if (node.children != null)
        {
            foreach (var child in node.children)
            {
                DrawQuadNode(child);
            }
        }
    }

    private void DrawQueryArea()
    {
        Vector2 playerPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
        List<Rect> nearbyCells = quadTree.QueryNearbyCells(playerPosition, queryRadius);
        foreach (var cell in nearbyCells)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(cell.center, cell.size);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerPosition, queryRadius);
    }

    private void PlayerLocationUpdatedHandler(Vector2 position)
    {
        // You can implement logic here that responds to the player's location update.
    }
}