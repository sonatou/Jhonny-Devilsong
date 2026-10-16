using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private EventReference hoverSound;
    [SerializeField] private EventReference clickSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Play(hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Play(clickSound);
    }

    void Play(EventReference sound)
    {
        if (!sound.IsNull)
            RuntimeManager.PlayOneShot(sound);
    }
}
