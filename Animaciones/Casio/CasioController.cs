using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyFollowCasio : MonoBehaviour
{
    [SerializeField] private GameObject areaAlientoGO;

    [Header("Animación")]
    private Animator animator;

    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;

    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Recibiendo Daño")]
    private bool recibiendoDanio;
    private float fuerzaRebote = 10f;
    public bool playerVivo;
    private bool muerto;
    public int vida_enemigo = 3;

    private bool mirandoDerecha = false;
    private float vida = 30f;

    [SerializeField] private GameObject cartelGanaste;
    private bool juegoTerminado = false;

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
            if (playerScript != null && !playerScript.muerto && !muerto)
            {
                Movimiento();
            }

            animator.SetBool("die_Casio", muerto);
        }
    }

    private void FixedUpdate()
    {
        if (!recibiendoDanio)
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }
    }

    private void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius) 
        {
            Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
            movement = direction;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);

            Movement playerScript = collision.gameObject.GetComponent<Movement>();
            playerScript.RecibeDanio(direccionDanio, 1);
            playerVivo = !playerScript.muerto;

            animator.SetTrigger("attack_Casio");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
                animator.SetBool("die_Casio", true);
                movement = Vector2.zero;

                if (cartelGanaste != null)
                    cartelGanaste.SetActive(true);

                StartCoroutine(CambiarAEscenaMenu()); // <--- Iniciar la corutina
            }
            else
            {
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

    IEnumerator AtaquePeriodico()
    {
        while (!muerto)
        {
            yield return new WaitForSeconds(5f);

            if (!recibiendoDanio && playerVivo)
            {
                animator.SetBool("Casio_cadena", true);
                yield return new WaitForSeconds(1f);
                animator.SetBool("Casio_cadena", false);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }


    private IEnumerator CambiarAEscenaMenu()
    {
        yield return new WaitForSecondsRealtime(5f); // Usa tiempo real aunque esté pausado

        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        int escenaAnterior = escenaActual - 1;

        if (escenaAnterior >= 0)
        {
            Time.timeScale = 1f; // <--- Reanudar tiempo antes de cambiar de escena
            SceneManager.LoadScene(escenaAnterior);
        }
        else
        {
            Debug.LogWarning("No hay escena anterior configurada en Build Settings.");
        }
    }


}
