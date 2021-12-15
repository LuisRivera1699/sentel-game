using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase HeroMovement para definir el movimiento del objeto Hero
public class HeroMovement : MonoBehaviour
{

    // mousePosition: variable para definir la posición vectorial del mouse
    Vector3 mousePosition;

    // movementSpeed: variable para definir la velocidad de movimiento del heroe
    public float movementSpeed = 4;

    void Start()
    {
        
    }

    void Update()
    {
        // Si no se ha perdido y el juego no está pausado, realizar lógica de movimiento
        // del heroe. El heroe se mueve verticalmente siguiendo la posición Y del mouse.
        if (!GameManager.Instance.stopped && !GameManager.Instance.lost) {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z;
            mousePosition.x = transform.position.x;
            transform.position = Vector3.MoveTowards(transform.position, mousePosition, movementSpeed*Time.deltaTime);
        }
    }
}
