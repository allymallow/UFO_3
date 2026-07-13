using System.Collections.Generic;
using _Project.Scripts;
using TMPro;
using UnityEngine;

namespace _Project._01_Scripts
{
    public class GameManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private AudioManager audioManager;

        [Header("Piece Prefabs")]
        [SerializeField] private List<GameObject> metalPrefabs;
        [SerializeField] private List<GameObject> guardPrefabs;
        [SerializeField] private List<GameObject> hiltPrefabs;

        [Header("Spawn Points")]
        [SerializeField] private Transform[] metalSpawnPoints;
        [SerializeField] private Transform[] guardSpawnPoints;
        [SerializeField] private Transform[] hiltSpawnPoints;

        [Header("Round Settings")]
        [SerializeField] private int piecesPerCategory = 2;
        [SerializeField] private float roundTimeSeconds = 60f;

        [Header("UI")]
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text scoreText;

        private float timeRemaining;
        private bool roundActive;

        private readonly List<GameObject> activePieces = new List<GameObject>();

        public int Score { get; private set; }

        void Start()
        {
            roundActive = false;
        }

        void Update()
        {
            if (!roundActive) return;

            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                UpdateTimerUI();
                EndRound();
                return;
            }

            UpdateTimerUI();

            if (uiManager != null)
            {
                uiManager.SetStatus(HasCompletedGroup());
            }
        }

        public void BeginGame()
        {
            if (uiManager != null)
            {
                uiManager.HideStartScreen();
            }

            StartRound();
        }

        public void StartRound()
        {
            Score = 0;
            timeRemaining = roundTimeSeconds;
            roundActive = true;

            ClearBoard();
            SpawnBatch();

            UpdateTimerUI();
            UpdateScoreUI();

            if (audioManager != null) audioManager.PlayNewOrderSound();
        }

        void EndRound()
        {
            roundActive = false;

            if (uiManager != null) uiManager.ShowEndScreen(Score);
            if (audioManager != null) audioManager.PlayEndScreenSound();
        }

        public void SubmitPieces()
        {
            if (!roundActive)
            {
                return;
            }

            if (!HasCompletedGroup())
            {
                return;
            }

            if (audioManager != null) audioManager.PlaySubmitSound();

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
            UpdateScoreUI();
        }

        bool HasCompletedGroup()
        {
            List<List<DraggablePiece>> groups = GetConnectedGroups();

            foreach (List<DraggablePiece> group in groups)
            {
                if (group.Count == 3)
                {
                    return true;
                }
            }

            return false;
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
            List<GameObject> topPicks = PickPrefabs(metalPrefabs);
            List<GameObject> middlePicks = PickPrefabs(guardPrefabs);
            List<GameObject> bottomPicks = PickPrefabs(hiltPrefabs);

            SpawnPicks(topPicks, metalSpawnPoints);
            SpawnPicks(middlePicks, guardSpawnPoints);
            SpawnPicks(bottomPicks, hiltSpawnPoints);

            if (uiManager != null)
            {
                uiManager.ShowNewOrderPopup(
                    GetSprites(topPicks),
                    GetSprites(middlePicks),
                    GetSprites(bottomPicks)
                );
            }
        }

        List<GameObject> PickPrefabs(List<GameObject> prefabPool)
        {
            List<GameObject> pool = new List<GameObject>(prefabPool);
            List<GameObject> picked = new List<GameObject>();

            for (int i = 0; i < piecesPerCategory && pool.Count > 0; i++)
            {
                int index = Random.Range(0, pool.Count);
                picked.Add(pool[index]);
                pool.RemoveAt(index);
            }

            return picked;
        }

        void SpawnPicks(List<GameObject> picks, Transform[] spawnPoints)
        {
            for (int i = 0; i < picks.Count; i++)
            {
                Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
                GameObject instance = Instantiate(picks[i], spawnPoint.position, Quaternion.identity);

                activePieces.Add(instance);
            }
        }

        List<Sprite> GetSprites(List<GameObject> prefabs)
        {
            List<Sprite> sprites = new List<Sprite>();

            foreach (GameObject prefab in prefabs)
            {
                SpriteRenderer renderer = prefab.GetComponentInChildren<SpriteRenderer>();

                if (renderer != null)
                {
                    sprites.Add(renderer.sprite);
                }
            }

            return sprites;
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

        void UpdateTimerUI()
        {
            if (timerText == null) return;

            int secondsLeft = Mathf.CeilToInt(timeRemaining);
            timerText.text = $"Time: {secondsLeft}";
        }

        void UpdateScoreUI()
        {
            if (scoreText == null) return;

            scoreText.text = $"Score:  {Score}";
        }

        public void RestartGame()
        {
            if (uiManager != null) uiManager.HideEndScreen();

            BeginGame();
        }
    }
}