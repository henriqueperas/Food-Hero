using System;
using UnityEngine;

public enum CondicaoPost
{
    ClientesAtendidos,   // libera depois de X clientes atendidos
    PPTotal              // libera depois de X PP ganhos no total
}

// Um post de rede social que conta a história. Configurado no Inspector.
[Serializable]
public class PostData
{
    public string autor = "@usuario";
    [TextArea] public string texto;
    public CondicaoPost condicao;
    public int valor;                        // quantidade necessária para liberar

    [HideInInspector] public bool desbloqueado;
}

// Guarda os posts e libera cada um quando o jogador atinge a meta dele.
public class NarrativaManager : MonoBehaviour
{
    public PostData[] posts;

    public int novosPosts = 0;   // posts liberados que o jogador ainda não viu

    // O painel escuta este evento para atualizar a lista e o indicador de "novo"
    public event Action aoMudar;

    private void Start()
    {
        // O PP muda a cada atendimento, então é um bom momento para conferir as metas
        GameManager.Instance.aoMudarPP += Verificar;
        Verificar(0);
    }

    private void OnDestroy()
    {
        GameManager.Instance.aoMudarPP -= Verificar;
    }

    private void Verificar(int pp)
    {
        bool liberouAlgum = false;

        foreach (PostData post in posts)
        {
            if (!post.desbloqueado && AtingiuMeta(post))
            {
                post.desbloqueado = true;
                novosPosts++;
                liberouAlgum = true;
            }
        }

        if (liberouAlgum && aoMudar != null)
        {
            aoMudar();
        }
    }

    private bool AtingiuMeta(PostData post)
    {
        if (post.condicao == CondicaoPost.ClientesAtendidos)
        {
            return GameManager.Instance.clientesAtendidos >= post.valor;
        }
        return GameManager.Instance.ppTotalGanho >= post.valor;
    }

    // Chamado quando o jogador abre ou fecha o painel
    public void MarcarComoLidos()
    {
        novosPosts = 0;

        if (aoMudar != null)
        {
            aoMudar();
        }
    }
}