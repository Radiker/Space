using UnityEngine;

public class Rocket : SpaceShip
{
    //Максимальная скорость
    public float SpeedMax = 10;
    //Скорости поворота
    public float SpeedRotationWithMouse = 1;
    public float SpeedRotationWithoutMouse = 1;
    //Ускорение
    public float Acceleration = 2;
    //Время неуязвимости
    public float TimeOfInvulnerability;
    //Время неуязвимости
    public int SpawnRatePerSecond;
    //Кол-во выстрелов в секунду
    public float FiringRatePerSecond;
    //Неуязвимость
    public bool Invulnerability = false;
    public Rigidbody rigidBody;

    //таймер неуязвимости
    float Timer = 0f;
    //Время между стрельбой
    float TimerFire = 0.0f;
    GameObject Mesh;
    AudioSource Sound;
    Main main;
    Vector3 velocityCache;
    bool ReturnValue = false;

    void Start()
    {
        main = Camera.main.GetComponent<Main>();
        Mesh = GetComponentInChildren<MeshRenderer>().gameObject;
        Sound = GetComponent<AudioSource>();
        //Становимся неуязвимыми
        Invulnerability = true;
    }

    void Update()
    {
        //Если игра не остановлена
        if (main.Processing)
        {
            //Возвращение импульсов
            if (ReturnValue)
            {
                ReturnValue = false;
                rigidBody.velocity = velocityCache;
            }
            //Если время неуязвимости прошло
            if (Timer > TimeOfInvulnerability)
            {
                Invulnerability = false;
                Mesh.SetActive(true);
            }
            //Если корабль неуязвим
            if (Invulnerability)
            {
                Timer += Time.deltaTime;
                if (Timer % (1.0f / SpawnRatePerSecond) > (1.0f / (SpawnRatePerSecond * 2))) Mesh.SetActive(false);
                else Mesh.SetActive(true);
            }
            //Если время отката выстрела не прошло
            if (TimerFire > 0)
                TimerFire -= Time.deltaTime;
            //Управлние с мышкой
            if (main.ControlWithMouse)
            {
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetMouseButtonDown(1))
                {
                    float newX = (new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z) + transform.forward).x;
                    float newZ = (new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z) + transform.forward).z;
                    float newSpeed = Mathf.Sqrt(newX * newX + newZ * newZ);

                    if (newSpeed < SpeedMax)
                        rigidBody.AddForce(transform.forward * Acceleration);
                }

                Vector3 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
                direction.y = 0;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), SpeedRotationWithMouse);

                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    if (TimerFire <= 0)
                    {
                        Sound.Play();
                        GameObject bulletObject = PooledGameObject.Instance.GetBullet();
                        bulletObject.transform.position = gameObject.transform.position;
                        bulletObject.transform.forward = gameObject.transform.forward;
                        bulletObject.GetComponent<Bullet>().SetType(0);
                        TimerFire = 1 / FiringRatePerSecond;
                    }
                }
            }
            //Управление по клавиатуре
            else
            {
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    float newX = (new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z) + transform.forward).x;
                    float newZ = (new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z) + transform.forward).z;
                    float newSpeed = Mathf.Sqrt(newX * newX + newZ * newZ);

                    if (newSpeed < SpeedMax)
                        rigidBody.AddForce(transform.forward * Acceleration);
                }
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                {
                    transform.Rotate(0, SpeedRotationWithoutMouse * Time.deltaTime, 0, Space.World);
                }
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                {
                    transform.Rotate(0, -SpeedRotationWithoutMouse * Time.deltaTime, 0, Space.World);
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (TimerFire <= 0)
                    {
                        Sound.Play();
                        GameObject bulletObject = PooledGameObject.Instance.GetBullet();
                        bulletObject.transform.position = gameObject.transform.position;
                        bulletObject.transform.forward = gameObject.transform.forward;
                        bulletObject.GetComponent<Bullet>().SetType(0);
                        TimerFire = 1 / FiringRatePerSecond;
                    }
                }
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
        //Захват и обнуление импульсов
        else
        {
            if (!ReturnValue)
            {
                velocityCache = rigidBody.velocity;
                ReturnValue = true;
                rigidBody.velocity = new Vector3(0, 0, 0);
            }
        }
    }

    public override void Destruction(Collider other)
    {
        //Если файл не неуязвим
        if (!Invulnerability)
        {
            main.ShipDestruction();
            Destroy(gameObject);
        }
    }
}
