using UnityEngine;
using FMODUnity;

// Zarządza snapshotem 'Outside' na podstawie tagu podłoża.
public class Outside_foot_switch : MonoBehaviour
{
    [SerializeField]
    private bool snapshotActivated = false;

    private float distToGround;

    private FMOD.Studio.EventInstance outsideSnapshotInstance;
    public EventReference outsideSnapshot;

    void Start()
    {
        // Pobierz odległość do podłoża.
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

            // Włącz snapshot (Na zewnątrz).
            if (tag == "Outside" && !snapshotActivated)
            {
                ToggleSnapshot(true);
            }
            // Wyłącz snapshot (Wewnątrz).
            else if ((tag == "Inside_stone" || tag == "Inside_wood") && snapshotActivated)
            {
                ToggleSnapshot(false);
            }
        }
    }

    private void ToggleSnapshot(bool activate)
    {
        if (activate)
        {
            // Uruchom instancję.
            outsideSnapshotInstance = FMODUnity.RuntimeManager.CreateInstance(outsideSnapshot);
            outsideSnapshotInstance.start();
        }
        else
        {
            if (outsideSnapshotInstance.isValid())
            {
                // Pozwól na płynne wyciszenie (wymaga AHDSR w FMOD).
                outsideSnapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                outsideSnapshotInstance.release();
            }
        }

        snapshotActivated = activate;
    }
}