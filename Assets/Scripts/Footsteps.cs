using UnityEngine;
using FMODUnity;

// Zarządza krokami, skokami i lądowaniem
public class Footsteps : MonoBehaviour
{
    // Referencje eventów FMOD
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
            // NATYCHMIAST zmień stan, aby zablokować podwójny dźwięk
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
        string surfaceLabel = null;

        // Rozpoznawanie tagów i logi
        switch (surfaceTag)
        {
            case "Stone":
            case "Inside_stone":
            case "Outside":
                surfaceLabel = "Stone";
                Debug.Log("puszczam dzwiek stone");
                break;

            case "Wood":
            case "Inside_wood":
                surfaceLabel = "Wood";
                Debug.Log("puszczam dzwiek drewna");
                break;
        }

        if (surfaceLabel != null)
        {
            // Graj i zapomnij
            var soundInstance = RuntimeManager.CreateInstance(eventRef);
            soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));
            soundInstance.setParameterByNameWithLabel(paramName, surfaceLabel);
            soundInstance.start();
            soundInstance.release();
        }
    }

    // Sprawdza czy gracz dotyka ziemi promieniem
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.5f);
    }
}





//using UnityEngine;
//using FMODUnity;

///// <summary>
///// Zarządza odtwarzaniem dźwięków kroków, skoków i lądowania w zależności od powierzchni.
///// </summary>
//public class Footsteps : MonoBehaviour
//{
//    // FMOD - Instancje zdarzeń.
//    private FMOD.Studio.EventInstance footstepsSoundInstance;
//    private FMOD.Studio.EventInstance jumpSoundInstance;
//    private FMOD.Studio.EventInstance landSoundInstance;

//    // Publiczne referencje do zdarzeń FMOD.
//    public EventReference footstepsEvent;
//    public EventReference jumpEvent;
//    public EventReference landEvent;

//    // Usunięto: private Dictionary<string, string> surfaceTags;

//    private float lastFootstepTime = 0f;
//    private float distToGround;

//    [SerializeField]
//    private bool isGrounded = true;
//    [SerializeField]
//    private bool isJumping = false;

//    void Start()
//    {
//        distToGround = GetComponent<Collider>().bounds.extents.y;

//        // Usunięto: Inicjalizację słownika.
//    }

//    void Update()
//    {
//        // Sprawdza, czy gracz skacze, używając spacji.
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            PlayJump();
//        }
//    }

//    void FixedUpdate()
//    {
//        HandleFootsteps();
//    }

//    /// <summary>
//    /// Obsługuje logikę odtwarzania dźwięków kroków.
//    /// </summary>
//    private void HandleFootsteps()
//    {
//        // Sprawdza, czy gracz się porusza.
//        bool isMoving = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);
//        // Sprawdza, czy gracz biegnie.
//        bool isRunning = Input.GetKey(KeyCode.LeftShift);

//        if (isMoving && IsGrounded())
//        {
//            // Ustawia interwał na podstawie tego, czy gracz biegnie.
//            float footstepInterval = isRunning ? 0.25f : 0.5f;

//            if (Time.time - lastFootstepTime > footstepInterval)
//            {
//                lastFootstepTime = Time.time;
//                PlayFootsteps();
//            }
//        }
//    }

//    /// <summary>
//    /// Odtwarza dźwięk kroków w zależności od powierzchni.
//    /// </summary>
//    private void PlayFootsteps()
//    {
//        RaycastHit hit;
//        if (Physics.Raycast(transform.position, Vector3.down, out hit, distToGround + 0.5f))
//        {
//            string surfaceTag = hit.collider.tag;
//            PlaySurfaceSound(footstepsSoundInstance, footstepsEvent, surfaceTag);
//        }
//    }

//    /// <summary>
//    /// Odtwarza dźwięk skoku.
//    /// </summary>
//    private void PlayJump()
//    {
//        if (IsGrounded())
//        {
//            RaycastHit hit;
//            if (Physics.Raycast(transform.position, Vector3.down, out hit, distToGround + 0.5f))
//            {
//                string surfaceTag = hit.collider.tag;
//                PlaySurfaceSound(jumpSoundInstance, jumpEvent, surfaceTag);
//            }
//            isGrounded = false;
//            isJumping = true;
//        }
//    }

//    /// <summary>
//    /// Obsługuje dźwięk lądowania po skoku.
//    /// </summary>
//    private void OnCollisionEnter(Collision col)
//    {
//        if (!isGrounded && isJumping)
//        {
//            PlayLanding();
//        }
//    }

//    /// <summary>
//    /// Odtwarza dźwięk lądowania.
//    /// </summary>
//    private void PlayLanding()
//    {
//        RaycastHit hit;
//        if (Physics.Raycast(transform.position, Vector3.down, out hit, distToGround + 0.5f))
//        {
//            string surfaceTag = hit.collider.tag;
//            PlaySurfaceSound(landSoundInstance, landEvent, surfaceTag);
//        }
//        isGrounded = true;
//        isJumping = false;
//    }

//    /// <summary>
//    /// Ogólna metoda do odtwarzania dźwięku na podstawie tagu powierzchni.

//    /// </summary>
//    /// <param name="soundInstance">Instancja dźwięku FMOD.</param>
//    /// <param name="eventRef">Referencja do zdarzenia FMOD.</param>
//    /// <param name="surfaceTag">Tag powierzchni, na której znajduje się gracz.</param>
//    private void PlaySurfaceSound(FMOD.Studio.EventInstance soundInstance, EventReference eventRef, string surfaceTag)
//    {
//        // Zmienna przechowująca parametr FMOD. Domyślnie ustawiona na null/pusty string.
//        string surfaceParameter = null; 

//        // Instrukcja SWITCH do mapowania Tagu na Parametr FMOD.
//        switch (surfaceTag)
//        {
//            case "Stone":
//            case "Inside_stone":
//            case "Outside": // "Outside" również używa parametru "Stone"
//                surfaceParameter = "Stone";
//                break;

//            case "Wood":
//            case "Inside_wood":
//                surfaceParameter = "Wood";
//                break;

//        }

//        // Jeśli znaleziono pasujący parametr, odtwórz dźwięk.
//        if (surfaceParameter != null)
//        {
//            soundInstance = RuntimeManager.CreateInstance(eventRef);
//            soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));
//            // Ustawia parametr FMOD na podstawie ustalonej wartości.
//            soundInstance.setParameterByNameWithLabel("Footsteps_surface", surfaceParameter); 
//            soundInstance.start();
//            soundInstance.release();
//        }
//    }

//    /// <summary>
//    /// Sprawdza, czy gracz znajduje się na podłożu.
//    /// </summary>
//    bool IsGrounded()
//    {
//        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.5f);
//    }  
//}