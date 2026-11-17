using System.Text;
using TMPro;
using UnityEngine;

// A janela de redes sociais: abre pelo ícone e mostra os posts já liberados.
public class PainelPosts : MonoBehaviour
{
    public NarrativaManager narrativa;
    public GameObject painel;            // a janela com os posts
    public TMP_Text textoPosts;          // texto (dentro do painel) onde os posts aparecem
    public GameObject indicadorNovo;     // bolinha no ícone quando há post novo

    private void Start()
    {
        narrativa.aoMudar += Atualizar;

        painel.SetActive(false);
        Atualizar();
    }

    private void OnDestroy()
    {
        narrativa.aoMudar -= Atualizar;
    }

    // Ligue este método no clique do botão do ícone (evento "On Click")
    public void AbrirOuFechar()
    {
        painel.SetActive(!painel.activeSelf);
        narrativa.MarcarComoLidos();   // já avisa e chama Atualizar
    }

    // Ligue este método no botão "X" de dentro do painel
    public void Fechar()
    {
        painel.SetActive(false);
        narrativa.MarcarComoLidos();   // já avisa e chama Atualizar
    }

    private void Atualizar()
    {
        // Bolinha de "novo" só aparece com o painel fechado
        indicadorNovo.SetActive(narrativa.novosPosts > 0 && !painel.activeSelf);

        // Monta o texto com os posts liberados, do mais recente para o mais antigo
        StringBuilder sb = new StringBuilder();

        for (int i = narrativa.posts.Length - 1; i >= 0; i--)
        {
            PostData post = narrativa.posts[i];
            if (post.desbloqueado)
            {
                sb.AppendLine("<b>" + post.autor + "</b>");
                sb.AppendLine(post.texto);
                sb.AppendLine();
            }
        }

        textoPosts.text = sb.ToString();
    }
}