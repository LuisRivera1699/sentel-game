using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Net;
using System;
using System.IO;
using UnityEngine.Networking;


// Clase Score que servirá para mapear la respuesta del servicio
// de puntaje del usuario
[Serializable]
public class Score {
    public Dictionary<string, string> _id;
    public int bestScore;
    public int lastScore;
    public string phoneNumber;
    public bool found;
}

// Clase IsEntel que servirá para mapear la respuesta del servicio simulado
// que valida si el telefono ingresado por el usuario es Entel
[Serializable]
public class IsEntel {
    public bool isEntel;
}

// Clase GameManager que define el Singleton del contexto global del juego
public class GameManager : MonoBehaviour
{

    // Lógica de Singleton
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    private string baseURL = "http://127.0.0.1:5000"; // Cambiar a la ruta de despliegue del Sentel-API cuando se quiera desplegar el API en un servidor dedicado

    // spawnObjectsPosition: define el Transform de la posición de spawneo de objetos
    // obstacles: define el arreglo de obstáculos
    // rewardGroups: define el arreglo de recompensas de juego
    // reward5G: define el objeto de recompensa 5G
    public Transform spawnObjectsPosition;
    public GameObject[] obstacles;
    public GameObject[] rewardGroups;
    public GameObject reward5G;

    // timerToFirstObstacles: define el temporizador para spwanear los primeros obstáculos
    // waitToFirstObstacles: define el tiempo máximo de espera para los primeros obstáculos
    // firstObstaclesSent: variable que define si el primer obstáculo ya ha sido spawneado
    private float timerToFirstObstacles = 0f;
    private float waitToFirstObstacles = 5f;
    private bool firstObstaclesSent = false;

    // scoreData: define el objeto de la clase Score
    // score: variable para contar el puntaje del usuario
    // lifes: variable para contar las vidas del usuario
    public Score scoreData;
    public int score;
    public int lifes;

    // scoreText: define el objeto Text del puntaje del usuario
    // lifeText: define el objeto Text de las vidas del usuario
    public Text scoreText;
    public Text lifeText;

    // lost: variable booleana para saber si se ha perdido o no
    // stopped: variable booleana para saber si el juego esta pausado o no
    // phoneNumber: variable para almacenar en memoria el número de teléfono ingresado
    public bool lost;
    public bool stopped;
    private string phoneNumber;

    // Objetos Button y Canvas de los diálogos de inicio del juego
    // phoneText: define el objeto Text del InputField
    public Canvas firstDialog;
    public Button firstButton;
    public Canvas secondDialog;
    public Button secondButton;
    public Canvas thirdDialog;
    public Button thirdButton;
    public Canvas fourthDialog;
    public Button fourthButton;
    public Canvas fifthDialog;
    public Button fifthButton;
    public Text phoneText;

    // Objeto Canvas y Button del diálogo de fin de juego
    // congratsText: define el objeto Text para saludar felicitar al usuario al final del juego
    // obtainedScore: define el objeto Text para el puntaje final del usuario
    // bestScore: define el objeto Text para el mejor puntaje del usuario
    // currentRank: define el objeto Text para la posición en el tablero de Ranking de usuarios
    // scores: define el arreglo de objetos Text para los mejores 5 puntajes
    public Canvas lostDialog;
    public Button playAgain;
    public Text congratsText;
    public Text obtainedScore;
    public Text bestScore;
    public Text currentRank;
    public Text[] scores;

    // Objetos AudioSource para los sonidos del juego
    public AudioSource treat;
    public AudioSource damage;
    public AudioSource lose;
    public AudioSource game;

    // Lógica de Singleton
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    void Start()
    {
        // Se inicia el score en 0, las vidas en 1
        // lost se setea en false y stopped en true para iniciar el juego pausado
        // Se ejecutan los métodos SpawnRewardGroup, SetTextScore, SetTextLifes,
        // OpenCanvas enviando el firstDialog Canvas y SetButtonBehaviour
        score = 0;
        lifes = 1;
        SpawnRewardGroup();
        SetTextScore();
        SetTextLifes();
        lost = false;
        stopped = true;
        OpenCanvas(firstDialog);
        SetButtonBehaviour();
    }

    void Update()
    {
        // Si no se ha perdido o el juego no está pausado
        // Se ejecuta el método Spawn5G si un valor aleatorio entre 0 y 125000 da como resultado 4
        // Si firstObstaclesSent es falso, se ejecuta la lógica para iniciar el temporizador 
        // para soltar el primer obstaculo
        // Si timerToFirstObstacles es mayor a waitToFirstObstacles, se ejecuta la función SpawnObstacle
        // se setea 2 veces firstObstaclesSent a false para diferenciar la aleatoriedad de la lógica interna
        // de SpawnObstacle, se ejecuta de nuevo el método y se setea finalmente firstObstaclesSent a true
        if (!stopped && !lost) {
            if (UnityEngine.Random.Range(0,125000) == 4) {
                Spawn5G();
            }

            if (!firstObstaclesSent) {
                timerToFirstObstacles += Time.deltaTime;
                if (timerToFirstObstacles > waitToFirstObstacles) {
                    SpawnObstacle();
                    firstObstaclesSent = false;
                    firstObstaclesSent = false;
                    SpawnObstacle();
                    firstObstaclesSent = true;
                }
            }
        }
    }

    // Método para incrementar el puntaje en uno
    // Ejecutar el sonido de treat y ejecutar el método
    // SetTextScore
    public void UpdateScore() {
        score++;
        treat.PlayOneShot(treat.clip);
        Debug.Log(score);
        SetTextScore();
    }

    // Método para restar una vida
    // Ejecutar el sonido de damage, se resta en uno
    // la variable lifes, se ejecuta el método SetTextLifes
    // Si lifes llega a ser 0, se ejecuta el método Stop
    // se ejecuta el sonido de Fin de juego, la variable lost se setea
    // a true para saber que se perdio, se setea el texto de obtainedScore para
    // pintar el puntaje obtenido
    // Si la variable found de scoreData es true, se setea la variable bestScore
    // comparando el bestScore obtenido por el fetching y el puntaje obtenido en el juego
    // finalmente se setea el lastScore al puntaje obtenido.
    // Si found es false, significa que no existen registros de puntaje del celular ingresado
    // por ende el lastScore y el bestScore se setean al puntaje obtenido
    // Se pinta el texto de bestScore con el bestScore de scoreData
    // Se inicia la corrutina con el método CreateOrUpdateScore
    // Se ejecuta el método OpenCanvas con el Canvas lostDialog
    public void RestLife() {
        damage.PlayOneShot(damage.clip);
        lifes--;
        SetTextLifes();
        if (lifes == 0) {
            game.Stop();
            lose.Play();
            lost = true;
            obtainedScore.text = $"{score}";
            if (scoreData.found) {
                if (score > scoreData.bestScore) {
                    scoreData.bestScore = score;
                }
                scoreData.lastScore = score;
            } else {
                scoreData.lastScore = score;
                scoreData.bestScore = score;
            }
            bestScore.text = $"{scoreData.bestScore}";
            StartCoroutine(CreateOrUpdateScore());
            OpenCanvas(lostDialog);
        }
    }

    // Método para añadir una vida
    // Ejecutar el sonido de obtención de punto
    // Ejecutar el método SetTextLifes
    public void AddLife() {
        lifes++;
        treat.PlayOneShot(treat.clip);
        SetTextLifes();
    }

    // Método para instanciar un objeto Obstacle aleatorio del arreglo obstacles en 
    // la posición spawnObjectsPosition.position
    public void SpawnObstacle() {
        Instantiate(obstacles[UnityEngine.Random.Range(0, obstacles.Length)], 
        spawnObjectsPosition.position, Quaternion.identity);
    }

    // Método para instanciar un objeto RewardGroup aleatorio del arreglo rewardGroups en 
    // la posición spawnObjectsPosition.position + X 3
    public void SpawnRewardGroup() {
        Instantiate(rewardGroups[UnityEngine.Random.Range(0, rewardGroups.Length)], 
        spawnObjectsPosition.position + new Vector3(3f, 0f, 0f), Quaternion.identity);
    }

    // Método para instanciar un paquete 5G en la posición spawnObjectsPosition
    public void Spawn5G() {
        Instantiate(reward5G, spawnObjectsPosition.position, Quaternion.identity);
    }

    // Método para pintar el puntaje que va obteniendo el usuario
    void SetTextScore() {
        scoreText.text = $"Score: {score}";
    }

    // Método para pintar las vidas del usuario
    void SetTextLifes() {
        lifeText.text = $"Lifes: {lifes}";
    }

    // Método para desactivar un Canvas enviado como parámetro
    void CloseCanvas(Canvas canvas) {
        canvas.gameObject.SetActive(false);
    }

    // Método para activar un Canvas enviado como parámetro
    void OpenCanvas(Canvas canvas) {
        canvas.gameObject.SetActive(true);
    }

    // Método para agregar los onClickListeners de los botones
    // Del firstButton al fourthButton se cerrarán los Canvas abiertos y se abrirá el siguiente Canvas
    // Para el fifthButton si se ingresa el número de teléfono al InputText, se setea la variable phoneNumber
    // al número de teléfono ingresado, se setea el texto de congratsText y se inicia la Corrutina con el
    // método CheckIfPhoneIsEntel
    // Para el playAgain Button, se vuelve a cargar la escena inicial
    void SetButtonBehaviour() {
        firstButton.onClick.AddListener(() => {CloseCanvas(firstDialog); OpenCanvas(secondDialog);});
        secondButton.onClick.AddListener(() => {CloseCanvas(secondDialog); OpenCanvas(thirdDialog);});
        thirdButton.onClick.AddListener(() => {CloseCanvas(thirdDialog); OpenCanvas(fourthDialog);});
        fourthButton.onClick.AddListener(() => {CloseCanvas(fourthDialog); OpenCanvas(fifthDialog);});
        fifthButton.onClick.AddListener(() => {
            if (phoneText.text != "") {
                phoneNumber = phoneText.text;
                congratsText.text = $"Felicitaciones {phoneNumber}";
                StartCoroutine(CheckIfPhoneIsEntel());
            }
        });
        playAgain.onClick.AddListener(() => {SceneManager.LoadScene(0);});
    }

    // Método para hacer el fetch a la ruta /scores del API Sentel Desarrollado 
    // para obtener los 5 mejores puntajes de los usuarios y pintarlos en los
    // Text del arreglo scores
    IEnumerator GetScores() {
        UnityWebRequest www = UnityWebRequest.Get($"{baseURL}/scores");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        } else {
            Debug.Log(www.downloadHandler.text);
            string woc = www.downloadHandler.text.Replace("[", string.Empty).Replace("]", string.Empty);
            string[] woc_splitted = woc.Split(',');
            for (var i = 0; i < woc_splitted.Length; i++) {
                scores[i].text = woc_splitted[i];
            }
        }
    }

    // Método para hacer el fetch a la ruta /scores/{phoneNumber}/rank del API Sentel Desarrollado 
    // para obtener la posición en el ranking de puntajes del celular ingresado y setear
    // texto de currentRank
    IEnumerator GetPhoneRank() {
        UnityWebRequest www = UnityWebRequest.Get($"{baseURL}/scores/{phoneNumber}/rank");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        } else {
            Debug.Log(www.downloadHandler.text);
            currentRank.text = $"{www.downloadHandler.text}º";
        }
    }

    // Método para hacer el fetch a la ruta /phones/{phoneNumber}/entel del API Sentel Desarrollado 
    // para validar si el número de celular ingresado es de entel
    // Si el servicio responde que sí es de Entel, se setean las vidas a 5 y se ejecuta el método SetTextLifes
    // Finalmente se inicia la Corrutina con el método GetPhoneScore
    IEnumerator CheckIfPhoneIsEntel() {
        UnityWebRequest www = UnityWebRequest.Get($"{baseURL}/phones/{phoneNumber}/entel");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        } else {
            Debug.Log(www.downloadHandler.text);
            IsEntel isEntel = JsonUtility.FromJson<IsEntel>(www.downloadHandler.text);
            if (isEntel.isEntel == true) {
                lifes = 5;
                SetTextLifes();
            }
            StartCoroutine(GetPhoneScore());
        }
    }

    // Método para hacer el fetch a la ruta /scores/{phoneNumber} del API Sentel Desarrollado 
    // para obtener el registro de puntaje del celular ingresado si es que tiene alguno
    // Se parsea el json de respuesta con el objeto scoreData de la clase Score
    // Se ejecuta el método CloseCanvas con el Canvas fifthDialog y se setea la variable stopped a 
    // false para iniciar el juego
    IEnumerator GetPhoneScore() {
        UnityWebRequest www = UnityWebRequest.Get($"{baseURL}/scores/{phoneNumber}");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        } else {
            Debug.Log(www.downloadHandler.text);
            scoreData = JsonUtility.FromJson<Score>(www.downloadHandler.text);
            if (scoreData.found) {
                Debug.Log("Si tiene");
            } else {
                Debug.Log("No tiene");
            }
            CloseCanvas(fifthDialog); 
            stopped=false;
        }
    }

    // Método para hacer un post a la ruta /scores del API Sentel Desarrollado 
    // para crear o actualizar el registro de puntajes del celular ingresado.
    // Se crea si no existe y se actualiza si ya existe.
    // Se le envian como parámetros las variables phoneNumber, scoreData.lastScore y scoreData.bestScore
    // Se inician las corrutinas con los métodos GetPhoneRank y GetScores
    IEnumerator CreateOrUpdateScore() {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection($"phoneNumber={phoneNumber}&lastScore={scoreData.lastScore}&bestScore={scoreData.bestScore}"));

        UnityWebRequest post = UnityWebRequest.Post($"{baseURL}/scores", formData);
        yield return post.SendWebRequest();
        if (post.result != UnityWebRequest.Result.Success) {
            Debug.Log(post.error);
        } else {
            StartCoroutine(GetPhoneRank());
            StartCoroutine(GetScores());
            Debug.Log("Updated!");
        }
    }
}


