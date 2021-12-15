using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum que define los tipos de Grupos de recompensas normales: Bar, Triangle, Diamond
public enum EnumRewardGroupType {
    Bar, Triangle, Diamond
}

// Clase que define el movimiento de los RewardGroups. Son los grupos de los Reward de tipo Normal
public class RewardGroupMovement : MonoBehaviour
{

    // rb: define el Rigidbody2D del objeto
    // enumRewardGroupType: define el tipo de RewardGroup
    // speed: define la velocidad de movimiento horizontal del RewardGroup
    // spawned: define si variable para controlar el spawneo de grupos al pasar un checkpoint
    private Rigidbody2D rb;
    public EnumRewardGroupType enumRewardGroupType;
    public float speed = 2;
    private bool spawned;

    void Start()
    {
        // Se instancia spawned como falso y el rb
        // Se ejecuta el método setFirstPosition de acuerdo al tipo de RewardGroup
        spawned = false;
        rb = GetComponent<Rigidbody2D>();
        switch (enumRewardGroupType) {
            case EnumRewardGroupType.Bar: 
                setFirstPosition(3f);
                break;
            case EnumRewardGroupType.Triangle:
                setFirstPosition(2.25f);
                break;
            case EnumRewardGroupType.Diamond:
                setFirstPosition(2f);
                break;
        }
    }

    void Update()
    {
        // Si no se ha perdido y el juego no está pausado, realizar lógica de movimiento
        // del RewardGroup. Si se pierde, se destruye el Rigidbody2D para evitar movimiento
        // Se instancia la velocidad del objeto con la variable speed.
        // Si la posición X del RewardGroup es -30 se destruye el objeto. Debido a que ya está fuera de 
        // pantalla
        // Se ejecuta el método spawnIfPassedX de acuedo al tipo de RewardGroup
        if (!GameManager.Instance.stopped && !GameManager.Instance.lost) {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            if (transform.position.x <= -30) {
                Destroy(gameObject);
            }

            switch (enumRewardGroupType) {
                case EnumRewardGroupType.Bar: 
                    spawnIfPassedX(5.24f);
                    break;
                case EnumRewardGroupType.Triangle:
                    spawnIfPassedX(5.76f);
                    break;
                case EnumRewardGroupType.Diamond:
                    spawnIfPassedX(5.76f);
                    break;
            }
        } else {
            if (GameManager.Instance.lost) {
                Destroy(rb);
            }
        }
    }

    // Método que define la posición inical Y del objeto con un valor
    // aleatorio entre los valores positivo y negativo del parámetro flotante rangeLimit
    void setFirstPosition(float rangeLimit) {
        transform.position = new Vector3(transform.position.x, UnityEngine.Random.Range(-rangeLimit, rangeLimit), transform.position.z);
    }

    // Método que pide al GameManager que ejecute su Método SpawnRewardGroup()
    // si es que el objeto ha pasado una posición x flotante, si es que el valor
    // de la variable spawned es falsa. Luego de llamar al método del GameManager
    // se setea la variable spawned como falsa para que este objeto no pueda llamar
    // el método del GameManager bastantes veces.
    void spawnIfPassedX(float x) {
        if (transform.position.x < x && !spawned) {
            GameManager.Instance.SpawnRewardGroup();
            spawned = true;
        }
    }
}
