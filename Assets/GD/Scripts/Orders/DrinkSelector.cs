using UnityEngine;
using System.Collections.Generic;

public class DrinkSelector : MonoBehaviour
{
    [Header("Available Drinks")]
    [SerializeField]
    private string[] availableDrinks =
    {
        "coffee",
        "cappu",
        "choco",
        "tea"
    };

    private string lastDrinkID = "";
    private int consecutiveDrinkCount = 0;

    private const int maxConsecutiveDrinks = 3;

    public void ResetSelector()
    {
        lastDrinkID = "";
        consecutiveDrinkCount = 0;
    }

    public string ChooseRandomDrink()
    {
        List<string> possibleDrinks = new List<string>();

        foreach (string drink in availableDrinks)// on regarde toutes boissons dispo
        {
            if (string.IsNullOrEmpty(drink))
                continue;

            if (consecutiveDrinkCount >= maxConsecutiveDrinks//si 3x même boisson= on exclut la dernière boisson tirée 
                && drink == lastDrinkID)
                continue;

            possibleDrinks.Add(drink);
        }

        if (possibleDrinks.Count == 0)//si aucune boisson dispo = autre tirage 
        {
            foreach (string drink in availableDrinks)
            {
                if (!string.IsNullOrEmpty(drink))
                    possibleDrinks.Add(drink);
            }
        }

        if (possibleDrinks.Count == 0)
        {
            Debug.LogError("Aucune boisson disponible !");
            return null;
        }

        return possibleDrinks[Random.Range(0, possibleDrinks.Count)];
    }

    public void RegisterDrink(string drinkID)//compteur à jour après chaque clients 
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
}