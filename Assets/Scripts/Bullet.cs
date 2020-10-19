using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int type;
    public int Speed;
    Main main;

    void Start()
    {
        main = Camera.main.GetComponent<Main>();
    }

    // Устанавливаем тип объекта свой/враждебный
    public void SetType(int NewType)
    {
        type = NewType;
        //Если пуля ракеты
        if (type == 0)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }
        //Если пуля НЛО
        else
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (main.Processing)
        {
            transform.position += transform.forward * Speed * Time.deltaTime;
            //Выход за границы экрана
            //Вычисляем позиции относительно экрана
            Vector3 PositionOnScreen = Camera.main.WorldToScreenPoint(transform.position);
            //Вычисляем фактические коориданаты
            Vector3 PositionInWorld = transform.position;
            //Проверяем на выход и меняем координаты
            if ((PositionOnScreen.x > Screen.width) || (PositionOnScreen.x < 0) ||
                (PositionOnScreen.y > Screen.height) || (PositionOnScreen.y < 0))
                gameObject.SetActive(false);
            //Устанавливаем новую позицию
            transform.position = PositionInWorld;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Проверка на хозяина пули для избежания самоуничтожения
        if((!other.gameObject.name.Contains("Rocket") && type == 0)
            || (!other.gameObject.name.Contains("UFO") && type == 1))
            gameObject.SetActive(false);
    }
}
