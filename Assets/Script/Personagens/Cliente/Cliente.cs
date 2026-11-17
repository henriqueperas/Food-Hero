using UnityEngine;

public enum TipoPedido
{
    Comida,
    Remedio
}

// Um necessitado: chega andando, espera no seu lugar e é atendido ao clicar.
// Precisa de um Collider2D no objeto para o clique funcionar.
public class Cliente : MonoBehaviour
{
    public TipoPedido pedido;
    public int quantidade = 1;      // quantas unidades do pedido ele precisa
    public float paciencia = 30f;   // segundos até ele desistir
    public int ppBase = 10;         // PP de um atendimento "normal"

    public Protagonista protagonista;   // preenchido pelo spawner
    public Estacao estacaoDePreparo;    // preenchido pelo spawner
    public Estacao salaDeEspera;        // preenchido pelo spawner

    [Header("Chegada")]
    public Vector3 destino;             // o lugar onde ele vai esperar (o spawner define)
    public float velocidade = 2f;

    [Header("Visual")]
    public SpriteRenderer corpo;        // sprite do cliente
    public Sprite[] variacoesDeSprite;  // vários desenhos; um é sorteado
    public bool spriteOlhaParaDireita = true;
    public Transform barraPaciencia;    // barrinha que encolhe (já vem dentro do prefab)
    public GameObject iconeComida;      // ícone que aparece se ele quer comida
    public GameObject iconeRemedio;     // ícone que aparece se ele quer remédio

    private float pacienciaRestante;
    private bool chegou = false;
    private bool sendoAtendido = false;
    private Vector3 escalaInicialBarra;
    private SpriteRenderer corDaBarra;

    private void Start()
    {
        pacienciaRestante = paciencia;

        // Sorteia a aparência do cliente
        if (corpo != null && variacoesDeSprite.Length > 0)
        {
            corpo.sprite = variacoesDeSprite[Random.Range(0, variacoesDeSprite.Length)];
        }

        // Mostra o ícone do que o cliente quer
        if (iconeComida != null)
        {
            iconeComida.SetActive(pedido == TipoPedido.Comida);
        }
        if (iconeRemedio != null)
        {
            iconeRemedio.SetActive(pedido == TipoPedido.Remedio);
        }

        if (barraPaciencia != null)
        {
            escalaInicialBarra = barraPaciencia.localScale;
            corDaBarra = barraPaciencia.GetComponent<SpriteRenderer>();
        }

        AtualizarBarra();
    }

    private void Update()
    {
        // Enquanto está andando, a paciência ainda não diminui
        if (!chegou)
        {
            Andar();
            return;
        }

        // Depois que chegou, a paciência diminui até a entrega
        pacienciaRestante -= Time.deltaTime;
        if (pacienciaRestante < 0f)
        {
            pacienciaRestante = 0f;
        }

        AtualizarBarra();

        // Se acabou e ninguém começou a atender, ele vai embora
        if (pacienciaRestante <= 0f && !sendoAtendido)
        {
            Debug.Log("Cliente desistiu...");
            Destroy(gameObject);
        }
    }

    private void Andar()
    {
        // Vira para o lado em que está indo
        float direcao = destino.x - transform.position.x;
        if (corpo != null && Mathf.Abs(direcao) > 0.01f)
        {
            corpo.flipX = ((direcao > 0f) != spriteOlhaParaDireita);
        }

        transform.position = Vector2.MoveTowards(
            transform.position, destino, velocidade * Time.deltaTime);

        if (Vector2.Distance(transform.position, destino) <= 0.05f)
        {
            chegou = true;   // a partir daqui a paciência começa a cair
        }
    }

    private void AtualizarBarra()
    {
        if (barraPaciencia == null) return;

        float proporcao = pacienciaRestante / paciencia;

        // Encolhe a barra conforme a paciência acaba
        barraPaciencia.localScale = new Vector3(
            escalaInicialBarra.x * proporcao, escalaInicialBarra.y, escalaInicialBarra.z);

        // Verde quando cheia, vermelha quando está acabando
        if (corDaBarra != null)
        {
            corDaBarra.color = Color.Lerp(Color.red, Color.green, proporcao);
        }
    }

    // Unity chama isso quando o jogador clica no cliente
    private void OnMouseDown()
    {
        if (Time.timeScale == 0f) return;   // jogo pausado
        if (!chegou) return;                // ainda está andando
        if (sendoAtendido) return;          // evita clicar duas vezes

        // Só atende se tiver estoque (o pedido é descontado agora)
        if (!Estoque.Instance.Consumir(pedido, quantidade))
        {
            Debug.Log("Sem estoque de " + pedido + "!");
            return;
        }

        sendoAtendido = true;

        // 1. O protagonista vai preparar o pedido
        protagonista.AdicionarAcao(estacaoDePreparo);

        // 2. Depois volta até o cliente e entrega (aí chama Entregar)
        protagonista.AdicionarAcao(salaDeEspera, Entregar);
    }

    private void Entregar()
    {
        // Quanto mais paciência sobrou, mais PP: de 0.5x até 1.5x
        float multiplicador = 0.5f + (pacienciaRestante / paciencia);
        int ganho = Mathf.RoundToInt(ppBase * multiplicador);

        GameManager.Instance.clientesAtendidos++;
        GameManager.Instance.AdicionarPP(ganho);
        Destroy(gameObject);
    }
}