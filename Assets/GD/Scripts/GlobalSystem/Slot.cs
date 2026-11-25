using UnityEngine;

public enum SlotType
{
    FreePlacement,
    FixedPlacement
}

public class Slot : MonoBehaviour
{
    [SerializeField] private MakeCoffee makeCoffee;
    [Header("Slot Settings")]
    public SlotType slotType;

    public string acceptedItemID;
    public Transform snapPoint;

    private InteractableObject currentObject;

    [Header("Delivery Settings")]
    [SerializeField] private bool isDeliverySlot;

    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private CoffeeIsInPreparation coffeeIsInPreparation;

    private GameObject gameObject1;


    private void OnTriggerStay(Collider other)
    {
        InteractableObject obj = other.GetComponentInParent<InteractableObject>();
        gameObject1 = other.gameObject;

        if (obj == null)
        {
            return;
        }

        // Demande au Slot de placer l'objet.
        TryPlaceObject(obj);
    }


    // Méthode utilisable par le Slot et par d'autres scripts.
    public bool TryPlaceObject(InteractableObject obj)
    {
        if (obj == null)
        {
            return false;
        }

        // Vérifie que le Slot est libre.
        if (currentObject != null)
        {
            return false;
        }

        // Vérifie que l'objet n'est pas déjà placé.
        if (obj.IsSnapped)
        {
            return false;
        }

        // Vérifie que le joueur ne tient pas l'objet.
        if (obj.IsGrabbed)
        {
            return false;
        }

        // Vérifie l'ID si renseigné(rien = libre)
        if (!string.IsNullOrEmpty(acceptedItemID) && obj.itemID != acceptedItemID)
        {
            return false;
        }

        // sp bar acceptent que final drink
        if (isDeliverySlot && !obj.isFinalDrink)
        {
            return false;
        }

        if (snapPoint == null)
        {
            Debug.LogWarning("Le SnapPoint du Slot n'est pas renseigné !");
            return false;
        }

        // Place l'objet selon le type de Slot.
        obj.SnapToSlot(snapPoint, this, slotType);
        coffeeIsInPreparation.rbCup = gameObject1;

        // Mémorise l'objet placé.
        currentObject = obj;
        if (makeCoffee != null)
        {
            CupState cupState = obj.GetComponent<CupState>();

            if (cupState != null)
            {
                makeCoffee.cupIsEmpty = cupState.cupIsEmpty;
            }

            makeCoffee.cupIsPlaced = true;
            makeCoffee.SetCurrentCup(obj);
        }
        return true;
    }

    public bool ValidateCurrentOrder()
    {
        //c'est delivery slot? 
        if (!isDeliverySlot)
        {
            Debug.LogWarning("po delivery slot!");
            return false;
        }

        // y'a une boisson ouuu? 
        if (currentObject == null)
        {
            Debug.Log("po de boisson !");
            return false;
        }

        //est ce que ya un customer manager? 
        if (customerManager == null)
        {
            Debug.LogWarning("po de cm !");
            return false;
        }

        // Ajoute un point si la commande est validée.
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddPoint();
        }
        else
        {
            Debug.LogWarning("ScoreManager introuvable !");
        }

        //essaye de valider 
        bool orderValidated = customerManager.TryValidateOrder(currentObject.itemID);

        if (!orderValidated)
        {
            Debug.Log("personne veut de toi");
            return false;
        }

        Debug.Log("Boisson consommée : " + currentObject.itemID);

        //memorise boisson avant de free slot 
        InteractableObject deliveredObject = currentObject;

        ClearSlot(deliveredObject);

        //bye bye boisson
        Destroy(deliveredObject.gameObject);

        return true;
    }

    public void FillCup()
    {
        gameObject1.GetComponent<CupState>().cupIsEmpty = makeCoffee.cupIsEmpty;
    }

    public void ClearSlot(InteractableObject obj)
    {
        if (currentObject == obj)
        {
            if (makeCoffee != null)
            {
                makeCoffee.cupIsPlaced = false;
                makeCoffee.SetCurrentCup(null);
            }

            currentObject = null;
        }
    }
}