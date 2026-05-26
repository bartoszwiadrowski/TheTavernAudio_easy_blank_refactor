using UnityEngine;
using System.Collections;
using FMODUnity;

public class BeerSpill_Randomizer : MonoBehaviour
{
    public EventReference beerSpillEvent;

    [Header("Ustawienia czasu (w sekundach)")]
    public float minWaitTime = 10f;
    public float maxWaitTime = 30f;

    void Start()
    {
        // Rozpocznij pêtlê czasow¹.
        StartCoroutine(PlaySoundRoutine());
    }

    private IEnumerator PlaySoundRoutine()
    {
        while (true)
        {
            // Czekaj zadan¹, losow¹ iloœæ sekund.
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // ZMIANA DLA 3D: Podajemy pozycjê obiektu (transform.position)
            // DŸwiêk zostanie odtworzony dok³adnie tam, gdzie jest ten obiekt.
            RuntimeManager.PlayOneShot(beerSpillEvent, transform.position);
        }
    }
}