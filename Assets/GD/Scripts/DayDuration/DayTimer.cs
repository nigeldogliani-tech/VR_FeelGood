using UnityEngine;
using System.Collections;

public class DayTimer : MonoBehaviour
{
    [SerializeField] private float dayDuration = 120f;
    [SerializeField] private CustomerManager customerManager;

    private void Start()
    {
        StartCoroutine(DayCountdown());
    }

    IEnumerator DayCountdown()
    {
        Debug.Log("Début de la journée !");

        customerManager.StartDay();

        yield return new WaitForSeconds(dayDuration);

        EndDay();
    }

    void EndDay()
    {
        Debug.Log("Fin de la journée !");

        customerManager.EndDay();
    }
}
