using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class ChunkManager : MonoBehaviour
{
    [Header("Chunk Settings")]
    public int chunkSize = 100; // Размер одного чанка (метры)
    public int viewDistance = 2; // Сколько чанков загружать вокруг игрока

    private Dictionary<Vector2Int, Chunk> _loadedChunks = new Dictionary<Vector2Int, Chunk>();
    private Transform _playerTransform;

    private void Start()
    {
        _playerTransform = Camera.main.transform; // Предположим, что камера = игрок
        LoadInitialChunks();
    }

    private void Update()
    {
        UpdateChunks();
    }

    private void LoadInitialChunks()
    {
        Vector2Int playerChunk = GetChunkCoordinates(_playerTransform.position);
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int chunkCoordinates = playerChunk + new Vector2Int(x, y);
                LoadChunk(chunkCoordinates);
            }
        }
    }

    private void UpdateChunks()
    {
        Vector2Int playerChunk = GetChunkCoordinates(_playerTransform.position);

        // Загрузка новых чанков
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int chunkCoordinates = playerChunk + new Vector2Int(x, y);
                if (!_loadedChunks.ContainsKey(chunkCoordinates))
                {
                    LoadChunk(chunkCoordinates);
                }
            }
        }

        // Выгрузка лишних чанков
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var chunk in _loadedChunks.Keys)
        {
            if (Vector2Int.Distance(playerChunk, chunk) > viewDistance)
            {
                chunksToRemove.Add(chunk);
            }
        }

        foreach (var chunk in chunksToRemove)
        {
            UnloadChunk(chunk);
        }
    }

    private void LoadChunk(Vector2Int coordinates)
    {
        if (_loadedChunks.ContainsKey(coordinates)) return;

        // Генерация пути к префабу с использованием точки как разделителя
        string prefabPath = string.Format(CultureInfo.InvariantCulture,
            "Prefabs/Terrains/Terrain_({0:F2}, 0.00, {1:F2})",
            coordinates.x * chunkSize,
            coordinates.y * chunkSize);

        // Загрузка префаба
        GameObject chunkPrefab = Resources.Load<GameObject>(prefabPath);

        if (chunkPrefab == null)
        {
            Debug.LogWarning($"Chunk prefab not found at {prefabPath}");
            return;
        }

        // Создание и инициализация чанка
        GameObject chunkObject = Instantiate(chunkPrefab, transform);
        Chunk chunk = chunkObject.GetComponent<Chunk>();
        chunk.Initialize(coordinates);
        chunk.Activate();

        _loadedChunks[coordinates] = chunk;
    }

    private void UnloadChunk(Vector2Int coordinates)
    {
        if (!_loadedChunks.ContainsKey(coordinates)) return;

        Chunk chunk = _loadedChunks[coordinates];
        chunk.Deactivate();
        Destroy(chunk.gameObject);

        _loadedChunks.Remove(coordinates);
    }

    private Vector2Int GetChunkCoordinates(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int z = Mathf.FloorToInt(position.z / chunkSize);
        return new Vector2Int(x, z);
    }
}