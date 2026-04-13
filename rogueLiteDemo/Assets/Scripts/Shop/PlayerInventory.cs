using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Esta es la lista de objetos actuales en la RUN
    public List<Item> currentItems = new List<Item>();
    public static PlayerInventory instance;//El 'static' hace que esta variable pertenezca a la clase y no a un objeto concreto.
                                           // Permite que otros scripts accedan a 'PlayerInventory.instance' desde cualquier lugar.

    private void Awake()
    {
        if (instance == null)// si no hay instancia asignada se asigna como única
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); //Esto evita tener dos inventarios funcionando a la vez.
        }
    }
    // Método para añadir un objeto al inventario del jugador
    public void AddItem(Item newItem)
    {
        currentItems.Add(newItem);// Agrega el nuevo objeto a la lista de objetos actuales
        Debug.Log("Objeto guardado en el inventario: " + newItem.itemName);

        // Aquí es donde llamaríamos al efecto del objeto
        newItem.OnEquip(gameObject);// Llama al método OnEquip del objeto, pasando el jugador como argumento
    }
}