using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    [Header("Typewriter Settings")]

    public TMP_Text creditsText;

    [Tooltip("Temps entre chaque caractère, en secondes")]
    [Min(0f)]
    public float typingSpeed = 0.05f;

    private Coroutine typingCoroutine;

    private void OnEnable()
    {
        if (creditsText == null)
            return;

        // Mettre à jour les informations du texte
        creditsText.ForceMeshUpdate();

        // Masquer tous les caractères
        creditsText.maxVisibleCharacters = 0;

        // Lancer l'effet machine à écrire
        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        int totalCharacters = creditsText.textInfo.characterCount;

        for (int i = 0; i < totalCharacters; i++)
        {
            creditsText.maxVisibleCharacters = i + 1;

            yield return new WaitForSeconds(typingSpeed);
        }

        // Afficher tous les caractères à la fin
        creditsText.maxVisibleCharacters = totalCharacters;

        typingCoroutine = null;
    }

    private void OnDisable()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (creditsText != null)
        {
            creditsText.maxVisibleCharacters = int.MaxValue;
        }
    }
}
