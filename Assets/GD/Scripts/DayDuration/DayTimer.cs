using UnityEngine;
using System.Collections;

public class DayTimer : MonoBehaviour
{
    [SerializeField] private float dayDuration = 120f;
    [SerializeField] private CustomerManager customerManager;

    private void Start()
    {
        if (customerManager == null)
        {
            Debug.LogError("CustomerManager non assigné au DayTimer !");
            return;
        }

        StartCoroutine(DayCountdown());
    }

    private IEnumerator DayCountdown()
    {
        Debug.Log("Début de la journée !");

        customerManager.StartDay();

        float elapsedTime = 0f;

        // Attend que tous les clients soient servis
        // OU que la durée maximale soit atteinte.
        while (elapsedTime < dayDuration && !customerManager.IsDayComplete)
        {
            yield return null;

            elapsedTime += Time.unscaledDeltaTime;
        }

        EndDay();
    }

    private void EndDay()
    {
        Debug.Log("Fin de la journée !");

        customerManager.EndDay();
    }
}