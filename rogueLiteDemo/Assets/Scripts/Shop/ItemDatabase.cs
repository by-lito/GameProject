using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;

    [Header("Todos los objetos del juego")]
    // Aquí es donde arrastramos los objetos en el Inspector de Unity
    public List<Item> allItems;

    void Awake()
    {
        // Esto sirve para que el script sea fácil de encontrar desde otros sitios
        if (instance == null) instance = this;
    }

    // Función para buscar un objeto por su nombre y devolverlo
    public Item GetItemByName(string name)
    {
        return allItems.Find(item => item.itemName == name);
    }
}