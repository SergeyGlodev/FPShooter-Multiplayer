using System;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] public Map[] maps;
    [SerializeField] private int currentMapID;
    
    private Map currentMap;
    private GameObject currentMapInGame;


    private void Awake()
    {
        GlobalEvents.OnStartGamePressed += CreateMap;
        GlobalEvents.OnLeaveGame += DestroyCurrentMap;

        currentMap = maps[currentMapID];
    }

    public Vector3 GetCurrentTBase() => currentMap.tBase;
    
    public Vector3 GetCurrentCtBase() => currentMap.ctBase;
    
    public Quaternion GetCurrentTBaseRotate() => currentMap.tRotate;
    
    public Quaternion GetCurrentCtBaseRotate() => currentMap.ctRotate;
    
    private void CreateMap() => currentMapInGame = Instantiate(currentMap.prefab);
    
    private void DestroyCurrentMap() => Destroy(currentMapInGame);
    


    [Serializable] public class Map
    {
        public GameObject prefab;
        public Vector3 tBase;
        public Vector3 ctBase;
        public Quaternion tRotate;
        public Quaternion ctRotate;
    }
}
