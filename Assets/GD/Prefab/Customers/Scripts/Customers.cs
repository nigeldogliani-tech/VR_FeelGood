using UnityEngine;
using TMPro; 

public class NewMonoBehaviourScript3 : MonoBehaviour
{
    [Header("Customer Order")]
    public string orderID;
    public string requestedDrinkID;

    [Header("Order Bubble")]
    [SerializeField] private TMP_Text orderText;

    public void Initialize(string drinkID)// quand client apparait dans la scène
    {
        requestedDrinkID = drinkID;

        UpdateOrderBubble();
    }

    public void UpdateOrderBubble()
    {
        if (orderText == null)
        {
            Debug.Log("Order text po assigné");
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
                orderText.text = "Chocolat Chaud";
                break;

            case "tea":
                orderText.text = "Thé";
                break;

            default:
                orderText.text = "commande incoco";
                break; 
        }
    }

    public void CompleteOrder()//bye bye le client 
    {
        Destroy(gameObject);
    }
}
