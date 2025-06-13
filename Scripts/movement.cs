using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb2D;


    [Header("Movimiento")]
    [SerializeField] private float movimientoHorizontal = 0f;
    [SerializeField] private float velocidadDeMovimiento;
    private Vector2 velocidad = Vector2.zero;
    private bool mirandoDerecha = true;

    [Header("Salto")]
    private bool isGrounded;
    public float jumpForce = 10f;

    [Header("Animacion")]
    private Animator animator;


    [Header("RecibiendoDaño")]
    private bool recibiendoDanio;
    private float fuerzaRebote = 10f;
    public int vida = 3;
    public bool muerto;
    private bool volandoAlMorir = false;
    public GameObject pantallaHasMuerto;



    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("jump", false);
        volandoAlMorir = false;
        pantallaHasMuerto.SetActive(false);

    }

    void Update()
    {
        if (!muerto)
        {
            movimientoHorizontal = Input.GetAxisRaw("Horizontal") * velocidadDeMovimiento;
            animator.SetFloat("Horizontal", Mathf.Abs(movimientoHorizontal));
            animator.SetBool("recibeDanio", recibiendoDanio);
            animator.SetBool("muerto", muerto);



            if (muerto && volandoAlMorir)
            {
                transform.position += Vector3.up * Time.deltaTime * 2f; // Velocidad hacia arriba
            }



            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && isGrounded)
            {
                Jump();
            }
        }




    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            recibiendoDanio = true;

            vida -= cantDanio;

            if (vida <= 0)
            {
                muerto = true;

                //  Esto desactiva el seguimiento de la cámara
                FindFirstObjectByType<CamaraController>().objetivo = null;


                rb2D.linearVelocity = Vector2.zero; // Detener movimiento
                animator.SetBool("muerto", true); // Activar animación de muerte

                volandoAlMorir = true;// se va hacia arriba


                //  Mostrar la imagen de "Has muerto"
                if (pantallaHasMuerto != null)
                    pantallaHasMuerto.SetActive(true);



                //  Detener toda física
                rb2D.linearVelocity = Vector2.zero;
                rb2D.gravityScale = 0f; //  Elimina la gravedad

                GetComponent<Collider2D>().enabled = false;
                this.enabled = false; // Desactiva el script para que no siga en Update

                //  Iniciar la corrutina para cambiar de escena
                StartCoroutine(CambiarAEscenaMenu());

                return; 
            }

            // Solo si sigue vivo
            animator.SetBool("recibeDanio", true);
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
            rb2D.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            Invoke("DesactivaDanio", 1f);
        }

    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
        animator.SetBool("recibeDanio", false); // <---- Muy importante
    }






    private void FixedUpdate()
    {
        Mover(movimientoHorizontal);
    }

    private void Mover(float mover)
    {
        Vector2 velocidadObjetivo = new Vector2(mover, rb2D.linearVelocity.y);
        rb2D.linearVelocity = Vector2.SmoothDamp(rb2D.linearVelocity, velocidadObjetivo, ref velocidad, 0.05f);

        if ((mover > 0 && !mirandoDerecha) || (mover < 0 && mirandoDerecha))
        {
            Girar();
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    void Jump()
    {
        rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
        animator.SetTrigger("jump"); // Esto puede seguir si tienes una transición con Trigger también
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("suelo"))
        {
            isGrounded = true;
            animator.SetBool("jump", false);
        }
    }

    private IEnumerator CambiarAEscenaMenu()
    {
        yield return new WaitForSeconds(5f);

        // Cargar la escena anterior según el índice
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        int escenaAnterior = escenaActual - 1;

        if (escenaAnterior >= 0)
        {
            SceneManager.LoadScene(escenaAnterior);
        }
        else
        {
            Debug.LogWarning("No hay escena anterior configurada en Build Settings.");
        }
    }


}