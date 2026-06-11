using UnityEngine;
using FMODUnity;

// Zarządza krokami, skokami i lądowaniem
public class Footsteps : MonoBehaviour
{
    [Header("Eventy FMOD")]
    public EventReference footstepsEvent;
    public EventReference jumpEvent;
    public EventReference landEvent;

    private float lastFootstepTime = 0f;
    private float distToGround;

    [SerializeField] private bool isGrounded = true;
    [SerializeField] private bool isJumping = false;

    void Start()
    {
        // Pobierz odległość do ziemi
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    void Update()
    {
        // Skok na spację
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayJump();
        }

        // Obsługa kroków
        HandleFootsteps();
    }

    // Liczy czas między krokami
    private void HandleFootsteps()
    {
        bool isMoving = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (isMoving && IsGrounded())
        {
            // Szybsze kroki podczas biegu
            float footstepInterval = isRunning ? 0.25f : 0.5f;

            if (Time.time - lastFootstepTime > footstepInterval)
            {
                lastFootstepTime = Time.time;
                PlayFootsteps();
            }
        }
    }

    // Strzela promieniem i gra krok
    private void PlayFootsteps()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distToGround + 0.5f))
        {
            PlaySurfaceSound(footstepsEvent, hit.collider.tag, "Switch");
        }
    }

    // Obsługa skoku
    private void PlayJump()
    {
        if (IsGrounded())
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distToGround + 0.5f))
            {
                PlaySurfaceSound(jumpEvent, hit.collider.tag, "Jump_switch");
            }

            // Zmień stan na lot
            isGrounded = false;
            isJumping = true;
        }
    }

    // Kolizja po skoku (w momencie uderzenia w ziemię)
    private void OnCollisionEnter(Collision col)
    {
        if (!isGrounded && isJumping)
        {
            // Natychmiast zmień stan, aby zablokować podwójny dźwięk
            isGrounded = true;
            isJumping = false;

            PlayLanding();
        }
    }

    // Odtwarza lądowanie
    private void PlayLanding()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distToGround + 0.5f))
        {
            PlaySurfaceSound(landEvent, hit.collider.tag, "Land_Switch");
        }
    }

    // FMOD: Tworzy i odtwarza dźwięk z odpowiednim parametrem
    private void PlaySurfaceSound(EventReference eventRef, string surfaceTag, string paramName)
    {
        // Domyślna etykieta to Stone
        string surfaceLabel = "Stone";

        // Rozpoznawanie tagów podłoża
        switch (surfaceTag)
        {
            case "Wood":
            case "Inside_wood":
                surfaceLabel = "Wood";
                break;
            case "Stairs":
                surfaceLabel = "Stairs";
                break;
            case "Chandelier":
                surfaceLabel = "Chandelier";
                break;
            default:
                // Pozostałe tagi (np. Stone, Outside) użyją domyślnego Stone
                surfaceLabel = "Stone";
                break;
        }

        // Bramka dla skoku: jeśli to wyskok i podłoże nie jest drewnem, wymuś Stone
        if (paramName == "Jump_switch" && surfaceLabel != "Wood")
        {
            surfaceLabel = "Stone";
        }

        // Graj i zapomnij
        var soundInstance = RuntimeManager.CreateInstance(eventRef);
        soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));
        soundInstance.setParameterByNameWithLabel(paramName, surfaceLabel);
        soundInstance.start();
        soundInstance.release();
        
        Debug.Log($"Puszczam dźwięk: {paramName} na podłożu: {surfaceLabel}");
    }

    // Sprawdza czy gracz dotyka ziemi promieniem
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.5f);
    }
}