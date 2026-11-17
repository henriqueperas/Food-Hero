using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Qualquer lugar onde o protagonista pode trabalhar:
// armazenamento, cozinha, enfermaria, salas de espera.
// Precisa de um Collider2D no objeto para o clique funcionar.
public class Estacao : MonoBehaviour
{
    public string nome = "Cozinha";
    public Protagonista protagonista;   // arraste no Inspector
    public Transform pontoDeUso;        // onde o protagonista fica em pé, dentro da sala
    public Transform pontoDoCorredor;   // ponto no corredor, bem em frente à porta da sala
    public float tempoDeUso = 2f;       // segundos de trabalho

    // Evento: o que acontece quando o trabalho termina
    // (configurado no Inspector, ex.: dar PP, gastar estoque)
    public UnityEvent aoTerminar;

    // Voluntários deixam o trabalho nesta estação mais rápido
    public int voluntarios = 0;
    private float tempoBase;

    private void Awake()
    {
        tempoBase = tempoDeUso;
    }

    // Cada voluntário adiciona +25% de velocidade
    public void AdicionarVoluntario()
    {
        voluntarios++;
        tempoDeUso = tempoBase / (1f + 0.25f * voluntarios);
    }

    // Unity chama isso sozinha quando o jogador clica no objeto
    private void OnMouseDown()
    {
        if (Time.timeScale == 0f) return;   // jogo pausado

        protagonista.AdicionarAcao(this);
    }

    // Chamado pelo protagonista quando ele chega aqui
    public IEnumerator Usar()
    {
        Debug.Log("Trabalhando em: " + nome);
        yield return new WaitForSeconds(tempoDeUso);
        aoTerminar.Invoke();
    }
}