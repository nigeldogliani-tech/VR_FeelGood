using System.Collections;
using UnityEngine;

public class Tutoriaux : MonoBehaviour
{
    [SerializeField] private WhoIsGrab ObjectGrab;
    private GameObject objectCopie;
    public Transform pointStart;
    public Transform pointEnd;
    public float deltaTime = 2f;
    private Coroutine movement;
    private GameObject currentObject;
    private float t = 0f;

    private bool doOnce;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointStart = ObjectGrab.gameObject.transform;
    }

    private IEnumerator MoveLoop(GameObject obj)
    {
        while (obj != null)
        {
            yield return Move(obj, pointStart.position, pointEnd.position);
            //yield return Move(obj, pointEnd.position, pointStart.position);
        }
    }

    private IEnumerator Move(GameObject obj, Vector3 a, Vector3 b)
    {
        while (t < deltaTime && obj != null)
        {
            t += Time.deltaTime;

            obj.transform.position = Vector3.Lerp(a, b, t / deltaTime);

            yield return null;
        }

        if (obj != null)
        {
            obj.transform.position = b;
        }   
    }
    void Update()
    {
        if (objectCopie == null && pointEnd == null && pointStart == null)
        {
            Debug.LogError("Références manquantes !");
            return;
        }
        objectCopie = ObjectGrab.grabbedObject;
        if (objectCopie != null)
        {
            if (doOnce == false)
            {
                currentObject = Instantiate(objectCopie, pointStart.position, pointStart.rotation);
                doOnce = true;
                movement = StartCoroutine(MoveLoop(currentObject));
            }
        }
    }
}
