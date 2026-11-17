using System.Collections;
using UnityEngine;

// Cria clientes sozinho na sala de espera (agora é uma sala só).
// Cada cliente sorteia se quer comida ou remédio.
public class ClienteSpawner : MonoBehaviour
{
    public Cliente prefabCliente;           // modelo do cliente (arraste do Project)

    public Protagonista protagonista;       // arraste da cena
    public Estacao salaDeEspera;            // a estação da sala de espera
    public Estacao cozinha;                 // onde se prepara comida
    public Estacao enfermaria;              // onde se prepara remédio

    public Transform[] posicoes;            // lugares onde os clientes ficam esperando
    public Transform pontoDeEntrada;        // de onde os clientes saem andando (ex.: a porta)

    public int lugaresIniciais = 3;         // quantos lugares estão liberados no começo
    public float intervalo = 6f;            // segundos entre um cliente e outro
    public float intervaloMinimo = 2.5f;    // o intervalo nunca fica menor que isso

    // Guarda quem está em cada lugar (vazio = lugar livre)
    private Cliente[] ocupantes;
    private int lugaresAtivos;

    private void Start()
    {
        ocupantes = new Cliente[posicoes.Length];
        lugaresAtivos = Mathf.Min(lugaresIniciais, posicoes.Length);
        StartCoroutine(GerarClientes());
    }

    private IEnumerator GerarClientes()
    {
        while (true)
        {
            yield return new WaitForSeconds(IntervaloAtual());

            int livre = PosicaoLivre();
            if (livre >= 0)
            {
                CriarCliente(livre);
            }
        }
    }

    // Quanto mais PP o jogador tem, mais rápido chegam clientes
    private float IntervaloAtual()
    {
        float reducao = GameManager.Instance.pp * 0.02f;
        return Mathf.Max(intervaloMinimo, intervalo - reducao);
    }

    // Procura um lugar sem cliente. Retorna -1 se a sala estiver cheia.
    private int PosicaoLivre()
    {
        for (int i = 0; i < lugaresAtivos; i++)
        {
            if (ocupantes[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    // Chamado pela melhoria "Expansão": libera mais um lugar na sala
    public void AdicionarLugar()
    {
        if (lugaresAtivos < posicoes.Length)
        {
            lugaresAtivos++;
        }
    }

    private void CriarCliente(int posicao)
    {
        Cliente novo = Instantiate(prefabCliente, pontoDeEntrada.position, Quaternion.identity);

        // Sorteia o que o cliente quer e já define onde será preparado
        if (Random.value < 0.5f)
        {
            novo.pedido = TipoPedido.Comida;
            novo.estacaoDePreparo = cozinha;
        }
        else
        {
            novo.pedido = TipoPedido.Remedio;
            novo.estacaoDePreparo = enfermaria;
        }

        novo.quantidade = Random.Range(1, 3);   // pede 1 ou 2 unidades
        novo.protagonista = protagonista;
        novo.salaDeEspera = salaDeEspera;
        novo.destino = posicoes[posicao].position;   // ele anda até o lugar dele

        ocupantes[posicao] = novo;
    }
}