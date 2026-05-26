using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

// Zarządza głośnością VCA i kursorem dla menu.
public class VCA_Manager : MonoBehaviour
{
    private FMOD.Studio.VCA globalVCA;
    private FMOD.Studio.VCA musicVCA;
    private FMOD.Studio.VCA tavernVCA;
    private FMOD.Studio.VCA outsideVCA;
    private FMOD.Studio.VCA sfxVCA;

    [Header("Referencje do UI")]
    public Slider globalSlider;
    public Slider musicSlider;
    public Slider tavernSlider;
    public Slider outsideSlider;
    public Slider sfxSlider;

    // Przechowuje stan menu (włączone/wyłączone).
    private bool isMenuOpen = false;

    void Start()
    {
        // Pobierz VCAs.
        globalVCA = RuntimeManager.GetVCA("vca:/Global mute");
        musicVCA = RuntimeManager.GetVCA("vca:/Music mute");
        tavernVCA = RuntimeManager.GetVCA("vca:/Tavern mute");
        outsideVCA = RuntimeManager.GetVCA("vca:/Outside mute");
        sfxVCA = RuntimeManager.GetVCA("vca:/SFX");

        // Ustaw domyślne wartości w UI.
        if (globalSlider != null) globalSlider.value = 1f;
        if (musicSlider != null) musicSlider.value = 1f;
        if (tavernSlider != null) tavernSlider.value = 1f;
        if (outsideSlider != null) outsideSlider.value = 1f;
        if (sfxSlider != null) sfxSlider.value = 1f;

        // Wymuś 100% głośności w FMOD na starcie (naprawia problem z zapisanym wyciszeniem).
        globalVCA.setVolume(1f);
        musicVCA.setVolume(1f);
        tavernVCA.setVolume(1f);
        outsideVCA.setVolume(1f);
        sfxVCA.setVolume(1f);
    }

    void Update()
    {
        // Tymczasowy przycisk (np. M) do testowania interakcji z myszką.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenuCursor();
        }
    }

    // --- METODY DLA SLIDERÓW ---

    public void SetGlobalVolume(float volume) => globalVCA.setVolume(volume);
    public void SetMusicVolume(float volume) => musicVCA.setVolume(volume);
    public void SetTavernVolume(float volume) => tavernVCA.setVolume(volume);
    public void SetOutsideVolume(float volume) => outsideVCA.setVolume(volume);
    public void SetSFXVolume(float volume) => sfxVCA.setVolume(volume);

    // --- ZARZĄDZANIE KURSOREM ---

    // Przełącza widoczność kursora, aby móc klikać suwaki.
    private void ToggleMenuCursor()
    {
        isMenuOpen = !isMenuOpen;

        if (isMenuOpen)
        {
            // Pokaż i odblokuj kursor.
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // Ukryj i zablokuj kursor (powrót do gry).
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}