using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int columns = 6;
    public int rows = 5;

    [Header("Hệ thống Thắng/Thua")]
    public int totalBlocks = 0;    
    public int wrongClickCount = 0;

    private GridBlock[,] grid;

    public Vector3[,] cellPositions;

    void Awake()
    {
        Instance = this;
        grid = new GridBlock[columns, rows];
        cellPositions = new Vector3[columns, rows];
    }

    public void RegisterBlock(GridBlock block, int x, int y)
    {
        if (x >= 0 && x < columns && y >= 0 && y < rows)
        {
            grid[x, y] = block;
            cellPositions[x, y] = block.transform.position;

            if (!block.isStaticObstacle)
            {
                totalBlocks++;
            }
        }
    }

    public void RemoveBlock(int x, int y)
    {
        if (x >= 0 && x < columns && y >= 0 && y < rows)
        {
            grid[x, y] = null;
            UpdateHiddenBlocks();

            totalBlocks--;
            if (totalBlocks <= 0)
            {
                AudioManager.Instance.PlayGameWinSound(); 
                UIManager.Instance.ShowVictory();
            }
        }
    }

    public void RegisterWrongClick()
    {
        wrongClickCount++;

        if (QueueManager.Instance != null)
        {
            QueueManager.Instance.ResetCombo();
        }


        if (wrongClickCount >= 3)
        {
            AudioManager.Instance.PlayGameOverSound();
            UIManager.Instance.ShowGameOver();
        }
    }

    public void UpdateHiddenBlocks()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GridBlock block = grid[x, y];
                if (block != null && block.isHidden)
                {
                    if (GetPathToExit(x, y) != null)
                    {
                        block.RevealColor();
                    }
                }
            }
        }
    }

    public List<Vector2Int> GetPathToExit(int startX, int startY)
    {

        if (startY == rows - 1)
        {
            return new List<Vector2Int>();
        }

        bool[,] visited = new bool[columns, rows];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> parentMap = new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(new Vector2Int(startX, startY));
        visited[startX, startY] = true;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Vector2Int exitNode = new Vector2Int(-1, -1);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (Vector2Int dir in directions)
            {
                int nextX = current.x + dir.x;
                int nextY = current.y + dir.y;

                if (nextX >= 0 && nextX < columns && nextY >= 0 && nextY < rows)
                {
                    if (!visited[nextX, nextY] && grid[nextX, nextY] == null)
                    {
                        visited[nextX, nextY] = true;
                        parentMap[new Vector2Int(nextX, nextY)] = current; 
                        queue.Enqueue(new Vector2Int(nextX, nextY));


                        if (nextY == rows - 1)
                        {
                            exitNode = new Vector2Int(nextX, nextY);
                            queue.Clear(); 
                            break;
                        }
                    }
                }
            }
        }

        if (exitNode.x != -1)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            Vector2Int curr = exitNode;
            while (curr != new Vector2Int(startX, startY))
            {
                path.Add(curr);
                curr = parentMap[curr];
            }
            path.Reverse(); 
            return path;
        }

        return null; 
    }
}