using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum para definir los tipos de recompensa del juego: Normales y Paquete 5G
public enum EnumRewardType {
    Normal, _5G
}

// Clase RewardBehaviour para definir el comportamiento de las recompensas del juego
public class RewardBehaviour : MonoBehaviour
{

    // rewardType: define el tipo de recompensa
    // rb: define el objeto Rigidbody2D
    // speed: define la velocidad de movimiento de la recompensa. Solo para los _5G
    public EnumRewardType rewardType;
    private Rigidbody2D rb;
    public float speed;

    void Start()
    {
        // Si la recompensa es de tipo _5G
        // Se instancia el rb y se instancia la posición Y en un valor aleatorio entre -3 y 3 flotante
        if (rewardType == EnumRewardType._5G) {
            rb = GetComponent<Rigidbody2D>();
            transform.position = new Vector3(transform.position.x, Random.Range(-3.0f,3.0f), transform.position.z);
        }
    }

    void Update()
    {
        // Si no se ha perdido y el juego no está pausado, realizar lógica de movimiento
        // de la recompensa _5G con la variable speed
        if (!GameManager.Instance.stopped && !GameManager.Instance.lost) {
            if (rewardType == EnumRewardType._5G) {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
            }
        }
    }

    // Al momento de haber un evento trigger sobre el objeto:
    // Si choca contra un objeto con la etiqueta Respawn: Se destruye el objeto.
    // Si choca contra un objeto con la etiqueta Player: si es una recompensa de tipo _5G se
    // le suma una vida al jugador, si no, no. Se actualiza el marcador (+1) y se destruye el 
    // objeto.
    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.transform.tag == "Respawn") {
            Destroy(gameObject);
        } else if (collider.transform.tag == "Player") {
            Debug.Log("Un punto mas");
            if (rewardType == EnumRewardType._5G) {
                GameManager.Instance.AddLife();
            }
            GameManager.Instance.UpdateScore();
            Destroy(gameObject);
        }
    }
}
