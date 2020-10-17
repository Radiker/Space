using System.Collections.Generic;
using UnityEngine;

public class PooledGameObject : MonoBehaviour
{
    private static PooledGameObject instance;
    public static PooledGameObject Instance { get { return instance; } }

    public GameObject bulletPrefab;
    public int NumberBullets = 40;
    List<GameObject> Bullets;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        Bullets = new List<GameObject>(NumberBullets);

        for (int i = 0; i < NumberBullets; i++)
        {
            GameObject prefab = Instantiate(bulletPrefab);
            prefab.transform.SetParent(transform);
            prefab.SetActive(false);
            Bullets.Add(prefab);
        }
    }

    public GameObject GetBullet()
    {
        foreach (GameObject bullet in Bullets)
            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet;
            }

        GameObject prefabInstance = Instantiate(bulletPrefab);
        prefabInstance.transform.SetParent(transform);
        Bullets.Add(prefabInstance);

        return prefabInstance;
    }
}
