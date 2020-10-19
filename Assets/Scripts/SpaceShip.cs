using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    //При столкновении с чем-либо
    private void OnTriggerStay(Collider other)
    {
        //При столкновении с астероидом или другим кораблем
        if (other.gameObject.CompareTag("Asteroid") || other.gameObject.CompareTag("SpaceShip"))
        {
            Destruction(other);
        }
        //При столкновении с пулей
        else if (other.gameObject.name.Contains("Bullet"))
        {
            //Если пуля соперника
            if ((other.gameObject.GetComponent<Bullet>().type == 0 && !gameObject.name.Contains("Rocket"))
                || (other.gameObject.GetComponent<Bullet>().type == 1 && !gameObject.name.Contains("UFO")))
                Destruction(other);
        }
    }

    public virtual void Destruction(Collider other)
    {

    }
}
