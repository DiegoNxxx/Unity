using UnityEngine;

public class ControladorAtaque : MonoBehaviour
{
    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float radioGolpe;
    [SerializeField] private float dañoGolpe;
    private Animator animator;
    private float tiempoUltimoToque = 0f;
    private int contadorToques = 0;
    private float umbralCombo = .0000004f;
    private bool atacando;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {

        animator.SetBool("Atacando", atacando);

        if (Input.GetKeyDown(KeyCode.F))
        {
            Atacando();
            
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
           
            animator.SetTrigger("PowerFox");
        }
    }

 
    public void Atacando()
    {
        atacando = true;
    }

    public void Desactivaataque()
    {
        atacando = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
    }


}