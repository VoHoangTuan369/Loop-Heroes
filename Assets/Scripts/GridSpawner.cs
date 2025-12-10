using UnityEngine;
using System.Collections.Generic;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] GameObject quadPrefab;
    [SerializeField] HeroMovement heroPrefab;
    [SerializeField] float spacing = 1f;

    public List<Vector3> borderPositions = new List<Vector3>();
    public List<WorldItem> worldItems = new List<WorldItem>();

    // HEXAGON 6 CẠNH — CẠNH = 2 — RỖNG
    int[,] hexMask = new int[,]
    {
        {0,1,1,0}, // Row 0 (bottom)
        {1,0,0,1}, // Row 1
        {1,0,0,1}, // Row 2
        {0,1,1,0}, // Row 3 (top)
    };

    void Start()
    {
        int size = hexMask.GetLength(0);
        Vector3[,] grid = new Vector3[size, size];

        float offset = (size - 1) / 2f * spacing;

        // Tạo ô hexagon dựa theo mask
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                if (hexMask[z, x] == 1)
                {
                    Vector3 pos = new Vector3(x * spacing - offset, 0, z * spacing - offset);
                    grid[x, z] = pos;
                    Instantiate(quadPrefab, pos, Quaternion.identity, transform);
                }
            }
        }

        // Lấy border theo chiều kim đồng hồ
        // TOP row (z = size - 1)
        for (int x = 0; x < size; x++)
            if (hexMask[size - 1, x] == 1)
                borderPositions.Add(grid[x, size - 1]);

        // RIGHT edge (x = size - 1)
        for (int z = size - 2; z >= 0; z--)
            if (hexMask[z, size - 1] == 1)
                borderPositions.Add(grid[size - 1, z]);

        // BOTTOM row (z = 0)
        for (int x = size - 2; x >= 0; x--)
            if (hexMask[0, x] == 1)
                borderPositions.Add(grid[x, 0]);

        // LEFT edge (x = 0)
        for (int z = 1; z < size - 1; z++)
            if (hexMask[z, 0] == 1)
                borderPositions.Add(grid[0, z]);

        // Spawn Hero tại border đầu tiên
        HeroMovement hero = Instantiate(heroPrefab, borderPositions[0], Quaternion.identity);
        hero.Grid = this;
        GameManager.Instance.HeroMovement = hero;
    }
}
