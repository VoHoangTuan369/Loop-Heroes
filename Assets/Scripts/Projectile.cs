using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float damage = 10f;
    [SerializeField] float lifeTime = 3f; // thời gian tồn tại

    private Vector3 moveDirection;   // hướng bay cố định
    private HashSet<Enemy> hitEnemies = new HashSet<Enemy>();
    TrailRenderer trail;

    private void Awake()
    {
        trail = GetComponentInChildren<TrailRenderer>();
    }
    public void Init(Vector3 direction)
    {
        moveDirection = direction.normalized;
        hitEnemies.Clear(); // reset danh sách enemy đã trúng
        StopAllCoroutines(); // dừng coroutine cũ nếu có
        StartCoroutine(MoveStraight());
        StartCoroutine(ReturnToPoolAfterTime()); // coroutine tự trả về pool

        // reset trail mỗi lần bắn
        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }
    }

    IEnumerator MoveStraight()
    {
        while (gameObject.activeInHierarchy)
        {
            transform.position += moveDirection * speed * Time.deltaTime;

            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }

            yield return null;
        }
    }

    IEnumerator ReturnToPoolAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        PoolManager.Instance.ReturnProjectile(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !hitEnemies.Contains(enemy))
        {
            Debug.Log("Projectile hit enemy!");
            enemy.TakeDamage(damage);
            hitEnemies.Add(enemy);
        }
    }
}
