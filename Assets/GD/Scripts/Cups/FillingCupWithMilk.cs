using UnityEngine;

public class FillingCupWithMilk : MonoBehaviour
{
    [SerializeField] private CupState cupState;
    private float milkAmount = 0f;
    private float maxMilk = 1f;

 public void AddMilk(float amount)
    {
        if (cupState.cupIsEmpty = true)
        {
            return;
        }

        cupState.filledWithMilk = true;

        if (milkAmount >= maxMilk)
        {
            return;
        }

        milkAmount += amount;

        milkAmount = Mathf.Clamp(milkAmount, 0f, maxMilk);

        Debug.Log("Quantité de lait : " + milkAmount);

    }
}
