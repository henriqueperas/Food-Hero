using System;
using UnityEngine;

// Guarda quanta comida e quantos remédios a instituição tem.
// Existe um só na cena, e qualquer script pode acessar com Estoque.Instance.
public class Estoque : MonoBehaviour
{
    public static Estoque Instance;

    public int comida = 0;
    public int remedio = 0;

    // A interface (UI) pode escutar este evento para atualizar os números
    public event Action aoMudar;

    private void Awake()
    {
        Instance = this;
    }

    public void Adicionar(TipoPedido tipo, int quantidade)
    {
        if (tipo == TipoPedido.Comida)
        {
            comida += quantidade;
        }
        else
        {
            remedio += quantidade;
        }

        Avisar();
    }

    // Tenta tirar do estoque. Retorna false se não tiver o suficiente.
    public bool Consumir(TipoPedido tipo, int quantidade)
    {
        if (Quantidade(tipo) < quantidade)
        {
            return false;
        }

        if (tipo == TipoPedido.Comida)
        {
            comida -= quantidade;
        }
        else
        {
            remedio -= quantidade;
        }

        Avisar();
        return true;
    }

    public int Quantidade(TipoPedido tipo)
    {
        if (tipo == TipoPedido.Comida)
        {
            return comida;
        }
        return remedio;
    }

    private void Avisar()
    {
        Debug.Log("Estoque -> Comida: " + comida + " | Remédio: " + remedio);

        if (aoMudar != null)
        {
            aoMudar();
        }
    }
}