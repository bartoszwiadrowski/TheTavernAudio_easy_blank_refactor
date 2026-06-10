using UnityEngine;
using FMODUnity;
using System.Collections;

// Losowo odtwarza event FMOD w zadanych odstępach czasu.
public class RandomMouseSqueak : MonoBehaviour
{
    public EventReference mouseSqueakEvent;

    [Header("Czas między piskami (w sekundach)")]
    public float minWaitTime = 15f; 
    public float maxWaitTime = 45f; 

    void Start()
    {
        // Uruchom nieskończoną pętlę odliczania.
        StartCoroutine(SqueakRoutine());
    }

    private IEnumerator SqueakRoutine()
    {
        while (true)
        {
            // Wylosuj czas do następnego pisku.
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            
            // Poczekaj wylosowaną ilość sekund.
            yield return new WaitForSeconds(waitTime);

            // Zagraj dźwięk.
            PlaySqueak();
        }
    }

    private void PlaySqueak()
    {
        if (!mouseSqueakEvent.IsNull)
        {
            // Utwórz, ustaw w przestrzeni 3D, odtwórz i zapomnij.
            var squeakInstance = RuntimeManager.CreateInstance(mouseSqueakEvent);
            squeakInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            squeakInstance.start();
            
            // Release() sprawia, że FMOD sam zniszczy obiekt po wybrzmieniu tych 2 sekund.
            squeakInstance.release(); 
        }
    }
}