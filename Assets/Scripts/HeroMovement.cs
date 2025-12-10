using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] GridSpawner grid;

    List<Vector3> pathPoints;
    int currentIndex = 0;
    bool isDashing = false;

    public GridSpawner Grid { get => grid; set => grid = value; }

    void Start()
    {
        pathPoints = grid.borderPositions;
        transform.position = pathPoints[0];

        // Bắt đầu Coroutine di chuyển liên tục
        StartCoroutine(MoveAlongPath());
    }

    IEnumerator MoveAlongPath()
    {
        while (true)
        {
            if (isDashing)
            {
                // Nếu đang dash thì bỏ qua frame này, chờ dash xong
                yield return null;
                continue;
            }

            Vector3 target = pathPoints[currentIndex];

            // Xoay theo hướng di chuyển
            Vector3 direction = (target - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            // Di chuyển tới target
            while (!isDashing && Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = target;
            CheckForWorldItemAtPosition(target);
            currentIndex = (currentIndex + 1) % pathPoints.Count;
        }
    }

    void CheckForWorldItemAtPosition(Vector3 position)
    {
        foreach (WorldItem item in grid.worldItems)
        {
            if (Vector3.Distance(item.transform.position, position) < 0.1f)
            {
                item.ActivateItem();
            }
        }
    }
    public IEnumerator ApplySpeedBoost(int level)
    {
        float originalSpeed = moveSpeed;
        float boostAmount = 1f * level;
        moveSpeed += boostAmount;
        yield return new WaitForSeconds(5f);
        moveSpeed = originalSpeed;
    }
    public IEnumerator ApplyDash()
    {
        isDashing = true;

        int nextIndex = (currentIndex + 1) % pathPoints.Count;
        Vector3 target = pathPoints[nextIndex];

        // xoay hướng
        Vector3 direction = (target - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        float dashSpeed = moveSpeed * 5f;
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, dashSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        currentIndex = nextIndex;

        isDashing = false;
    }
}
