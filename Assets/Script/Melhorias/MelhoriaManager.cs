using System;
using UnityEngine;

public enum TipoMelhoria
{
    Voluntario,   // deixa uma estação mais rápida
    Parceria,     // doações mais frequentes e maiores
    Expansao,     // mais lugares na sala de espera
    Sede          // gera PP sozinho
}

// Uma melhoria à venda. Você configura cada uma no Inspector.
[Serializable]
public class Melhoria
{
    public string nome = "Nova melhoria";
    public TipoMelhoria tipo;
    public int custoBase = 50;
    public int nivelMaximo = 3;
    public Estacao estacaoAlvo;          // só usado quando o tipo é Voluntario

    [HideInInspector] public int nivel;  // quantas vezes já foi comprada
}

// Controla a compra das melhorias e aplica o efeito de cada uma.
public class MelhoriaManager : MonoBehaviour
{
    public Melhoria[] melhorias;
    public ClienteSpawner clienteSpawner;     // usado pela Expansao
    public float ppPassivoPorSede = 0.3f;     // PP por segundo de cada sede

    // Os botões escutam este evento para atualizar nome, nível e custo
    public event Action aoMudar;

    // O preço sobe 15% a cada nível comprado
    public int CustoAtual(int indice)
    {
        Melhoria m = melhorias[indice];
        return Mathf.RoundToInt(m.custoBase * Mathf.Pow(1.15f, m.nivel));
    }

    // Chamado quando o jogador clica no botão da melhoria
    public void Comprar(int indice)
    {
        Melhoria m = melhorias[indice];

        if (m.nivel >= m.nivelMaximo)
        {
            Debug.Log(m.nome + " já está no máximo.");
            return;
        }

        if (!GameManager.Instance.GastarPP(CustoAtual(indice)))
        {
            Debug.Log("PP insuficiente para " + m.nome);
            return;
        }

        m.nivel++;
        AplicarEfeito(m);

        if (aoMudar != null)
        {
            aoMudar();
        }
    }

    private void AplicarEfeito(Melhoria m)
    {
        switch (m.tipo)
        {
            case TipoMelhoria.Voluntario:
                m.estacaoAlvo.AdicionarVoluntario();
                break;

            case TipoMelhoria.Parceria:
                DoacaoManager.Instance.AplicarParceria();
                break;

            case TipoMelhoria.Expansao:
                clienteSpawner.AdicionarLugar();
                break;

            case TipoMelhoria.Sede:
                GameManager.Instance.AumentarPPPassivo(ppPassivoPorSede);
                break;
        }
    }
}