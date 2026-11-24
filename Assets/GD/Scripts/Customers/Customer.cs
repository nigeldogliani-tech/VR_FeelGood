using UnityEngine;
using TMPro;

public class Customer : MonoBehaviour
{
    [Header("Customer Order")]
    public string orderID;
    public string requestedDrinkID;

    [Header("Order Bubble")]
    [SerializeField] private TMP_Text orderText;

    private CustomerManager customerManager;

    private int spawnPointIndex;

    // clients + demande
    public void Initialize( string drinkID,string newOrderID, CustomerManager manager,int pointIndex)
    {
        requestedDrinkID = drinkID;

        orderID = newOrderID;

        customerManager = manager;

        spawnPointIndex = pointIndex;

        UpdateOrderBubble();
    }

    // affiche commande
    private void UpdateOrderBubble()
    {
        if (orderText == null)
        {
            Debug.LogWarning("OrderTxt po assigné !");
            return;
        }

        switch (requestedDrinkID)
        {
            case "coffee":
                orderText.text = "Café";
                break;

            case "cappu":
                orderText.text = "Cappuccino";
                break;

            case "choco":
                orderText.text = "Chocolat chaud";
                break;

            case "tea":
                orderText.text = "Thé";
                break;

            default:
                orderText.text = "tié ki";
                break;
        }
    }

    // plu commande bye bye client
    public void CompleteOrder()
    {
        Debug.Log("Client servi : " + orderID);

        // retire client liste
        customerManager.RemoveCustomer(this);

        // libère sp
        customerManager.ReleaseSpawnPoint(spawnPointIndex);

        // client
        Destroy(gameObject);
    }
}
