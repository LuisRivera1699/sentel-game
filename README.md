# Sentel - The Treats Adventure

En este documento se detallará una breve descripción del videojuego Sentel - The Treats Adventure, así como una descripción de como ejecutar el proyecto.


## Descripción

Sentel - The Treats Adventure es un No-end Game desarrollado en Unity con el lenguaje de programación C#.

Sentelito es un habitante de Entel Landia. Cada cierto tiempo, los Dioses Mensajeros bajan a su planeta para darles distintos retos, lo que les venga en mente. En este caso, le han retado a llevar un paquete de datos lo más lejos posible, obteniendo la mayor cantidad de puntos sin perder. Para ello, Sentelito tendrá que esquivar bombas y cohetes, agarrando los Entel Treats que le darán puntaje y estando atento a la aparición de un Paquete 5G que le dará una vida extra. ¿Podrá superar Sentelito esta misión?

## Ejecución del Proyecto

- Clonar repositorio de git ```git clone https://github.com/LuisRivera1699/sentel-game.git```
- Si no se tiene Unity Instalado, instalar en el siguiente link: https://unity.com/download
- Abrir Unity Hub
- Hacer click en "Add" o "Agregar", de acuerdo al idioma elegido
- Seleccionar la carpeta donde se clonó el repositorio y agregar
- Seleccionar el proyecto agregardo dentro de Unity Hub
- Esperar que se configure todo el proyecto
- En la ventana del Proyecto, dirigirse a la carpeta Scenes
- Hacer doble click sobre SplashScene
- Si se desea utilizar el Sentel-API desplegada, dirigirse al último paso
- Si se desea utilizar el Sentel-API de manera local luego de ejecutar el proyecto en Python, seguir con los pasos
- Dirigirse a la carpeta Scripts y abrir el script GameManager.cs
- En las lineas 280, 299, 315, 337 y 365, reemplazar ´´´https://xisksentel.loca.lt´´´ por ´´´http://127.0.0.1:5000´´´ para apuntar al servidor corriendo localmente.
- Ejecutar el juego apretando el botón de Play

## Generación de Build del juego para WebGL

Para generar el Build y desplegarlo en algún servicio WebGL que soporte Unity, seguir los pasos de la documentación oficial: https://docs.unity3d.com/Manual/webgl-building.html
 
## Equipo

### Nombre del equipo

Chis Gaming

### Integrante

- Luis Rivera Díaz
- +51 932 104 502
- luisriveradiaz1699@gmail.com
