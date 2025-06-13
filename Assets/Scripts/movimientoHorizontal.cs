using UnityEngine;

public class movimientoHorizontal : MonoBehaviour
{
    public float velocidad = 5f;

    void Update()
    {
        float movimiento = Input.GetAxisRaw("Horizontal"); // Flechas y A/D
        transform.position += new Vector3(movimiento, 0, 0) * velocidad * Time.deltaTime;
    }
}
