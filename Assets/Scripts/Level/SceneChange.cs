using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public Object nextScene;
    public GameObject loadingScreen;
    [SerializeField] private GameObject spawnPosObj;

    [HideInInspector] public Vector3 playerSpawnPos;
    public float loadTime = 1f;

    void Awake()
    {
        playerSpawnPos = spawnPosObj.GetComponent<SpawnPositionObject>().spawnPos;
    }
}


