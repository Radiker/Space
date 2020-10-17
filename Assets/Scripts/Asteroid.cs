using UnityEngine;

public class Asteroid : MonoBehaviour
{
    //Астероиды поменьше 
    public GameObject SpawnAsteroid;
    //Кол-во очков за уничтожение
    public int Points;
    //Скорость полета
    public float Speed;
    Main main;

    void Start()
    {
        main = Camera.main.GetComponent<Main>();
    }

    void Update()
    {
        //Если игра запущена
        if (main.Processing)
        {
            //Передвигаем вперед
            transform.Translate(transform.forward * Speed * Time.deltaTime, Space.World);
            //Выход за границы экрана
            //Вычисляем позиции относительно экрана
            Vector3 PositionOnScreen = Camera.main.WorldToScreenPoint(transform.position);
            //Вычисляем фактические коориданаты
            Vector3 PositionInWorld = transform.position;
            //Проверяем на выход и меняем координаты
            if (PositionOnScreen.x > Screen.width)
                PositionInWorld.x = -PositionInWorld.x + 1;
            if (PositionOnScreen.x < 0)
                PositionInWorld.x = - PositionInWorld.x - 1;
            if (PositionOnScreen.y > Screen.height)
                PositionInWorld.z = -PositionInWorld.z + 1;
            if(PositionOnScreen.y < 0)
                PositionInWorld.z = - PositionInWorld.z - 1;
            //Устанавливаем новую позицию
            transform.position = PositionInWorld;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Если столкнулся с ракетой и неуязвимой
        if (other.gameObject.name.Contains("Rocket"))
            if (!other.gameObject.GetComponent<Rocket>().Invulnerability)
                Destruction();
        //Если столкнулся с НЛО
        if (other.gameObject.name.Contains("UFO"))
            Destruction();
        //Если столкнулся с пулей
        if (other.gameObject.name.Contains("Bullet"))
        {
            if(other.gameObject.GetComponent<Bullet>().type == 0)
                main.AddPoints(Points);
            Destruction();
        }
    }

    private void Destruction()
    {
        {
            //Воспроизведение звуков при разрушении
            main.SetAudio(GetComponent<AudioSource>().clip);

            //Если есть что спавнить
            if (SpawnAsteroid != null)
            {
                //Устанавливаем скорость
                int newSpeed = Random.Range(main.MinSpeedAsteroid, main.MaxSpeedAsteroid);
                //Инициализируем метеориты
                //Первый
                GameObject NewAsteroid1 = Instantiate(SpawnAsteroid);
                NewAsteroid1.transform.position = gameObject.transform.position;
                NewAsteroid1.transform.Rotate(0, gameObject.transform.eulerAngles.y + 45, 0, Space.World);
                NewAsteroid1.GetComponent<Asteroid>().Speed = newSpeed;
                //Второй
                GameObject NewAsteroid2 = Instantiate(SpawnAsteroid);
                NewAsteroid2.transform.position = gameObject.transform.position;
                NewAsteroid2.transform.Rotate(0, gameObject.transform.eulerAngles.y - 45, 0, Space.World);
                NewAsteroid2.GetComponent<Asteroid>().Speed = newSpeed;
                //Обновляем кол-во астреоидов на экране
                main.ChangeNumberOfAsteroids(2);
            }
            //Обновляем кол-во астреоидов на экране
            main.ChangeNumberOfAsteroids(-1);
            //Уничтожаем астероид
            Destroy(gameObject);
        }
    }
}
