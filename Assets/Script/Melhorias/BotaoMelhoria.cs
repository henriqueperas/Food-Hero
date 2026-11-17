using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Liga um botão da tela a uma melhoria do MelhoriaManager.
// Mostra nome, nível e custo, e só deixa clicar se o jogador tiver PP.
public class BotaoMelhoria : MonoBehaviour
{
    public MelhoriaManager manager;
    public int indice;          // posição da melhoria na lista do manager (0, 1, 2...)
    public TMP_Text texto;

    private Button botao;

    private void Start()
    {
        botao = GetComponent<Button>();
        botao.onClick.AddListener(Clicou);

        // Atualiza quando comprar algo ou quando o PP mudar
        manager.aoMudar += Atualizar;
        GameManager.Instance.aoMudarPP += AoMudarPP;

        Atualizar();
    }

    private void OnDestroy()
    {
        manager.aoMudar -= Atualizar;
        GameManager.Instance.aoMudarPP -= AoMudarPP;
    }

    private void Clicou()
    {
        manager.Comprar(indice);
    }

    private void AoMudarPP(int pp)
    {
        Atualizar();
    }

    private void Atualizar()
    {
        Melhoria m = manager.melhorias[indice];

        if (m.nivel >= m.nivelMaximo)
        {
            texto.text = m.nome + "\nMÁXIMO";
            botao.interactable = false;
            return;
        }

        int custo = manager.CustoAtual(indice);
        texto.text = m.nome + " (" + m.nivel + "/" + m.nivelMaximo + ") " + custo + " PP";
        botao.interactable = GameManager.Instance.pp >= custo;
    }
}