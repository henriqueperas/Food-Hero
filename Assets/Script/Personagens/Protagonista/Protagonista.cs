using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// O protagonista anda até as estações que o jogador clicou,
// uma de cada vez, na ordem dos cliques.
// Entre uma sala e outra, ele sempre passa pelo corredor.
public class Protagonista : MonoBehaviour
{
    // Uma ação = um lugar para ir + o que fazer quando terminar (opcional)
    private class Acao
    {
        public Estacao estacao;
        public Action aoConcluir;
    }

    public float velocidade = 3f;

    [Header("Visual")]
    public Animator animator;                    // arraste o Animator do protagonista
    public SpriteRenderer corpo;                 // o sprite que vira para os lados
    public bool spriteOlhaParaDireita = true;    // desmarque se o desenho original olha para a esquerda

    private Queue<Acao> fila = new Queue<Acao>();
    private bool ocupado = false;

    // Onde ele está agora (vazio = está no corredor)
    private Estacao estacaoAtual = null;

    // Chamado quando o jogador clica numa estação ou num cliente
    public void AdicionarAcao(Estacao estacao, Action aoConcluir = null)
    {
        Acao acao = new Acao();
        acao.estacao = estacao;
        acao.aoConcluir = aoConcluir;
        fila.Enqueue(acao);

        // Se estava parado, começa a trabalhar
        if (!ocupado)
        {
            StartCoroutine(ExecutarFila());
        }
    }

    private IEnumerator ExecutarFila()
    {
        ocupado = true;

        while (fila.Count > 0)
        {
            Acao acao = fila.Dequeue();
            Estacao destino = acao.estacao;

            // Só precisa andar se não estiver já nessa sala
            if (estacaoAtual != destino)
            {
                // 1. Sai da sala atual até o corredor
                if (estacaoAtual != null)
                {
                    yield return StartCoroutine(AndarAte(estacaoAtual.pontoDoCorredor.position));
                }

                // 2. Anda pelo corredor até a frente da sala de destino
                yield return StartCoroutine(AndarAte(destino.pontoDoCorredor.position));

                // 3. Entra na sala e vai até o ponto de trabalho
                yield return StartCoroutine(AndarAte(destino.pontoDeUso.position));

                estacaoAtual = destino;
            }

            // 4. Trabalha na estação (espera o tempo de uso)
            DefinirAnimacao(false, true);
            yield return StartCoroutine(destino.Usar());
            DefinirAnimacao(false, false);

            // 5. Avisa quem pediu a ação (ex.: o cliente, para entregar o pedido)
            if (acao.aoConcluir != null)
            {
                acao.aoConcluir();
            }
        }

        ocupado = false;
    }

    private IEnumerator AndarAte(Vector3 alvo)
    {
        DefinirAnimacao(true, false);

        while (Vector2.Distance(transform.position, alvo) > 0.05f)
        {
            // Vira para o lado em que está indo
            float direcao = alvo.x - transform.position.x;
            if (Mathf.Abs(direcao) > 0.01f)
            {
                Virar(direcao > 0f);
            }

            transform.position = Vector2.MoveTowards(
                transform.position, alvo, velocidade * Time.deltaTime);
            yield return null; // espera o próximo frame
        }
    }

    private void Virar(bool paraDireita)
    {
        if (corpo != null)
        {
            // Inverte o sprite só quando o lado desejado é diferente do desenho original
            corpo.flipX = (paraDireita != spriteOlhaParaDireita);
        }
    }

    // Avisa o Animator em que estado o protagonista está
    private void DefinirAnimacao(bool andando, bool trabalhando)
    {
        if (animator != null)
        {
            animator.SetBool("Andando", andando);
            animator.SetBool("Trabalhando", trabalhando);
        }
    }
}