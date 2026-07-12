using System.Collections.Generic;
using UnityEngine;
using _Project.Scripts;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("Piece Prefabs")] [SerializeField]
    private List<GameObject> metalPrefabs;
    [SerializeField] private List<GameObject> guardPrefabs;
    [SerializeField] private List<GameObject> hiltPrefabs;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] metalSpawnPoints;
    [SerializeField] private Transform[] guardSpawnPoints;
    [SerializeField] private Transform[] hiltSpawnPoints;

    [Header("Round Settings")]
    [SerializeField] private int piecesPerCategory = 2;
    [SerializeField] private float roundTimeSeconds = 60f;

    private float timeRemaining;
    private bool roundActive;

    private readonly List<GameObject> activePieces = new List<GameObject>();

    public int Score { get; private set; }

    void Start()
    {
        StartRound();
    }

    void Update()
    {
        if (!roundActive)
        {
            return;
        }

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            EndRound();
        }
    }

    public void StartRound()
    {
        Score = 0;
        timeRemaining = roundTimeSeconds;
        roundActive = true;

        ClearBoard();
        SpawnBatch();
    }

    void EndRound()
    {
        roundActive = false;
        Debug.Log($"Final score: {Score}");
    }
    
    public void SubmitPieces()
    {
        if (!roundActive)
        {
            return;
        }

        List<List<DraggablePiece>> groups = GetConnectedGroups();

        foreach (List<DraggablePiece> group in groups)
        {
            int points = ScoreGroup(group);
            Score += points;

            foreach (DraggablePiece piece in group)
            {
                activePieces.Remove(piece.gameObject);
                Destroy(piece.gameObject);
            }
        }

        SpawnBatch();
    }
    
    int ScoreGroup(List<DraggablePiece> group)
    {
        return group.Count == 3 ? 1 : 0;
    }

    List<List<DraggablePiece>> GetConnectedGroups()
    {
        Dictionary<Transform, List<DraggablePiece>> groupsByRoot = new Dictionary<Transform, List<DraggablePiece>>();

        foreach (GameObject obj in activePieces)
        {
            DraggablePiece piece = obj.GetComponent<DraggablePiece>();
            Transform root = DraggablePiece.GetGroupRoot(piece.transform);

            if (!groupsByRoot.TryGetValue(root, out List<DraggablePiece> list))
            {
                list = new List<DraggablePiece>();
                groupsByRoot[root] = list;
            }

            list.Add(piece);
        }

        return new List<List<DraggablePiece>>(groupsByRoot.Values);
    }

    void SpawnBatch()
    {
        SpawnFromCategory(metalPrefabs, metalSpawnPoints);
        SpawnFromCategory(guardPrefabs, guardSpawnPoints);
        SpawnFromCategory(hiltPrefabs, hiltSpawnPoints);
    }

    void SpawnFromCategory(List<GameObject> prefabPool, Transform[] spawnPoints)
    {
        List<GameObject> pool = new List<GameObject>(prefabPool);

        for (int i = 0; i < piecesPerCategory && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            GameObject prefab = pool[index];
            pool.RemoveAt(index); 

            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            GameObject instance = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            activePieces.Add(instance);
        }
    }

    void ClearBoard()
    {
        foreach (GameObject piece in activePieces)
        {
            if (piece != null)
            {
                Destroy(piece);
            }
        }

        activePieces.Clear();
    }
}