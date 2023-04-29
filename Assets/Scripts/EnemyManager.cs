using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    public int enemyCount = new int();

    public static EnemyManager instance;

    private void Awake()
    {
        instance = this;
        
    }

    private void Update()
    {
        UI.instance.UpdateNumberOfEnemies(enemyCount);
    }



}
