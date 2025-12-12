using UnityEngine;
using System.Collections.Generic;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] GameObject quadPrefab;
    [SerializeField] HeroMovement heroPrefab;

    public List<Vector3> borderPositions = new List<Vector3>();
    public List<WorldItem> worldItems = new List<WorldItem>();

    // số ô muốn tạo
    int cellCount = 10;

    void Start()
    {
        // bán kính vòng tròn (tùy chỉnh để vừa đủ 16 ô)
        float radius = 1.8f;

        Vector3[] positions = new Vector3[cellCount];

        // Tính toán vị trí theo vòng tròn
        for (int i = 0; i < cellCount; i++)
        {
            float angle = i * Mathf.PI * 2f / cellCount; // chia đều 360 độ
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Vector3 pos = new Vector3(x, 0, z);
            positions[i] = pos;

            Instantiate(quadPrefab, pos, Quaternion.identity, transform);
        }

        // Lấy border theo chiều kim đồng hồ (chính là toàn bộ vòng tròn)
        borderPositions.AddRange(positions);

        // Spawn Hero tại border đầu tiên
        HeroMovement hero = Instantiate(heroPrefab, borderPositions[0], Quaternion.identity);
        hero.Grid = this;
        GameManager.Instance.HeroMovement = hero;
        GameManager.Instance.Hero = hero.GetComponent<Hero>();
    }
}
