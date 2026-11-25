using UnityEngine;

public class MilkPourer : MonoBehaviour
{
    [SerializeField] private Transform spoutPoint;
    [SerializeField] private float pourAngle = 50f;
    [SerializeField] private float pourDistance = 1f;
    [SerializeField] private float milkSpeed = 0.5f;
    


    private void Update()
    {
        float angle = Vector3.Angle(transform.up, Vector3.up);
        if (angle< pourAngle)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(spoutPoint.position, Vector3.down,
    out hit, pourDistance))
        {
            CupState cup = hit.collider.GetComponentInParent<CupState>();

            if (cup != null)
            {
                //lancer fonction cup.AddMilk(milkSpedd * time.deltaTime);
            }
        }


        Debug.DrawRay(spoutPoint.position, Vector3.down * pourDistance, Color.blue);
    }
}
