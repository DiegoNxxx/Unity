using UnityEngine;

public class PausarJuego : MonoBehaviour

{
    public GameObject menuPause;
    public bool juegoPausado = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Reanudar()
    {
        menuPause.SetActive(false);
        Time.timeScale = 1;
        juegoPausado = false ;
    }

    public void Pausar()
    {
        menuPause.SetActive(true);
        Time.timeScale = 0;
        juegoPausado = true ;
    }

}
