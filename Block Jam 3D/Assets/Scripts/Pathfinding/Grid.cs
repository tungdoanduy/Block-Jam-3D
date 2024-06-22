using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Node[,] grid;
    int row, col;

    public bool CreateGrid(List<Column> groundSlots)
    {
        col = groundSlots.Count;
        row = groundSlots[0].slots.Count;
        grid = new Node[row, col];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Node node = new Node(i, j, groundSlots[j].slots[i]);
                grid[i, j] = node;
            }
        }
        return true;
    }
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        for (int i = -1; i <= 1; i += 2)
        {
            if (node.x + i >= 0 && node.x + i < row)
                neighbors.Add(grid[node.x + i, node.y]);
            if (node.y + i >= 0 && node.y + i < col)
                neighbors.Add(grid[node.x, node.y + i]);
        }
        return neighbors;
    }

    void OnDrawGizmos()
    {
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                if (n.slot == null)
                    continue;
                Gizmos.color = (n.walkable) ? Color.blue : Color.red;
                Gizmos.DrawCube(n.slot.transform.position, Vector3.one * 0.35f);
            }
        }
    }
}
