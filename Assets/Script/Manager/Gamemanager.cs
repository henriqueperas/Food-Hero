using System;
using UnityEngine;

// Guarda os Pontos de Popularidade (PP) do jogador.
// Existe um só na cena, e qualquer script pode acessar com GameManager.Instance.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int pp = 0;

    // Progresso total (usado para liberar os posts da história)
    public int clientesAtendidos = 0;
    public int ppTotalGanho = 0;

    // PP que entra sozinho a cada segundo (vem das sedes construídas)
    public float ppPorSegundo = 0f;
    private float acumulado = 0f;

    // A interface (UI) pode escutar este evento para atualizar o texto do PP
    public event Action<int> aoMudarPP;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (ppPorSegundo <= 0f) return;

        // Junta frações de PP até completar 1 ponto inteiro
        acumulado += ppPorSegundo * Time.deltaTime;
        if (acumulado >= 1f)
        {
            int inteiro = Mathf.FloorToInt(acumulado);
            acumulado -= inteiro;
            AdicionarPP(inteiro);
        }
    }

    public void AdicionarPP(int quantidade)
    {
        pp += quantidade;
        ppTotalGanho += quantidade;
        Avisar();
    }

    // Tenta gastar PP. Retorna false se o jogador não tiver o suficiente.
    public bool GastarPP(int quantidade)
    {
        if (pp < quantidade)
        {
            return false;
        }

        pp -= quantidade;
        Avisar();
        return true;
    }

    public void AumentarPPPassivo(float quantidade)
    {
        ppPorSegundo += quantidade;
    }

    private void Avisar()
    {
        if (aoMudarPP != null)
        {
            aoMudarPP(pp);
        }
    }
}