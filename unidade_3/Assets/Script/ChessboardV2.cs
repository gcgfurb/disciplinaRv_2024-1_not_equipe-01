using System.Collections.Generic;
using System.Linq;
using Script.ChessPieces;
using UnityEngine;

public class ChessboardV2 : MonoBehaviour
{
    [Header("ArtStuff")] [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 0.7f;
    [SerializeField] private float yOffset = 0.15f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathSize = 0.5f;
    [SerializeField] private float deathSpacing = 0.3725f;
    [SerializeField] private float dragOffset = 1.0f;

    [Header("Prefabs & Materials")] [SerializeField]
    private GameObject[] prefabs;

    [SerializeField] private Material[] teamMaterials;

    private ChessPiece[,] chessPieces;
    private ChessPiece currentlyDragging;
    private List<Vector2Int> availableMoves = new();
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private bool isWhiteTurn;
    private Dictionary<GameObject, Color> originalColors = new();

    private void Start()
    {
        isWhiteTurn = true;

        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);

        foreach (var prefab in prefabs)
        {
            var chessPiece = prefab.GetComponent<ChessPiece>();

            prefab.transform.position = GetTileCenter(chessPiece.currentX, chessPiece.currentY);
        }
    }

    public void OnBeginGrab(ChessPiece chessPiece)
    {
        currentlyDragging = chessPiece;
        availableMoves = currentlyDragging.GetAvailableMoves(
            prefabs.Select(x => x.GetComponent<ChessPiece>()).ToList(), TILE_COUNT_X, TILE_COUNT_Y, chessPiece);

        HighlightTiles();
    }

    public void EndBeginGrab(ChessPiece chessPiece)
    {
        Vector3 finalPosition = chessPiece.transform.position;
        Ray ray = new Ray(finalPosition + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f, LayerMask.GetMask("Highlight")))
        {
            Vector2Int finalTileIndex = LookupTileIndex(hit.collider.gameObject);

            // Verifique se a posição final está na lista de movimentos válidos
            if (availableMoves.Contains(finalTileIndex))
            {
                // Movimento válido, atualize a posição da peça
                chessPiece.transform.position = GetTileCenter(finalTileIndex.x, finalTileIndex.y);
                // Atualize a posição da peça no tabuleiro
                chessPiece.currentX = finalTileIndex.x;
                chessPiece.currentY = finalTileIndex.y;
            }
            else
            {
                // Movimento inválido, retorne a peça para sua posição original
                chessPiece.transform.position = GetTileCenter(chessPiece.currentX, chessPiece.currentY);
            }
        }
        
        RemoveHighlightTiles();
    }

    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);
        }

        return -Vector2Int.one;
    }

    //tabuleiro
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3(tileCountX / 2 * tileSize, 0, tileCountX / 2 * tileSize) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
        for (int y = 0; y < tileCountY; y++)
            tiles[x, y] = GenerateSingleTile(tileSize, x, y);
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject($"X:{x}, Y:{y}")
        {
            transform =
            {
                parent = transform
            }
        };

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        int[] tris = { 0, 1, 2, 1, 3, 2 };
        mesh.vertices = vertices;
        mesh.triangles = tris;

        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    private void HighlightTiles()
    {
        foreach (var t in availableMoves)
        {
            tiles[t.x, t.y].layer = LayerMask.NameToLayer("Highlight");
            originalColors.TryAdd(tiles[t.x, t.y], tiles[t.x, t.y].GetComponent<Renderer>().material.color);
            tiles[t.x, t.y].GetComponent<Renderer>().material.color = Color.green;
        }
    }

    private void RemoveHighlightTiles()
    {
        foreach (var t in availableMoves)
        {
            tiles[t.x, t.y].layer = LayerMask.NameToLayer("Tile");
            originalColors.TryGetValue(tiles[t.x, t.y], out Color originalColor);
            tiles[t.x, t.y].GetComponent<Renderer>().material.color = originalColor;
        }

        availableMoves.Clear();
    }
}