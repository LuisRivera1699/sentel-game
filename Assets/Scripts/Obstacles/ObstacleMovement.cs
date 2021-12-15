using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Enum de los tipos de obstaculos: bomba o cohete.
public enum EnumObstacleType {
    Bomb, Rocket
}

// Clase ObstacleMovement para definir el movimiento de los obstaculos
public class ObstacleMovement : MonoBehaviour
{

    // rb: define el Rigidbody2D del obstáculo
    // speed: define la velocidad de movimiento horizontal del obstáculo
    // sinYPos: define la posicion y a iniciar del obstáculo en el caso de los cohetes
    // enumObstacleType: devine el tipo de obstáculo que es el objeto
    private Rigidbody2D rb;
    public float speed = 2;
    private float sinYPos;
    public EnumObstacleType enumObstacleType;

    void Start()
    {
        // Se instancia el rb.
        // sinYPos se instanica como un número flotante variable entre -3 y 3
        // Si el obstáculo es una bomba se instancia su posición inicial con la posición
        // Y variable entre -3 y 3 flotante
        rb = GetComponent<Rigidbody2D>();
        sinYPos = UnityEngine.Random.Range(-3.0f, 3.0f);
        if (enumObstacleType == EnumObstacleType.Bomb) {
            transform.position = new Vector3(transform.position.x, UnityEngine.Random.Range(-3.0f, 3.0f), transform.position.z);
        }
    }

    void Update()
    {
        // Si no se ha perdido y el juego no está pausado, realizar lógica de movimiento
        // del obstáculo. Si se ha perdido, se destruye el objeto RigidBody2D para evitar que siga el movimiento.
        // La velocidad horizontal del objeto se instancia con la variable speed.
        // Si el obstáculo es un cohete, su movimiento será siguiendo la función seno en base a su posición X. 
        if (!GameManager.Instance.stopped && !GameManager.Instance.lost) {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            if (enumObstacleType == EnumObstacleType.Rocket) {
                float lastX = transform.position.x;
                transform.position = new Vector3(transform.position.x, 1.5f*(float)Math.Sin(lastX)+sinYPos, transform.position.z);
            }
        } else {
            if (GameManager.Instance.lost) {
                Destroy(rb);
            }
        }
    }

    // Al momento de haber un evento trigger sobre el objeto:
    // Si choca contra un objeto con la etiqueta Respawn: Se ejecuta el método SpawnAndDestroy()
    // Si choca contra un objeto con la etiqueta Player: Se le resta una vida al Hero y se ejecuta el método SpawnAndDestroy()
    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.transform.tag == "Respawn") {
            Debug.Log("Hit!");
            SpawnAndDestroy();
        } else if (collider.transform.tag == "Player") {
            GameManager.Instance.RestLife();
            SpawnAndDestroy();
        }
    }

    // Método que llama al GameManager para que cree un nuevo obstáculo y luego destruye el obstáculo actual.
    void SpawnAndDestroy() {
        GameManager.Instance.SpawnObstacle();
        Destroy(gameObject);
    }
}
