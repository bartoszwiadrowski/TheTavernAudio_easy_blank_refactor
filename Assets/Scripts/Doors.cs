using FMODUnity;
using System.Collections;
using UnityEngine;

// Zarządza drzwiami i dźwiękiem FMOD.
public class Doors : MonoBehaviour, IInteractable
{
    public float rotationDuration = 1f;

    [SerializeField] private bool doorsOpened = true;
    [SerializeField] private bool isRotating = false;

    private FMOD.Studio.EventInstance doorsSoundInstance;
    public EventReference doorsEvent;

    private FMOD.Studio.EventInstance insideRoomSnapshot;
    public EventReference insideRoomSnap;

    public void Interact()
    {
        if (isRotating) return;

        doorsOpened = !doorsOpened;

        PlaySound("Open");

        StartCoroutine(RotateDoors(doorsOpened ? -65 : 65));
        
        // Odśwież stan snapshotu przy zmianie stanu drzwi.
        EvaluateSnapshot();
    }

    private IEnumerator RotateDoors(float targetAngle)
    {
        isRotating = true;
        float elapsedTime = 0f;
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = startRot * Quaternion.Euler(0, targetAngle, 0);

        while (elapsedTime < rotationDuration)
        {
            transform.rotation = Quaternion.Lerp(startRot, targetRot, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRot;
        isRotating = false;

        if (!doorsOpened)
        {
            PlaySound("Close");
        }
    }

    private void PlaySound(string parameterLabel)
    {
        if (doorsSoundInstance.isValid())
        {
            doorsSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            doorsSoundInstance.release();
        }

        doorsSoundInstance = RuntimeManager.CreateInstance(doorsEvent);
        doorsSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));

        doorsSoundInstance.setParameterByNameWithLabel("Doors", parameterLabel);
        doorsSoundInstance.start();
    }

    // Sprawdza warunki i włącza/wyłącza snapshot.
    public void EvaluateSnapshot()
    {
        RoomAmbient roomAmbient = FindObjectOfType<RoomAmbient>();
        bool playerInside = (roomAmbient != null && roomAmbient.ambientActivated);

        // Snapshot działa TYLKO gdy drzwi są zamknięte ORAZ gracz jest w środku.
        bool shouldBeActive = !doorsOpened && playerInside;

        if (shouldBeActive)
        {
            // Włącz snapshot, jeśli jeszcze nie działa.
            if (!insideRoomSnapshot.isValid())
            {
                insideRoomSnapshot = RuntimeManager.CreateInstance(insideRoomSnap);
                insideRoomSnapshot.start();
            }
        }
        else
        {
            // Wyłącz snapshot, jeśli jest aktywny.
            if (insideRoomSnapshot.isValid())
            {
                insideRoomSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                insideRoomSnapshot.release();
            }
        }
    }
}