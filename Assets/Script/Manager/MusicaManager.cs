using UnityEngine;

// Toca uma música em loop durante o jogo e permite mudar o volume.
[RequireComponent(typeof(AudioSource))]
public class MusicaManager : MonoBehaviour
{
    public AudioClip musica;                         // arraste o arquivo de música
    [Range(0f, 1f)] public float volumeInicial = 0.5f;

    private AudioSource fonte;

    private void Awake()
    {
        fonte = GetComponent<AudioSource>();
        fonte.clip = musica;
        fonte.loop = true;               // repete para sempre
        fonte.volume = volumeInicial;
        fonte.Play();
    }

    // O slider do menu de pause chama este método
    public void DefinirVolume(float volume)
    {
        fonte.volume = volume;
    }

    public float VolumeAtual()
    {
        return fonte.volume;
    }
}