using UnityEngine;
using System.Collections;


public class EnemyFollow : MonoBehaviour
{

    [SerializeField] private GameObject areaAlientoGO; // Draggea el hijo con el collider en el Inspector


    [Header("Animacion")]
    private Animator animator;

    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;

    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("RecibiendoDaño")]
    private bool recibiendoDanio;
    private float fuerzaRebote = 10f;
    public bool playerVivo;
    private bool muerto;
    public int vida_enemigo = 3;

    [Header("Animacion")]
    private bool atacando;
    private bool mirandoDerecha = false;


    [SerializeField] private float vida = 30f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerVivo = true;
        StartCoroutine(AtaquePeriodico());

    }

    void Update()
    {
        if (player != null)
        {
            Movement playerScript = player.GetComponent<Movement>();
            if (playerScript != null)
            {
                if (!playerScript.muerto && !muerto)
                {
                    Movimiento();
                }

                animator.SetBool("muerto", muerto);
            }


        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);

            Movement playerScript = collision.gameObject.GetComponent<Movement>();
            playerScript.RecibeDanio(direccionDanio, 1);
            playerVivo = !playerScript.muerto;

            atacando = true;
            animator.SetBool("atacando", true); // ✅ Activa el parámetro del Animator
        }
    }



    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            atacando = false;
            animator.SetBool("atacando", false); // ✅ Desactiva el parámetro del Animator
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Colisión con: " + collision.tag);  // 👈 Para verificar si la espada entra

        if (collision.CompareTag("Espada"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direccionDanio, 1);
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {

            vida_enemigo -= cantDanio;
            recibiendoDanio = true;

            if (vida_enemigo <= 0)
            {
                muerto = true;
                animator.SetBool("muerto", true);
                movement = Vector2.zero;
                Destroy(gameObject, .5f); // Da tiempo a la animación antes de destruir
            }
            else
            {

                // Aplicar fuerza de rebote
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                StartCoroutine(DesactivaDanio());
            }


        }

    }

    IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;
    }



    public void TomarDaño(float cantidadDaño)
    {
        vida -= cantidadDaño;
        Debug.Log("Enemigo recibió daño. Vida restante: " + vida);

        if (vida <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        // Aquí puedes poner una animación o sonido de muerte antes de destruir
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (!recibiendoDanio)
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

        }
    }


    private void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > detectionRadius)
        {
            Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
            movement = direction; // o new Vector2(direction.x, 0) si quieres solo mover en X

            // Girar si cambia la dirección horizontal
            if ((direction.x > 0 && !mirandoDerecha) || (direction.x < 0 && mirandoDerecha))
            {
                Girar();
            }

        }
        else
        {
            movement = Vector2.zero;
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
    IEnumerator AtaquePeriodico()
    {
        while (!muerto)
        {
            yield return new WaitForSeconds(5f); // Tiempo entre ataques

            if (!recibiendoDanio && playerVivo)
            {
                animator.SetBool("usandoAliento", true); // Activa la animación
                yield return new WaitForSeconds(1f); // Tiempo que dura la animación
                animator.SetBool("usandoAliento", false); // Desactiva después del tiempo
            }
        }
    }


    public void ActivarAliento()
    {
        areaAlientoGO.SetActive(true);
    }

    public void DesactivarAliento()
    {
        areaAlientoGO.SetActive(false);
    }


}
