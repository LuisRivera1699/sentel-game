using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Enum que define si el texto está arriba o abajo. Para escena inicial.
public enum EnumTextSplashType {
    up, down
}

// Clase que define el movimiento de los Text de la escena inicial.
public class TextMovement : MonoBehaviour
{

    // textSplashType: define el tipo de texto
    // rt: define el objeto RectTransform
    // inPosition: define una variable para saber si el texto está o no en posición
    public EnumTextSplashType textSplashType;
    private RectTransform rt;
    private bool inPosition;

    // timerToNextScene: define temporizador para la siguiente escena
    // waitToNextScene: define el tiempo máximo de espera para pasar a la siguiente escena
    private float timerToNextScene = 0f;
    private float waitToNextScene = 2f;

    void Start()
    {
        // Se define la variable inPosition a falsa y el rt
        inPosition = false;
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Si el texto está arriba, se desplaza el texto hacia abajo de 0.06 en 0.06 unidades
        // Si el texto está abajo, se desplaza el texto hacia arriba de 0.06 en 0.06 unidades
        // Si han pasado el 0, se actualiza inPosition a verdadero y ya no se mueven más
        switch (textSplashType) {
            case (EnumTextSplashType.up):
                if (rt.position.y > 0) {
                    rt.position = new Vector3(rt.position.x, rt.position.y - 0.06f, rt.position.z);
                } else {
                    inPosition = true;
                }
                break;

            case (EnumTextSplashType.down):
                if (rt.position.y < 0) {
                    rt.position = new Vector3(rt.position.x, rt.position.y + 0.06f, rt.position.z);
                } else {
                    inPosition = true;
                }
                break;
        }

        // Si inPosition es verdadero. Se asegura que los textos estén en la posición Y 0, actualizando su valor
        // Y se inicia el contador para ir a la segunda escena.
        if (inPosition) {
            rt.position = new Vector3(rt.position.x, 0f, rt.position.z);
            timerToNextScene += Time.deltaTime;
            if (timerToNextScene > waitToNextScene) {
                if (textSplashType == EnumTextSplashType.up) {
                    SceneManager.LoadScene(1);
                }
            }
        }
    }
}
