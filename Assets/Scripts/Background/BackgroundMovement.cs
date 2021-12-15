using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase BackgroundMovement para definir el movimiento del objeto Background
public class BackgroundMovement : MonoBehaviour
{

    // scrollSpeed: define la velocidad de movimiento hacia la izquierda del background.
    public float scrollSpeed = -5f;

    // startPosition: define la posición inicial del background
    private Vector3 startPosition;

    // Al iniciarce el objeto, startPosition toma el valor de la posición del objeto inicial.
    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Si no se ha perdido y el juego no está pausado, realizar lógica de movimiento
        // del background.
        if (!GameManager.Instance.stopped && !GameManager.Instance.lost) {
            float newPos = Mathf.Repeat(scrollSpeed*Time.time, 52.49f);
            transform.position = startPosition + (Vector3.right * newPos);
        }
    }
}
