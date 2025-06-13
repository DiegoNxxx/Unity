using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Image rellenoBarravida;
    private Movement playerMovement;
    private float vidaMaxima;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject jugador = GameObject.Find("Player");
        if (jugador != null)
        {
            playerMovement = jugador.GetComponent<Movement>();
            vidaMaxima = playerMovement.vida;
        }
    }

    // Update is called once per frame
    void Update()
    {
        rellenoBarravida.fillAmount = playerMovement.vida / vidaMaxima; 
    }
}
