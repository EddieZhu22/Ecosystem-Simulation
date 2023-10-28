using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlobalUISound : MonoBehaviour
{
    public AudioClip uiSound; // Drag your AudioClip here
    public AudioClip uiSound2; // Drag your AudioClip here
    private AudioSource audioSource;
    public GameManager manager;
    void Start()
    {
        // Initialize the AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = uiSound;
        AttachSoundToUI();
    }

    public void AttachSoundToUI()
    {
        Selectable[] uiElements = FindObjectsOfType<Selectable>();

        foreach (Selectable element in uiElements)
        {
            EventTrigger eventTrigger = element.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = element.gameObject.AddComponent<EventTrigger>();
            }

            // Check if a PointerClick trigger already exists
            bool alreadyHasClickTrigger = false;
            foreach (EventTrigger.Entry entry in eventTrigger.triggers)
            {
                if (entry.eventID == EventTriggerType.PointerClick)
                {
                    alreadyHasClickTrigger = true;
                    break;
                }
            }

            // If not, add a new PointerClick trigger
            if (!alreadyHasClickTrigger)
            {
                EventTrigger.Entry clickEntry = new EventTrigger.Entry();
                clickEntry.eventID = EventTriggerType.PointerClick;
                clickEntry.callback.AddListener((eventData) => { PlaySound(); });
                eventTrigger.triggers.Add(clickEntry);
            }
        }
    }


    void PlaySound()
    {
        audioSource.volume = (float) (0.15 * manager.settings.volume);
        audioSource.clip = uiSound;

        audioSource.PlayOneShot(uiSound);
        AttachSoundToUI(); // Re-attach sounds to all UI elements
    }
    public void PlaySound2()
    {
        audioSource.volume = (float)(0.85 * manager.settings.volume);
        audioSource.clip = uiSound2;
        audioSource.PlayOneShot(uiSound2);
    }
}
