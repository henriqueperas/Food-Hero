using TMPro;
using UnityEngine;

// Mostra na tela o PP, o estoque e as doações esperando no PC.
// Ele só ESCUTA os outros scripts: quando algo muda, o texto atualiza.
public class HUD : MonoBehaviour
{
    public TMP_Text textoPP;
    public TMP_Text textoComida;
    public TMP_Text textoRemedio;
    public TMP_Text textoDoacoes;

    private void Start()
    {
        // Pede para ser avisado quando algo mudar
        GameManager.Instance.aoMudarPP += AtualizarPP;
        Estoque.Instance.aoMudar += AtualizarEstoque;
        DoacaoManager.Instance.aoMudarPendentes += AtualizarDoacoes;

        // Mostra os valores iniciais
        AtualizarPP(GameManager.Instance.pp);
        AtualizarEstoque();
        AtualizarDoacoes(0);
    }

    private void OnDestroy()
    {
        // Para de escutar quando o HUD for destruído
        GameManager.Instance.aoMudarPP -= AtualizarPP;
        Estoque.Instance.aoMudar -= AtualizarEstoque;
        DoacaoManager.Instance.aoMudarPendentes -= AtualizarDoacoes;
    }

    private void AtualizarPP(int pp)
    {
        textoPP.text = "PP: " + pp;
    }

    private void AtualizarEstoque()
    {
        textoComida.text = "Comida: " + Estoque.Instance.comida;
        textoRemedio.text = "Remédio: " + Estoque.Instance.remedio;
    }

    private void AtualizarDoacoes(int quantidade)
    {
        textoDoacoes.text = "Doações: " + quantidade;
    }
}