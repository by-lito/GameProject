using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public enum ShopType { Lobby, Run }

    [Header("Configuración de Tienda")]
    public ShopType shopMode; // Seleccionable en el Inspector: Lobby o Run
    public Item itemToSell;   // El ScriptableObject del ítem
    public int price;         // Precio del artículo

    [Header("Interacción 3D")]
    public float interactionRange = 3f;

    void Update()
    {
        // Detectamos la interacción (puedes cambiar 'E' por la que prefieras)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPurchase();
        }
    }

    private void TryPurchase()
    {
        // Localizamos al jugador por su Tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Comprobación de distancia en el espacio 3D
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance > interactionRange) return;

        // Lógica de transacción según el modo de tienda
        if (shopMode == ShopType.Lobby)
        {
            HandleLobbyPurchase();
        }
        else
        {
            HandleRunPurchase();
        }
    }

    private void HandleLobbyPurchase()
    {
        // Comprobamos moneda persistente (monedas del nexo)
        if (PlayerWallet.instance.permanentMoney >= price)
        {
            PlayerWallet.instance.SpendPermanentMoney(price); // Gastamos el dinero persistente
            CompleteTransaction();
        }
        else
        {
            Debug.Log("<color=red>Tienda:</color> No tienes suficiente dinero persistente.");
        }
    }

    private void HandleRunPurchase()
    {
        // Comprobamos moneda de la run (Angel Dust)
        if (PlayerWallet.instance.CanAfford(price))
        {
            PlayerWallet.instance.SpendDust(price);
            CompleteTransaction();
        }
        else
        {
            Debug.Log("<color=red>Tienda:</color> No tienes suficiente Angel Dust.");
        }
    }

    private void CompleteTransaction()
    {
        // Añadimos al inventario y confirmamos
        PlayerInventory.instance.AddItem(itemToSell);
        Debug.Log($"<color=green>Compra Exitosa:</color> Has adquirido {itemToSell.itemName}");

        // Aquí podrías añadir un efecto visual o sonido
    }
}