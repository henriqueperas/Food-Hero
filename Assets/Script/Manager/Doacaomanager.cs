using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Recebe doações de tempos em tempos e deixa os lotes esperando no PC.
// Quando o protagonista termina o trabalho no Armazenamento,
// um lote é separado e entra no estoque.
public class DoacaoManager : MonoBehaviour
{
    // Um lote de doação = tipo + quantidade + quem doou
    private class Lote
    {
        public TipoPedido tipo;
        public int quantidade;
        public string origem;
    }

    public static DoacaoManager Instance;

    public float intervalo = 15f;       // segundos entre uma doação e outra
    public int quantidadeMinima = 3;
    public int quantidadeMaxima = 6;
    public int maximoPendentes = 5;     // lotes esperando no PC

    private List<Lote> pendentes = new List<Lote>();

    // A interface (UI) pode escutar este evento para mostrar "lotes no PC"
    public event Action<int> aoMudarPendentes;

    private string[] origensComida = { "Mercado", "Feira livre" };
    private string[] origensRemedio = { "Farmácia" };

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(ReceberDoacoes());
    }

    private IEnumerator ReceberDoacoes()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalo);

            if (pendentes.Count < maximoPendentes)
            {
                CriarLote();
            }
        }
    }

    private void CriarLote()
    {
        Lote lote = new Lote();
        lote.quantidade = UnityEngine.Random.Range(quantidadeMinima, quantidadeMaxima + 1);

        // Sorteia comida ou remédio
        if (UnityEngine.Random.value < 0.5f)
        {
            lote.tipo = TipoPedido.Comida;
            lote.origem = origensComida[UnityEngine.Random.Range(0, origensComida.Length)];
        }
        else
        {
            lote.tipo = TipoPedido.Remedio;
            lote.origem = origensRemedio[UnityEngine.Random.Range(0, origensRemedio.Length)];
        }

        pendentes.Add(lote);
        Debug.Log("Nova doação de " + lote.origem + ": " + lote.quantidade + " de " + lote.tipo);
        AvisarPendentes();
    }

    // Ligue este método no evento "Ao Terminar" da estação Armazenamento
    public void SepararLote()
    {
        if (pendentes.Count == 0)
        {
            Debug.Log("Nenhuma doação para separar.");
            return;
        }

        Lote lote = pendentes[0];
        pendentes.RemoveAt(0);

        Estoque.Instance.Adicionar(lote.tipo, lote.quantidade);
        AvisarPendentes();
    }

    // Chamado pela melhoria "Parceria": doações mais frequentes e maiores
    public void AplicarParceria()
    {
        intervalo = Mathf.Max(5f, intervalo * 0.85f);
        quantidadeMaxima++;
    }

    private void AvisarPendentes()
    {
        if (aoMudarPendentes != null)
        {
            aoMudarPendentes(pendentes.Count);
        }
    }
}