using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class ButtonSound : MonoBehaviour,
    IPointerEnterHandler, IPointerClickHandler
{
    [Header("Audio Settings")]
    public AudioSource audioSource;

    [Header("Button Sounds")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Button Action")]
    public Button button;
    public UnityEvent actionAfterSound;

    private bool isPlaying = false;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
        {
            // On remplace l'action immédiate par notre propre système
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(PlayClickSound);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
    public void PlayClickSound()
    {
        if (isPlaying)
            return;

        StartCoroutine(PlaySoundThenAction());
    }

    private IEnumerator PlaySoundThenAction()
    {
        isPlaying = true;

        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);

            yield return new WaitForSeconds(clickSound.length);
        }

        // Exécuter l'action après le son
        actionAfterSound.Invoke();

        isPlaying = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // L'action est gérée par Button.onClick
    }
}
