using UnityEngine;

// Zarządza stanem ambientu pokoju w zależności od pozycji gracza.
public class Rooms : MonoBehaviour
{
    // Wywoływane przy wejściu w trigger.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RoomAmbient roomAmbient = FindObjectOfType<RoomAmbient>();
            if (roomAmbient != null)
            {
                roomAmbient.ambientActivated = true;
            }

            // Powiadom drzwi o wejściu gracza.
            FindObjectOfType<Doors>()?.EvaluateSnapshot();
        }
    }

    // Wywoływane przy wyjściu z triggera.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RoomAmbient roomAmbient = FindObjectOfType<RoomAmbient>();
            if (roomAmbient != null)
            {
                roomAmbient.ambientActivated = false;
            }

            // Powiadom drzwi o wyjściu gracza.
            FindObjectOfType<Doors>()?.EvaluateSnapshot();
        }
    }
}