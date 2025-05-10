using System.Collections.Generic;
using UnityEngine;

public class HighlightPoolManager : MonoBehaviour
{
    public enum HighlightType
    {
        Movement,
        Attack,
        Hover,
        Skill,
        Invalid
    }

    private const float YOffSet = 0.14f;

    private static HighlightPoolManager _instance;

    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private int poolSize = 75;
    [SerializeField] private Transform poolParent;

    [Header("Highlight Materials")] [SerializeField]
    private Material highlightMaterial;

    [SerializeField] private Material hoverHighlightMaterial;
    [SerializeField] private Material invalidHighlightMaterial;
    private Dictionary<Tile, GameObject> _activeHighlights;

    private Queue<GameObject> _availableHighlights;

    public static HighlightPoolManager Instance { get; private set; }

    private void Awake()
    {
        // Only one instance of the manager can exist
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // initialized vars
        Instance = this;

        _availableHighlights = new Queue<GameObject>();
        _activeHighlights = new Dictionary<Tile, GameObject>();

        // create the parent for highlight planes if it doesn't exist
        if (!poolParent)
        {
            poolParent = new GameObject("HighlightPool").transform;
            poolParent.SetParent(transform);
        }

        // initialized pool
        InitializePool();

        Debug.Log("HighlightPoolManager initialized. Pool size: " + _availableHighlights.Count);
    }

    private void InitializePool()
    {
        for (var i = 0; i < poolSize; i++)
        {
            var go = Instantiate(highlightPrefab, poolParent);
            go.SetActive(false);
            _availableHighlights.Enqueue(go);
        }
    }

    public void HighlightTile(Tile tile, HighlightType type)
    {
        // if there is an active highlight, deactivate it
        if (_activeHighlights.TryGetValue(tile, out var existingHighlight)) ReturnToPool(tile);

        // if run out of highlight planes, create a new one
        if (_availableHighlights.Count == 0)
        {
            var newHighlight = Instantiate(highlightPrefab, poolParent);
            _availableHighlights.Enqueue(newHighlight);
        }

        // Get a plane from the pool
        var highlight = _availableHighlights.Dequeue();

        // Set transform of the object
        var tilePosition = tile.transform.position;
        highlight.transform.position = new Vector3(tilePosition.x, YOffSet, tilePosition.z);
        highlight.transform.rotation = Quaternion.Euler(0, 0, 0);

        // set material to plane
        var highlightRenderer = highlight.GetComponent<Renderer>();
        if (!highlightRenderer) return;
        highlightRenderer.material = type switch
        {
            HighlightType.Movement => highlightMaterial,
            HighlightType.Hover => hoverHighlightMaterial,
            HighlightType.Invalid => invalidHighlightMaterial,
            _ => highlightMaterial
        };

        // set the plane to an active GameObject
        highlight.SetActive(true);
        _activeHighlights[tile] = highlight;
    }

    /// <summary>
    ///     Return the plane to the pool
    /// </summary>
    /// <param name="tile"></param>
    public void ReturnToPool(Tile tile)
    {
        if (!tile) return;

        if (_activeHighlights.TryGetValue(tile, out var highlight))
        {
            highlight.SetActive(false);
            _availableHighlights.Enqueue(highlight);
            _activeHighlights.Remove(tile);
        }
    }

    /// <summary>
    ///     Clear all active highlights
    /// </summary>
    public void ClearAllHighlights()
    {
        foreach (var highlight in _activeHighlights.Values)
        {
            highlight.SetActive(false);
            _availableHighlights.Enqueue(highlight);
        }

        _activeHighlights.Clear();
    }
}