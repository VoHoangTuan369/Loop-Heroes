using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [SerializeField] EnemyDataSO enemyDataSO;
    [SerializeField] List<LevelDataSO> listLevel;

    public EnemyDataSO EnemyDataSO { get => enemyDataSO; set => enemyDataSO = value; }
    public List<LevelDataSO> ListLevel { get => listLevel; set => listLevel = value; }
}
