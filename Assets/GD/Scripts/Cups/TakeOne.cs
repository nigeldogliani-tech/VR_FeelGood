using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TakeOne : MonoBehaviour
{
    [Tooltip("le prefab de la tasse à ")]
    [SerializeField] private XRGrabInteractable objectSpawned;
    [SerializeField] private XRBaseInteractable spawner;

    private void Awake()
    {
        spawner = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        spawner.selectEntered.AddListener(OncupGrabbed);
    }

    private void OnDisable()
    {
        spawner.selectEntered.RemoveListener(OncupGrabbed);
    }

    private void OncupGrabbed(SelectEnterEventArgs args)
    {
        IXRSelectInteractor handOfPlayer =args.interactorObject;
        XRInteractionManager manager = spawner.interactionManager;

        manager.SelectExit(handOfPlayer, spawner);

        XRGrabInteractable newCup = Instantiate(objectSpawned, handOfPlayer.GetAttachTransform(spawner).position, handOfPlayer.GetAttachTransform(spawner).rotation);

        manager.SelectEnter(handOfPlayer, newCup);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
