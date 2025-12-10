using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float damage;
    private Vector3 moveDirection;   // hướng bay cố định

    // lưu các enemy đã trúng
    private HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

    public void Init(Vector3 direction)
    {
        moveDirection = direction.normalized;
        StartCoroutine(MoveStraight());
        Destroy(gameObject, 3f); // tự hủy sau 3 giây
    }

    IEnumerator MoveStraight()
    {
        while (true)
        {
            transform.position += moveDirection * speed * Time.deltaTime;

            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }

            yield return null;
        }
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
