using UnityEngine;
using FMODUnity;

// Przełącza snapshoty między zewnątrz a wewnątrz.
public class Outside_foot_switch : MonoBehaviour
{
    [Header("Snapshoty")]
    public EventReference outsideSnapshot;
    public EventReference insideSnapshot;

    private FMOD.Studio.EventInstance outsideInstance;
    private FMOD.Studio.EventInstance insideInstance;

    // Aktualna strefa, zapobiega spamowaniu eventów.
    private string currentZone = "None"; 
    private float distToGround;

    void Start()
    {
        // Pobierz odległość do podłogi.
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    void FixedUpdate()
    {
        ToggleSnapshotLogic();
    }

    private void ToggleSnapshotLogic()
    {
        RaycastHit hit;

        // Sprawdź tag podłoża pod graczem.
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distToGround + 0.5f))
        {
            string tag = hit.collider.tag;

            // Gracz wychodzi na zewnątrz.
            if (tag == "Outside" && currentZone != "Outside")
            {
                currentZone = "Outside";
                
                // Wyłącz wnętrze, włącz zewnątrz.
                StopSnapshot(ref insideInstance);
                PlaySnapshot(ref outsideInstance, outsideSnapshot);
            }
            // Gracz wchodzi do środka.
            else if ((tag == "Inside_stone" || tag == "Inside_wood") && currentZone != "Inside")
            {
                currentZone = "Inside";
                
                // Wyłącz zewnątrz, włącz wnętrze.
                StopSnapshot(ref outsideInstance);
                PlaySnapshot(ref insideInstance, insideSnapshot);
            }
        }
    }

    // Włącza wybrany snapshot.
    private void PlaySnapshot(ref FMOD.Studio.EventInstance instance, EventReference snapshotRef)
    {
        if (!snapshotRef.IsNull)
        {
            instance = RuntimeManager.CreateInstance(snapshotRef);
            instance.start();
        }
    }

    // Wyłącza snapshot z płynnym przejściem.
    private void StopSnapshot(ref FMOD.Studio.EventInstance instance)
    {
        if (instance.isValid())
        {
            // Płynne wyciszenie (dzięki AHDSR w FMOD).
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }
    }
}