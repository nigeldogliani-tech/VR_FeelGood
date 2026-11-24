using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerManager : MonoBehaviour
{
    [Header("Customer Settings")]
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Daily Customer Settings")]// au début de la journée alea choisit nb total clients entre ces deux nb
    [SerializeField] private int minCustomers = 5;
    [SerializeField] private int maxCustomers = 12;
    [SerializeField] private float spawnCheckInterval = 0.25f;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Available Drinks")]
    [SerializeField]
    private string[] availableDrinks =
    {
        "coffee",
        "cappu",
        "choco",
        "tea"
    };
    private List<Customer> activeCustomers = new List<Customer>();
    private bool[] occupiedSpawnPoints;
    private int totalCustomersToday;
    private int spawnedCustomers;
    private int nextOrderID = 1;
    private bool dayActive;
    private string lastDrinkID = "";
    private int consecutiveDrinkCount = 0;
    private const int maxConsecutiveDrinks = 3;
    public bool IsDayComplete => spawnedCustomers >= totalCustomersToday && activeCustomers.Count == 0;

    private void Awake()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Aucun SpawnPoint assigné au CustomerManager !");
            return;
        }

        occupiedSpawnPoints = new bool[spawnPoints.Length];
    }
    public void StartDay()
    {
        if (dayActive)
        {
            return;
        }

        dayActive = true;

        totalCustomersToday = Random.Range(minCustomers,maxCustomers + 1);

        spawnedCustomers = 0;
        lastDrinkID = "";

        consecutiveDrinkCount = 0;//réinitialise compteurs boissons 

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
            if (GetFreeSpawnPoint() != -1)
            {
                TrySpawnCustomer();

                // Attend avant de faire apparaître un autre client.
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                // Aucun SpawnPoint libre :
                // on vérifie régulièrement si un client est parti.
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

        int availablePoint = GetFreeSpawnPoint();//sp dispo 

        // po spawnppint dispo
        if (availablePoint == -1)
        {
            Debug.Log("All SpawnPoints occupied");
            return;
        }

        string randomDrink = ChooseRandomDrink();// alea boisson

        if (randomDrink == null)
        {
            return;
        }

        Transform spawnPoint = spawnPoints[availablePoint];

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
        RegisterDrink(randomDrink);

        occupiedSpawnPoints[availablePoint] = true;

        activeCustomers.Add(customer);

        spawnedCustomers++;

        Debug.Log("Nouveau client : " + orderID +" | Boisson : " + randomDrink);
    }

    private string ChooseRandomDrink()
    {
        List<string> possibleDrinks = new List<string>();

        // on regarde toutes boissons dispo
        foreach (string drink in availableDrinks)
        {
            if (string.IsNullOrEmpty(drink))
            {
                continue;
            }

            //si 3x même boisson= on exclut la dernière boisson tirée 
            if (consecutiveDrinkCount >= maxConsecutiveDrinks && drink == lastDrinkID)
            {
                continue;
            }

            possibleDrinks.Add(drink);
        }

        //si aucune boisson dispo = autre tirage 
        if (possibleDrinks.Count == 0)
        {
            Debug.LogWarning("Impossible de varier les boissons : aucune alternative disponible.");

            foreach (string drink in availableDrinks)
            {
                if (!string.IsNullOrEmpty(drink))
                {
                    possibleDrinks.Add(drink);
                }
            }
        }

        if (possibleDrinks.Count == 0)
        {
            Debug.LogError("Aucune boisson disponible dans Available Drinks !");
            return null;
        }

        return possibleDrinks[Random.Range(0, possibleDrinks.Count)];
    }

    private void RegisterDrink(string drinkID)//compteur à jour après chaque clients 
    {
        if (drinkID == lastDrinkID)
        {
            consecutiveDrinkCount++;
        }
        else
        {
            lastDrinkID = drinkID;
            consecutiveDrinkCount = 1;
        }
    }

    // quel SP est libre??
    private int GetFreeSpawnPoint()
    {
        List<int> freePoints = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!occupiedSpawnPoints[i])
            {
                freePoints.Add(i);
            }
        }

        if (freePoints.Count == 0)
        {
            return -1;
        }

        // alea sp choisit
        return freePoints[Random.Range(0, freePoints.Count)];
    }

    // sp libéré quand un client part 
    public void ReleaseSpawnPoint(int index)
    {
        if (index >= 0 && index < occupiedSpawnPoints.Length)
        {
            occupiedSpawnPoints[index] = false;
        }
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

        Debug.Log(
            "Commande validée : " + matchingCustomer.orderID
        );

        // Validation en dehors du foreach
        matchingCustomer.CompleteOrder();

        return true;
    }
}
