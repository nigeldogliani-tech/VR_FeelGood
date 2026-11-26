using UnityEngine;
using System.Collections;

[System.Serializable]
public class DrinkBubbleIcon
{
    public string drinkID;
    public Sprite drinkSprite;
}

public class CustomerInteraction : MonoBehaviour
{
    [Header("Bubble")]
    [SerializeField] private SpriteRenderer bubbleBackgroundRenderer;
    [SerializeField] private SpriteRenderer drinkIconRenderer;
    [SerializeField] private Color receivedDrinkColor = Color.green;

    [Header("Drink Icons")]
    [SerializeField] private DrinkBubbleIcon[] drinkIcons;

    [Header("Consumption Time")]
    [SerializeField] private float minConsumptionTime = 3f;
    [SerializeField] private float maxConsumptionTime = 8f;

    [Header("Consumption Sounds")]
    [SerializeField] private AudioSource consumptionAudioSource;
    [SerializeField] private AudioClip[] consumptionSounds;
    [SerializeField] private float soundDelay = 1.5f;

    private Customer customer;
    private bool drinkReceived;

    private void Awake()
    {
        customer = GetComponent<Customer>();
        if(consumptionAudioSource == null)
        {
            consumptionAudioSource = GetComponent<AudioSource>();
        }
    }

    public void SetDrinkBubble(string drinkID)//sprite boisson demandée dans la bulle 
    {
        if (drinkIconRenderer == null)
        {
            Debug.LogWarning("Bubble SpriteRenderer manquant !");
            return;
        }

        foreach (DrinkBubbleIcon icon in drinkIcons)
        {
            if (icon.drinkID == drinkID)
            {
                drinkIconRenderer.sprite = icon.drinkSprite;
                return;
            }
        }
        Debug.LogWarning("Sprite non trouvé pour : " + drinkID);
    }

    public bool ReceiveDrink(GameObject drinkVisual)//bulle passe verte, boisson ciao +1pt 
    {
        if (drinkReceived)
        {
            return false;
        }
        drinkReceived = true;

        if (bubbleBackgroundRenderer != null)// La bulle devient verte
        {
            bubbleBackgroundRenderer.color = receivedDrinkColor;
        }
        if (drinkVisual != null) // La boisson livrée disparaît
        {
            Destroy(drinkVisual);
        }
        
        if (ScoreManager.Instance != null)// Le score augmente à la réception
        {
            ScoreManager.Instance.AddPoint();
        }
        
        StartCoroutine(ConsumptionRoutine());

        return true;
    }

    private IEnumerator ConsumptionRoutine()
    {
        float consumptionTime = Random.Range(minConsumptionTime, maxConsumptionTime);

        Debug.Log("Le client consomme pendant : " + consumptionTime + " secondes");

         float actualDelay = Mathf.Min(soundDelay, consumptionTime);// Attend un peu avant de jouer le son
        yield return new WaitForSeconds(actualDelay);

        AudioClip selectedSound = null;

        if (consumptionSounds != null && consumptionSounds.Length > 0)
        {
            selectedSound = consumptionSounds[Random.Range(0, consumptionSounds.Length)];
        }

        float remainingTime = consumptionTime - actualDelay;

        if (selectedSound != null && consumptionAudioSource != null)
        {
            consumptionAudioSource.PlayOneShot(selectedSound);
            remainingTime = Mathf.Max(remainingTime, selectedSound.length);// Evite que le client disparaisse avant la fin du son
        }
        else
        {
            Debug.LogWarning("Son de consommation ou AudioSource manquant !");
        }

        yield return new WaitForSeconds(remainingTime);

        if (customer != null)
        {
            customer.CompleteOrder();
        }
    }
}