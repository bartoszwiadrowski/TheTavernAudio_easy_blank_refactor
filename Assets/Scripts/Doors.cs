using FMODUnity;
using System.Collections;
using UnityEngine;

// Zarządza drzwiami i dźwiękiem FMOD.
public class Doors : MonoBehaviour, IInteractable
{
    // Czas obrotu drzwi.
    public float rotationDuration = 1f;

    [SerializeField] private bool doorsOpened = true;
    [SerializeField] private bool isRotating = false;

    // FMOD - Referencje.
    private FMOD.Studio.EventInstance doorsSoundInstance;
    public EventReference doorsEvent;

    private FMOD.Studio.EventInstance insideRoomSnapshot;
    public EventReference insideRoomSnap;

    // Główna interakcja z drzwiami.
    public void Interact()
    {
        if (isRotating) return;

        doorsOpened = !doorsOpened;

        // Zawsze graj skrzypienie przy starcie ruchu.
        PlaySound("Open");

        StartCoroutine(RotateDoors(doorsOpened ? -65 : 65));
        RoomsSnap();
    }

    // Korutyna obrotu drzwi.
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

        // Wyrównaj po zakończeniu ruchu.
        transform.rotation = targetRot;
        isRotating = false;

        // Graj trzaśnięcie tylko po zamknięciu.
        if (!doorsOpened)
        {
            PlaySound("Close");
        }
    }

    // Konfiguracja i start dźwięku FMOD z wybraną etykietą.
    private void PlaySound(string parameterLabel)
    {
        if (doorsSoundInstance.isValid())
        {
            // Wycisz poprzedni dźwięk płynnie.
            doorsSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            doorsSoundInstance.release();
        }

        doorsSoundInstance = RuntimeManager.CreateInstance(doorsEvent);
        doorsSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));

        // Przekaż etykietę "Open" lub "Close".
        doorsSoundInstance.setParameterByNameWithLabel("Doors", parameterLabel);
        doorsSoundInstance.start();
    }

    // Obsługa snapshotu.
    private void RoomsSnap()
    {
        RoomAmbient roomAmbient = FindObjectOfType<RoomAmbient>();
        if (roomAmbient == null || !roomAmbient.ambientActivated) return;

        if (doorsOpened)
        {
            if (insideRoomSnapshot.isValid())
            {
                insideRoomSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                insideRoomSnapshot.release();
            }
        }
        else
        {
            insideRoomSnapshot = RuntimeManager.CreateInstance(insideRoomSnap);
            insideRoomSnapshot.start();
        }
    }
}