using UnityEngine;
using FMODUnity;

// Obsługa kroków, skoków i lądowań
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
    
    // Czas skoku (blokada fałszywych lądowań)
    private float jumpTime = 0f; 

    void Start()
    {
        // Dystans do ziemi
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    void Update()
    {
        bool currentlyGrounded = IsGrounded();

        // Lądowanie
        if (!isGrounded && currentlyGrounded)
        {
            // Ignoruj kolizje tuż po wybiciu (0.2s)
            if (isJumping && Time.time > jumpTime + 0.2f)
            {
                PlayLanding();
                isJumping = false; 
            }
            isGrounded = true; 
        }
        // Spadanie bez skoku
        else if (isGrounded && !currentlyGrounded)
        {
            isGrounded = false;
        }

        // Skok
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            PlayJump();
        }

        // Kroki
        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        bool isMoving = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Graj kroki tylko na ziemi
        if (isMoving && isGrounded && !isJumping)
        {
            float footstepInterval = isRunning ? 0.25f : 0.5f;

            if (Time.time - lastFootstepTime > footstepInterval)
            {
                lastFootstepTime = Time.time;
                PlayFootsteps();
            }
        }
    }

    private void PlayFootsteps()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distToGround + 0.2f))
        {
            PlaySurfaceSound(footstepsEvent, hit.collider.tag, "Switch");
        }
    }

    private void PlayJump()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distToGround + 0.2f))
        {
            PlaySurfaceSound(jumpEvent, hit.collider.tag, "Jump_switch");
        }

        // Ustaw status lotu
        isGrounded = false;
        isJumping = true;
        jumpTime = Time.time; 
    }

    private void PlayLanding()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distToGround + 0.2f))
        {
            PlaySurfaceSound(landEvent, hit.collider.tag, "Land_Switch");
        }
    }

    private void PlaySurfaceSound(EventReference eventRef, string surfaceTag, string paramName)
    {
        string surfaceLabel = "Stone";

        // Rozpoznaj tag podłoża
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
            case "Bed": // Dodany tag łóżka
                surfaceLabel = "Bed";
                break;
            default:
                surfaceLabel = "Stone";
                break;
        }

        // Bramka wyskoku: Domyślnie kamień, chyba że drewno lub łóżko
        if (paramName == "Jump_switch" && surfaceLabel != "Wood" && surfaceLabel != "Bed")
        {
            surfaceLabel = "Stone";
        }

        // Odpal FMOD
        var soundInstance = RuntimeManager.CreateInstance(eventRef);
        soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));
        soundInstance.setParameterByNameWithLabel(paramName, surfaceLabel);
        soundInstance.start();
        soundInstance.release();
        
        Debug.Log($"Puszczam dźwięk: {paramName} na podłożu: {surfaceLabel}");
    }

    private bool IsGrounded()
    {
        // Promień 0.2f do wykrycia ziemi
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.2f);
    }
}