using UnityEngine;

public class UFO : SpaceShip
{
    //Скорость передвижения
    public float Speed = 10;
    //Границы времени смены направления движения 
    public int MinTimeChangeMove, MaxTimeChangeMove;
    //Кол-во очков за уничтожение
    public int Points;
    //Границы частоты выстрела 
    public float MinTimeFire, MaxTimeFire;

    //Движение вправо или влево
    bool MoveInRight = true;
    float Timer = 0;
    //Время для смены движения
    float TimeChangeMove = 0;
    //Время для выстрела
    float TimerFire = 0.0f;
    AudioSource audioSource;
    Main main;

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main.GetComponent<Main>();
        audioSource = GetComponent<AudioSource>();
        //Вычисляем скорость передвижения для преодоления экрана за 10 сек
        Vector3 length = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.width, 0)) - Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Speed = length.z / 10;
        //Координаты появления
        float x = 0;
        float z = Random.Range(Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector3(Screen.height, 0, 0)).x);
        //Определяем направление движения
        MoveInRight = Random.Range(0, 10) > 5 ? true : false;
        while (true)
        {
            //Позиция на экране
            Vector3 PositionOnScreen = Camera.main.WorldToScreenPoint(new Vector3(x, 0, z));
            //Если объект за границами камеры, то передвигаем
            if (MoveInRight) x -= 1; else x += 1;
            if (PositionOnScreen.x < Screen.width) x -= 1;
            if (PositionOnScreen.x > 0) x += 1;
            if (PositionOnScreen.y > (Screen.height - Screen.height * 0.2)) z -= 10;
            if (PositionOnScreen.y < Screen.height * 0.2) z += 10;

            if ((PositionOnScreen.x > Screen.width * 0.9 || PositionOnScreen.x < Screen.width * 0.1) &&
                PositionOnScreen.y < (Screen.height - Screen.height * 0.2) && PositionOnScreen.y > Screen.height * 0.2)
                break;
        }
        //Задаем координаты в мире
        transform.position = new Vector3(x, 0, z);
        //Определяем время смены движения
        TimeChangeMove = Random.Range(MinTimeChangeMove, MaxTimeChangeMove);
        //Определяем время стрельбы
        TimerFire = Random.Range(MinTimeFire, MaxTimeFire);
    }

    // Update is called once per frame
    void Update()
    {
        if (main.Processing)
        {
            //Включаем звук
            audioSource.mute = false;
            //Обновляем таймер
            Timer += Time.deltaTime;
            //Перемещаем объект
            if (MoveInRight)
                transform.position = new Vector3(transform.position.x + Speed * Time.deltaTime, transform.position.y, transform.position.z);
            else
                transform.position = new Vector3(transform.position.x - Speed * Time.deltaTime, transform.position.y, transform.position.z);
            //Меняем направление движения при истечении времени
            if (Timer > TimeChangeMove)
            {
                MoveInRight = Random.Range(0, 10) > 5 ? true : false;
                TimeChangeMove += Random.Range(MinTimeChangeMove, MaxTimeChangeMove);
            }
            //Стреляем при истечении времени
            if (Timer > TimerFire)
            {
                GameObject bulletObject = PooledGameObject.Instance.GetBullet();
                bulletObject.transform.position = gameObject.transform.position;
                bulletObject.transform.LookAt(GameObject.Find(main.Rocket.name+"(Clone)").transform.position);
                
                bulletObject.GetComponent<Bullet>().SetType(1);
                TimerFire += Random.Range(MinTimeFire, MaxTimeFire);
            }
            //Выход за границы экрана
            //Вычисляем позиции относительно экрана
            Vector3 PositionOnScreen = Camera.main.WorldToScreenPoint(transform.position);
            //Вычисляем фактические коориданаты
            Vector3 PositionInWorld = transform.position;
            //Проверяем на выход и меняем координаты
            if (PositionOnScreen.x > Screen.width)
                PositionInWorld.x = -PositionInWorld.x + 1;
            if (PositionOnScreen.x < 0)
                PositionInWorld.x = -PositionInWorld.x - 1;
            if (PositionOnScreen.y > Screen.height)
                PositionInWorld.z = -PositionInWorld.z + 1;
            if (PositionOnScreen.y < 0)
                PositionInWorld.z = -PositionInWorld.z - 1;
            //Устанавливаем новую позицию
            transform.position = PositionInWorld;
        }
        //Отключаем звук
        else
            audioSource.mute = true;
    }

    public override void Destruction(Collider other)
    {
        //Если столкнулись с пулей
        if(other.gameObject.name.Contains("Bullet"))
            main.AddPoints(Points);
        //Обновляем время появления нового НЛО
        main.DestroyUFO();
        //Уничтожаем объект
        Destroy(gameObject);
    }
}
