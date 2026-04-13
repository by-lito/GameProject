using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [Header("Datos del Objeto")]
    public string itemName;
    public int price;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Costes")]
    public int dustCost;       // Coste en la tienda del nivel
    public int lobbyCost;      // Coste en el lobby para desbloquearlo
    public bool isUnlocked;    // ¿Lo ha comprado ya en el lobby?

    // Método para desbloquear el objeto en el lobby
    public virtual void OnUnlock()
    {
        isUnlocked = true;
        Debug.Log(itemName + " ha sido desbloqueado para siempre.");
    }
    // Lo que hace el objeto al recogerlo en la RUN
    public abstract void OnEquip(GameObject player);// Este método se llama cuando el jugador lo compra en la tienda del nivel. Aquí es donde se aplica el efecto al jugador.

    // Detección de cuando el jugador se acerca al objeto
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Aquí podríamos mostrar un mensaje de "Pulsar E para comprar"
            Debug.Log("Cerca de: " + itemName + ". Precio: " + price);
        }
    }
}