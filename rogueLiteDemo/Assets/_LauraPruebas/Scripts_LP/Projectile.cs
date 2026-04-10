using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 10;
    public float lifeTime = 2f; // Se destruye solo a los 2 segundos para no llenar la memoria

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Mueve la bala hacia la derecha (ajustar si queremos que vaya en otra dirección)
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si choca con un enemigo
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<EnemyHealth>(out EnemyHealth enemy))
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject); // La bala desaparece al chocar
        }
    }
}