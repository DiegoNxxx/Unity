using UnityEngine;
using System.Collections;

public class EnemyFollowMunequito : MonoBehaviour
{
    [SerializeField] private GameObject areaAlientoGO;

    [Header("Animaci�n")]
    private Animator animator;

    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;

    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Recibiendo Da�o")]
    private bool recibiendoDanio;
    private float fuerzaRebote = 10f;
    public bool playerVivo;
    private bool muerto;
    public int vida_enemigo = 3;

    private bool mirandoDerecha = false;
    private float vida = 30f;

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

            animator.SetBool("die_munequito", muerto); // Animaci�n de muerte
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

            animator.SetTrigger("attack_munequito"); // Animaci�n de ataque
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
                animator.SetBool("die_munequito", true);
                movement = Vector2.zero;
                Destroy(gameObject, 0.5f);
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

    IEnumerator AtaquePeriodico()
    {
        while (!muerto)
        {
            yield return new WaitForSeconds(5f);

            if (!recibiendoDanio && playerVivo)
            {
                animator.SetBool("martini", true); // Activar animaci�n tipo cadena
                yield return new WaitForSeconds(1f);
                animator.SetBool("martini", false); // Desactivar luego de animar
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
