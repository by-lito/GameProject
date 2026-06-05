using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Valor")]
    [Tooltip("Angel Dust que da al recogerla.")]
    [SerializeField] private int amount = 10;

    [Header("Recogida")]
    [Tooltip("Distancia a la que el jugador la recoge.")]
    [SerializeField] private float pickupRadius = 1.2f;

    [Header("Visual (opcional)")]
    [Tooltip("Velocidad de giro. 0 = quieta.")]
    [SerializeField] private float spinSpeed = 90f;

    private Transform player;
    private bool collected = false;

    private void Update()
    {
        if (spinSpeed != 0f)
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

        if (collected) return;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p == null) return;
            player = p.transform;
        }

        Vector3 a = transform.position; a.y = 0f;
        Vector3 b = player.position;    b.y = 0f;

        if (Vector3.Distance(a, b) <= pickupRadius)
            Collect();
    }

    private void Collect()
    {
        collected = true;

        if (PlayerWallet.instance != null)
            PlayerWallet.instance.AddAngelDust(amount);
        else
            Debug.LogWarning("[CoinPickup] No hay PlayerWallet (arranca desde Boot).", this);

        Destroy(gameObject);
    }
}