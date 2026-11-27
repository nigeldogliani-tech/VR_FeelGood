using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Tutoriaux : MonoBehaviour
{
    [SerializeField] private WhoIsGrab ObjectGrab;
    [SerializeField] private MakeCoffee makeCoffee;
    private GameObject objectCopie;
    public Transform pointStart;
    public Transform pointEnd;
    public float deltaTime = 2f;
    private Coroutine movement;
    private GameObject currentObject;
    private float t = 0f;
    [SerializeField] private Transform crankMachinePosition;
    [SerializeField] private Transform stampedTimePosition;
    [SerializeField] private Transform coffeeTimePosition;

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

            if (obj.transform.position == pointEnd.position)
            {
                obj.transform.position = pointStart.position;
                t = 0;
            }
                
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

    private void createTutorial()
    {
        doOnce = true;
        currentObject = Instantiate(objectCopie, pointStart.position, Quaternion.Euler(0f, 90f, 0f));
        for (int i = 0; i < currentObject.transform.childCount; i++)
        {
            GameObject child = currentObject.transform.GetChild(i).gameObject;
            if (child != null && child.GetComponent<Renderer>() != null)
            {
                child.GetComponent<Renderer>().material.color = Color.white;
                child.GetComponent<Collider>().enabled = false;

            }
        }
        currentObject.GetComponent<Renderer>().material.color = Color.white;
        currentObject.GetComponent<Rigidbody>().freezeRotation = true;
        currentObject.GetComponent<Collider>().enabled = false;
        if (currentObject.GetComponent<InteractableObject>() != false)
        {
            currentObject.GetComponent<InteractableObject>().enabled = false;
        }
        else
        {
            currentObject.GetComponent<XRGrabInteractable>().enabled = false;
        }
        movement = StartCoroutine(MoveLoop(currentObject));
    }

    void Update()
    {
        objectCopie = ObjectGrab.grabbedObject;

        if (pointEnd == null || pointStart == null || ObjectGrab == null)
        {
            Debug.LogError("Références manquantes !");
            return;
        }

        if (objectCopie != null)
        {
            if (doOnce == false)
            {
                if (objectCopie.name == "manche")
                {
                    if (makeCoffee.isEmpty == true)
                    {
                        pointEnd = crankMachinePosition;
                        createTutorial();
                    }
                    else if (makeCoffee.isEmpty == false && makeCoffee.isTamped == false)
                    {
                        pointEnd = stampedTimePosition;
                        createTutorial();
                    }
                    else if (makeCoffee.isEmpty == false && makeCoffee.isTamped == true)
                    {
                        pointEnd = coffeeTimePosition;
                        createTutorial();
                    }
                }
            }
        }

        else
        {
            if (currentObject != null)
            {
                if (movement != null)
                {
                    StopCoroutine(movement);
                } 

                Destroy(currentObject);

                currentObject = null;
                movement = null;
                doOnce = false;
            }
        }
    }
}
