using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    //public variables
    public bool ControlWithMouse = false;
    public bool Processing = false;
    public int MaxSpeedAsteroid = 20;
    public int MinSpeedAsteroid = 5;
    public GameObject Rocket;
    public GameObject UFO;
    public GameObject AsteroidBig;

    //private variables
    GameObject GamePanel;
    GameObject[] LifeImages = new GameObject[3];
    Text textScore;
    GameObject MenuPanel;
    Button ButtonContinue;
    AudioSource audioSource;
    int Score = 0;
    int CountLife = 3;
    int NumberOfAsteroids = 0;
    int NumberSpawnAsteroids = 2;
    float TimeSpawnAsteroids = 0;
    float TimeSpawnUFO = 0;
    float Timer = 0;


    // Start is called before the first frame update
    void Start()
    {
        GamePanel = GameObject.Find("GamePanel");
        MenuPanel = GameObject.Find("MenuPanel");
        ButtonContinue = GameObject.Find("ButtonContinue").GetComponent<Button>();
        LifeImages[0] = GameObject.Find("ImageLife1");
        LifeImages[1] = GameObject.Find("ImageLife2");
        LifeImages[2] = GameObject.Find("ImageLife3");
        textScore = GameObject.Find("TextScore").GetComponent<Text>();

        GamePanel.SetActive(false);
        ButtonContinue.interactable = false;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Processing = false;
            MenuPanel.SetActive(true);
        }
        if (Processing)
        {
            Timer += Time.deltaTime;
            if (TimeSpawnAsteroids != 0 && TimeSpawnAsteroids < Timer)
            {
                CreateAsteroids();
                TimeSpawnAsteroids = 0;
            }
            if (TimeSpawnUFO != 0 && TimeSpawnUFO < Timer)
            {
                Instantiate(UFO);
                TimeSpawnUFO = 0;
            }
        }
    }

    public void Continue()
    {
        Processing = true;
        MenuPanel.SetActive(false);
    }

    public void NewGame()
    {
        ButtonContinue.interactable = true;
        MenuPanel.SetActive(false);
        GamePanel.SetActive(true);

        GameObject[] Asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        foreach (GameObject asteroid in Asteroids)
            Destroy(asteroid);
        GameObject[] SS = GameObject.FindGameObjectsWithTag("SpaceShip");
        foreach (GameObject ss in SS)
            Destroy(ss);
        GameObject[] Bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in Bullets)
            bullet.GetComponent<Bullet>().gameObject.SetActive(false);

        Timer = 0;
        CountLife = 3;
        UpdateLifePanel();
        Score = 0;
        UpdateScore();
        Instantiate(Rocket);
        NumberOfAsteroids = 0;
        NumberSpawnAsteroids = 2;
        CreateAsteroids();
        TimeSpawnUFO = Random.Range(20, 40);
        Processing = true;

    }

    public void Control()
    {
        ControlWithMouse = !ControlWithMouse;
        if (ControlWithMouse)
            GameObject.Find("ButtonControl").GetComponentInChildren<Text>().text = "Управление: клавиатура + мышь";
        else
            GameObject.Find("ButtonControl").GetComponentInChildren<Text>().text = "Управление: клавиатура";
    }

    public void Exit()
    {
        Application.Quit();
    }

    void UpdateScore()
    {
       textScore.text = Score.ToString();
    }


    public void ShipDestruction()
    {
        if (CountLife > 0)
        {
            CountLife--;

        }
        Camera.main.GetComponent<Main>().UpdateLifePanel();
        if (CountLife != 0)
        {
            GameObject Rocket = Camera.main.GetComponent<Main>().Rocket;
            Instantiate(Rocket);
        }
    }

    void UpdateLifePanel()
    {
        for (int i = 0; i < LifeImages.Length; i++)
            if (i < CountLife)
                LifeImages[i].SetActive(true);
            else
                LifeImages[i].SetActive(false);

        if(CountLife == 0)
        {
            Processing = false;
            MenuPanel.SetActive(true);
            GamePanel.SetActive(false);
            ButtonContinue.interactable = false;
        }
    }

    public void AddPoints(int Points)
    {
        Score += Points;
        UpdateScore();
    }

    public void CreateAsteroids()
    {
        for (int i = 0; i < NumberSpawnAsteroids; i++)
        {
            //Задаем координаты мира
            float x = Random.Range(-50, 50);
            float z = Random.Range(-50, 50);
            while (true)
            {
                Vector3 PositionOnScreen = Camera.main.WorldToScreenPoint(new Vector3(x, 0, z));
                //Если объект за границами камеры, то передвигаем
                if (PositionOnScreen.x > Screen.width) x -= 10;
                if (PositionOnScreen.x < 0) x += 10;
                if (PositionOnScreen.y > Screen.height) z -= 10;
                if (PositionOnScreen.y < 0) z += 10;
                if (PositionOnScreen.x < Screen.width && PositionOnScreen.x > 0 &&
                    PositionOnScreen.y < Screen.height && PositionOnScreen.y > 0)
                    break;
            }
            //Задаем координаты в мире
            GameObject Asteroid = Instantiate(AsteroidBig);
            //Задаем координаты в мире
            Asteroid.transform.position = new Vector3(x, 0, z);
            //Задаем направление движения
            Asteroid.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
            Asteroid.GetComponent<Asteroid>().Speed = Random.Range(MinSpeedAsteroid, MaxSpeedAsteroid);
            ChangeNumberOfAsteroids(1);
        }
    }

    public void ChangeNumberOfAsteroids(int i)
    {
        NumberOfAsteroids += i;
        if (NumberOfAsteroids == 0)
        {
            NumberSpawnAsteroids++;
            TimeSpawnAsteroids = Timer + 2;
        }
    }

    public void DestroyUFO()
    {
        TimeSpawnUFO = Timer + Random.Range(20, 40);
    }

    public void SetAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
