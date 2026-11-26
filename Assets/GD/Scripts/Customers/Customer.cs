using UnityEngine;

public class Customer : MonoBehaviour
{
    [Header("Customer Order")]
    public string orderID;
    public string requestedDrinkID;

    private CustomerManager customerManager;
    private int spawnPointIndex;
    public void Initialize( string drinkID,string newOrderID, CustomerManager manager,int pointIndex)// clients + demande
    {
        requestedDrinkID = drinkID;
        orderID = newOrderID;
        customerManager = manager;
        spawnPointIndex = pointIndex;
    }
    public void CompleteOrder()// plu commande bye bye client
    {
        Debug.Log("Client servi : " + orderID);
        if(customerManager != null)
        {
            customerManager.RemoveCustomer(this);// retire client liste
            customerManager.ReleaseSpawnPoint(spawnPointIndex);// libère sp
        }
        // client
        Destroy(gameObject);
    }
}
