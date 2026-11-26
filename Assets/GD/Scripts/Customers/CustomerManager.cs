using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerManager : MonoBehaviour
{
    [Header("Customer Settings")]
    [SerializeField] private GameObject customerPrefab;

    [Header("Daily Customer Settings")]// au début de la journée alea choisit nb total clients entre ces deux nb
    [SerializeField] private int minCustomers = 5;
    [SerializeField] private int maxCustomers = 12;
    [SerializeField] private float spawnCheckInterval = 0.25f;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Managers")]
    [SerializeField] private DrinkSelector drinkSelector;
    [SerializeField] private SpawnPointManager spawnPointManager;


    private List<Customer> activeCustomers = new List<Customer>();
    private int totalCustomersToday;
    private int spawnedCustomers;
    private int nextOrderID = 1;

    private bool dayActive;
    public bool IsDayComplete => spawnedCustomers >= totalCustomersToday && activeCustomers.Count == 0;

    private void Awake()
    {
        if (drinkSelector == null)
        {
            Debug.LogError("DrinkSelector  po assigné");
        }

        if (spawnPointManager == null)
        {
            Debug.LogError("spawnPointManager po assigné ");
        }
    }
    public void StartDay()
    {
        if (dayActive)
        {
            return;
        }

        if (drinkSelector != null && spawnPointManager == null)
        {
            return;
        }

        dayActive = true;

        totalCustomersToday = Random.Range(minCustomers,maxCustomers + 1);

        spawnedCustomers = 0;
        drinkSelector.ResetSelector();
        Debug.Log("Nb clients 2day : " + totalCustomersToday);

        StartCoroutine(SpawnCustomersRoutine());
    }

    public void EndDay()
    {
        dayActive = false;

        StopAllCoroutines();

        Debug.Log("Fin de journée : arrêt des apparitions de clients.");
    }

    //apparitions



    private IEnumerator SpawnCustomersRoutine()
    {
        while (spawnedCustomers < totalCustomersToday && dayActive)
        {
            if (spawnPointManager.GetFreeSpawnPoint() != -1)
            {
                TrySpawnCustomer();

                // pause avant apparition client 
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                // po de Sp libre donc check reguliers au cas ou un client est parti
                yield return new WaitForSeconds(spawnCheckInterval);
            }
        }

        Debug.Log("Tous les clients prévus ont été créés.");
    }




    //svp apparaissez
    private void TrySpawnCustomer()
    {
        if (customerPrefab == null)
        {
            Debug.LogWarning("Customer Prefab po assigné");
            return;
        }

        int availablePoint = spawnPointManager.GetFreeSpawnPoint();//sp dispo 

        // po spawnppint dispo
        if (availablePoint == -1)
        {
            Debug.Log("All SpawnPoints occupied");
            return;
        }

        string randomDrink = drinkSelector.ChooseRandomDrink();// alea boisson

        if (randomDrink == null)
        {
            return;
        }

        Transform spawnPoint = spawnPointManager.GetSpawnPoint(availablePoint);
        if (spawnPoint == null)
        {
            return;
        }

        GameObject newCustomer = Instantiate(customerPrefab,spawnPoint.position,spawnPoint.rotation);//instance prefab au sp
        Customer customer = newCustomer.GetComponent<Customer>();//et ici c'est son script 

        if (customer == null)
        {
            Debug.LogError("Prefab n'a pas de script Customer");
            Destroy(newCustomer);
            return;
        }

        string orderID = "ORDER_" + nextOrderID;//toujours l'id des commandes , chacun des clients en a un diff

        nextOrderID++;

        customer.Initialize(randomDrink,orderID,this,availablePoint);

        if (!spawnPointManager.OccupySpawnPoint(availablePoint))
        {
            Destroy(new Customer());
            return;
        }


        drinkSelector.RegisterDrink(randomDrink);

        activeCustomers.Add(customer);

        spawnedCustomers++;

        Debug.Log("Nouveau client : " + orderID +" | Boisson : " + randomDrink);
    }

    public void ReleaseSpawnPoint(int index)
    {
        spawnPointManager.ReleaseSpawnPoint(index);
    }

    // client enlevé de la liste 
    public void RemoveCustomer(Customer customer)
    {
        activeCustomers.Remove(customer);
    }

    // qui veut quoi ?


    public bool TryValidateOrder(string drinkID)
    {
        Customer matchingCustomer = null;

        // qui veut la boisson
        foreach (Customer customer in activeCustomers)
        {
            if (customer == null)
            {
                continue;
            }

            if (customer.requestedDrinkID == drinkID)
            {
                matchingCustomer = customer;
                break;
            }
        }

        // personne la veut...
        if (matchingCustomer == null)
        {
            Debug.Log( "tié pas invité : " + drinkID);

            return false;
        }

        Debug.Log("Commande validée : " + matchingCustomer.orderID);

        // Validation en dehors du foreach
        matchingCustomer.CompleteOrder();

        return true;
    }
}