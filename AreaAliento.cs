using UnityEngine;

public class AreaAliento : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Movement playerScript = collision.GetComponent<Movement>();
            if (playerScript != null && !playerScript.muerto)
            {
                Vector2 direccionDanio = (collision.transform.position - transform.position).normalized;
                playerScript.RecibeDanio(direccionDanio, 1);
            }
        }
    }
}