using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour,
    IPointerEnterHandler, IPointerClickHandler
{
    [Header("Audio Settings")]
    public AudioSource audioSource;

    [Header("Button Sounds")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    // Son lorsque le curseur ou le rayon VR
    // arrive sur le bouton
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    // Son lorsque le bouton est cliqué
    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
