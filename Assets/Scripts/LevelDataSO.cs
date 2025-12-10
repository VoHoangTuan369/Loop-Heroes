using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Game Data/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
    public int level;
    public int coinStart;
    public List<Wave> listWave;
}
[Serializable]
public class Wave 
{
    public float spawnInterval;
    public List<EnemyType> listEnemy;
}
public enum EnemyType 
{
    Normal1, Normal2, Normal3,
    NomarlHat1, NomarlHat2, NomarlHat3,
    BoneWeapon,
    AxeWeapon,
    SpikedClubWeapon,
    MillingCutterWeapon,
    Speed1, Speed2,
    Soldier1, Soldier2
}
