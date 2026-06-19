using UnityEngine;
using UnityEngine.EventSystems;

public class HoverSound : MonoBehaviour, IPointerEnterHandler
{
    public AudioClip sound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (sound != null)
            AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position);
    }
}