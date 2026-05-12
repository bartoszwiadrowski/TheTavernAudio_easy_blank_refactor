using UnityEngine;
using FMODUnity;
using FMOD.Studio;

// Zarządza głośnością ścieżek audio przez FMOD VCAs.
public class VCA_Manager : MonoBehaviour
{
    private FMOD.Studio.VCA globalVCA;
    private FMOD.Studio.VCA musicVCA;
    private FMOD.Studio.VCA tavernVCA;
    private FMOD.Studio.VCA outsideVCA;

    [SerializeField] private bool globalMuteActive = true;
    [SerializeField] private bool musicMuteActive = false;
    [SerializeField] private bool tavernMuteActive = false;
    [SerializeField] private bool outsideMuteActive = false;

    void Start()
    {
        // Pobierz VCAs. Ścieżki muszą zgadzać się z nazwami w FMOD.
        globalVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Global mute");
        musicVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Music mute");
        tavernVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Tavern mute");
        outsideVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Outside mute");

        // Ustaw początkową głośność globalną.
        globalVCA.setVolume(DecibelToLinear(globalMuteActive ? -100f : 0f));
    }

    // --- METODY PUBLICZNE DLA PRZYCISKÓW UI ---

    public void ToggleGlobalMuteUI() => ToggleMute(globalVCA, ref globalMuteActive);
    public void ToggleMusicMuteUI() => ToggleMute(musicVCA, ref musicMuteActive);
    public void ToggleTavernMuteUI() => ToggleMute(tavernVCA, ref tavernMuteActive);
    public void ToggleOutsideMuteUI() => ToggleMute(outsideVCA, ref outsideMuteActive);

    // ------------------------------------------

    void Update()
    {
        // Klawisze zachowane do szybkiego testowania.
        if (Input.GetKeyDown(KeyCode.U)) ToggleGlobalMuteUI();
        if (Input.GetKeyDown(KeyCode.I)) ToggleMusicMuteUI();
        if (Input.GetKeyDown(KeyCode.O)) ToggleTavernMuteUI();
        if (Input.GetKeyDown(KeyCode.P)) ToggleOutsideMuteUI();
    }

    // Odwraca stan wyciszenia i ustawia głośność.
    private void ToggleMute(FMOD.Studio.VCA vca, ref bool muteFlag)
    {
        muteFlag = !muteFlag;
        float volume = muteFlag ? DecibelToLinear(-100f) : DecibelToLinear(0f);
        vca.setVolume(volume);
    }

    // Konwertuje dB na liniową skalę.
    private float DecibelToLinear(float dB)
    {
        return Mathf.Pow(10.0f, dB / 20f);
    }
}