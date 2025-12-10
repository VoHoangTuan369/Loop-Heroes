using UnityEngine;
using System.Collections.Generic;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] GameObject quadPrefab;
    [SerializeField] HeroMovement heroPrefab;
    [SerializeField] int width = 3;
    [SerializeField] int height = 3;
    [SerializeField] float spacing = 1f;

    public List<Vector3> borderPositions = new List<Vector3>();
    public List<WorldItem> worldItems = new List<WorldItem>();

    void Start()
    {
        float offsetX = (width - 1) / 2f * spacing;
        float offsetZ = (height - 1) / 2f * spacing;

        // Tạo tất cả ô
        Vector3[,] grid = new Vector3[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (x == width / 2 && z == height / 2)
                    continue;

                Vector3 position = new Vector3(x * spacing - offsetX, 0, z * spacing - offsetZ);
                grid[x, z] = position;

                Instantiate(quadPrefab, position, Quaternion.identity, transform);
            }
        }

        // Thêm theo thứ tự viền (chu vi, chiều kim đồng hồ)
        // Top row (trái -> phải)
        for (int x = 0; x < width; x++)
            borderPositions.Add(grid[x, height - 1]);

        // Right column (trên -> dưới, bỏ góc đã thêm)
        for (int z = height - 2; z >= 0; z--)
            borderPositions.Add(grid[width - 1, z]);

        // Bottom row (phải -> trái, bỏ góc đã thêm)
        for (int x = width - 2; x >= 0; x--)
            borderPositions.Add(grid[x, 0]);

        // Left column (dưới -> trên, bỏ góc đã thêm)
        for (int z = 1; z < height - 1; z++)
            borderPositions.Add(grid[0, z]);

        HeroMovement hero = Instantiate(heroPrefab, borderPositions[0], Quaternion.identity);
        hero.Grid = this;
        GameManager.Instance.HeroMovement = hero;
    }
}
