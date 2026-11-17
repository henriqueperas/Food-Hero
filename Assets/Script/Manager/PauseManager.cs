using UnityEngine;
using UnityEngine.UI;

// Pausa o jogo com a tecla P. O menu tem: voltar, sair e volume da música.
// Importante: coloque este script num objeto que NUNCA é desligado
// (não coloque no próprio painel de pause).
public class PauseManager : MonoBehaviour
{
    public GameObject painelPause;      // o menu de pause
    public Slider sliderVolume;         // slider de 0 a 1 (dentro do painel)
    public MusicaManager musica;

    private bool pausado = false;

    private void Start()
    {
        Time.timeScale = 1f;            // garante que o jogo começa rodando
        painelPause.SetActive(false);

        // O slider começa no volume atual e controla a música
        sliderVolume.value = musica.VolumeAtual();
        sliderVolume.onValueChanged.AddListener(musica.DefinirVolume);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AlternarPause();
        }
    }

    public void AlternarPause()
    {
        pausado = !pausado;
        painelPause.SetActive(pausado);

        // timeScale 0 congela movimento, tempo, paciência, doações... tudo
        Time.timeScale = pausado ? 0f : 1f;
    }

    // Ligue no botão "Voltar"
    public void Continuar()
    {
        if (pausado)
        {
            AlternarPause();
        }
    }

    // Ligue no botão "Sair"
    public void Sair()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // no Editor, para o Play
#else
        Application.Quit();                                // no jogo final, fecha o programa
#endif
    }
}