using System.Linq;
 
using UnityEngine;

using UnityEngine.EventSystems;
using System.Collections.Generic;

public sealed class TerrainTools : MonoBehaviour
{
    public GameManager manager;
    public enum TerrainModificationAction
    {
        Raise,
        Lower,
        Flatten,
        Sample,
        SampleAverage,
        Paint
    }
 
    public int brushWidth;
    public int brushLength;
 
    public float strength;
 
    public TerrainModificationAction modificationAction;
 
    private Terrain _targetTerrain;
 
    private float _sampledHeight;

    public Terrain terrain; // Reference to the Terrain component

    public TerrainData originalTerrainData;

    private TerrainData temporaryTerrainData; // Temporary copy of the original terrain data
    public TerrainCollider terrainCollider;
    public TerrainLayer[] terrainLayers; // Drag and drop your TerrainLayer assets here in the Inspector
    private int _selectedLayerIndex = 0;

    public void ChangeToRaise()
    {
        modificationAction = TerrainModificationAction.Raise;
    }
    public void ChangeToFlatten()
    {
        modificationAction = TerrainModificationAction.Flatten;
    }
    public void ChangeToLower()
    {
        modificationAction = TerrainModificationAction.Lower;
    }
    public void SetToPaintMode()
    {
        modificationAction = TerrainModificationAction.Paint;
    }

    public void ChangeToSand()
    {
        SelectLayer(0);
    }
    public void ChangeToGrass()
    {
        SelectLayer(1); 
    }



    public void ChangeToRock()
    {
        SelectLayer(2); 
    }

    private void Start()
    {
        // Set to sand

        // Store the original terrain data
        originalTerrainData = terrain.terrainData;

        // Create a temporary copy of the original terrain data
        temporaryTerrainData = new TerrainData();
        temporaryTerrainData.heightmapResolution = originalTerrainData.heightmapResolution;
        temporaryTerrainData.size = originalTerrainData.size;
        temporaryTerrainData.SetHeights(0, 0, originalTerrainData.GetHeights(0, 0, originalTerrainData.heightmapResolution, originalTerrainData.heightmapResolution));
        _targetTerrain = terrain;

        terrain.terrainData = temporaryTerrainData;


        SelectLayer(0);
        SelectLayer(1);
        SelectLayer(2);

        SelectLayer(1);

        PaintTexture(Vector3.zero);


        terrainCollider.terrainData = temporaryTerrainData;

    }
    private void Update()
    {
        brushWidth = (int) manager.UI.TerrainWidth.value;
        brushLength = (int)manager.UI.TerrainLength.value;
        strength = manager.UI.TerrainStrength.value;


        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;  // Exit the update if the mouse is over a UI element.
        }

        if (Input.GetMouseButton(0))
        {

            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(camRay, out var hit, Mathf.Infinity))
            {
                // Get the accurate hit point using SampleHeight

                if (hit.transform.TryGetComponent(out Terrain terrain))
                {
                    _targetTerrain = terrain;
                }
                Vector3 preciseHitPoint = new Vector3(hit.point.x, _targetTerrain.SampleHeight(hit.point), hit.point.z);


                PaintTexture(preciseHitPoint);

                switch (modificationAction)
                {
                    case TerrainModificationAction.Raise:
                        RaiseTerrain(preciseHitPoint, strength, brushWidth, brushLength);
                        break;
                    case TerrainModificationAction.Lower:
                        LowerTerrain(preciseHitPoint, strength, brushWidth, brushLength);
                        break;
                    case TerrainModificationAction.Flatten:
                        FlattenTerrain(preciseHitPoint, _sampledHeight, brushWidth, brushLength);
                        break;
                    case TerrainModificationAction.Sample:
                        _sampledHeight = SampleHeight(preciseHitPoint);
                        break;
                    case TerrainModificationAction.SampleAverage:
                        _sampledHeight = SampleAverageHeight(preciseHitPoint, brushWidth, brushLength);
                        break;
                }
                
            }
        }
    }

    private void PaintTexture(Vector3 worldPosition)
    {
        var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushLength);
        var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushLength);
        var terrainData = GetTerrainData();

        float[,,] alphaMap = terrainData.GetAlphamaps(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);

        for (int y = 0; y < brushSize.y; y++)
        {
            for (int x = 0; x < brushSize.x; x++)
            {
                // Determine the height at this point
                float height = terrainData.GetHeight(brushPosition.x + x, brushPosition.y + y);

                // If the height is below the threshold, set to sand
                if (height <= manager.settings.waterHeight + 15)
                {
                    for (int layer = 0; layer < terrainData.alphamapLayers; layer++)
                    {
                        //print(layer);
                        alphaMap[y, x, layer] = (layer == 0) ? 1 : 0;  // Assuming sand is at index 1
                    }
                }
                else
                {
                    // Otherwise, paint with the currently selected layer
                    for (int layer = 0; layer < terrainData.alphamapLayers; layer++)
                    {
                        alphaMap[y, x, layer] = (layer == _selectedLayerIndex) ? 1 : 0;
                    }
                }
            }
        }

        terrainData.SetAlphamaps(brushPosition.x, brushPosition.y, alphaMap);
    }

    public void SelectLayer(int index)
    {
        _selectedLayerIndex = index;
        if (_selectedLayerIndex >= 0 && _selectedLayerIndex < terrainLayers.Length)
        {
            // Ensure the layer is added to the terrain
            if (!System.Array.Exists(_targetTerrain.terrainData.terrainLayers, layer => layer == terrainLayers[_selectedLayerIndex]))
            {
                var layers = new List<TerrainLayer>(_targetTerrain.terrainData.terrainLayers);
                layers.Add(terrainLayers[_selectedLayerIndex]);
                _targetTerrain.terrainData.terrainLayers = layers.ToArray();
            }
        }
    }

    private void UpdateTexturesBelowWaterHeight()
    {
        var terrainData = GetTerrainData();
        int width = terrainData.alphamapWidth;
        int height = terrainData.alphamapHeight;

        float[,,] alphaMap = terrainData.GetAlphamaps(0, 0, width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Determine the height at this point
                float currentHeight = terrainData.GetHeight(y, x);

                // If the height is below the waterHeight, set to sand
                if (currentHeight <= manager.settings.waterHeight)
                {
                    for (int layer = 0; layer < terrainData.alphamapLayers; layer++)
                    {
                        alphaMap[y, x, layer] = (layer == 1) ? 1 : 0;  // Assuming sand is at index 1
                    }
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, alphaMap);
    }
    private TerrainData GetTerrainData() => _targetTerrain.terrainData;
 
    private int GetHeightmapResolution() => GetTerrainData().heightmapResolution;
 
    private Vector3 GetTerrainSize() => GetTerrainData().size;
 
    public Vector3 WorldToTerrainPosition(Vector3 worldPosition)
    {
        var terrainPosition = worldPosition - _targetTerrain.transform.position;
        var terrainSize = GetTerrainSize();
        var heightmapResolution = GetHeightmapResolution();

        terrainPosition = new Vector3(terrainPosition.x / terrainSize.x, terrainPosition.y / terrainSize.y, terrainPosition.z / terrainSize.z);
        return new Vector3(terrainPosition.x * heightmapResolution, 0, terrainPosition.z * heightmapResolution);
    }
 
    public Vector2Int GetBrushPosition(Vector3 worldPosition, int brushWidth, int brushHeight)
    {
        var terrainPosition = WorldToTerrainPosition(worldPosition);
 
        var heightmapResolution = GetHeightmapResolution();
 
        return new Vector2Int((int)Mathf.Clamp(terrainPosition.x - brushWidth / 2.0f, 0.0f, heightmapResolution), (int)Mathf.Clamp(terrainPosition.z - brushHeight / 2.0f, 0.0f, heightmapResolution));
    }
 
    public Vector2Int GetSafeBrushSize(int brushX, int brushY, int brushWidth, int brushHeight)
    {
        var heightmapResolution = GetHeightmapResolution();
 
        while (heightmapResolution - (brushX + brushWidth) < 0) brushWidth--;
 
        while (heightmapResolution - (brushY + brushHeight) < 0) brushHeight--;
 
        return new Vector2Int(brushWidth, brushHeight);
    }
 
    public void RaiseTerrain(Vector3 worldPosition, float strength, int brushWidth, int brushHeight)
    {
        var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);
 
        var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);
 
        var terrainData = GetTerrainData();
 
        var heights = terrainData.GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);
 
        for (var y = 0; y < brushSize.y; y++)
        {
            for (var x = 0; x < brushSize.x; x++)
            {
                heights[y, x] += strength * Time.deltaTime;
            }
        }
 
        terrainData.SetHeights(brushPosition.x, brushPosition.y, heights);
    }
 
    public void LowerTerrain(Vector3 worldPosition, float strength, int brushWidth, int brushHeight)
    {
        var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);
 
        var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);
 
        var terrainData = GetTerrainData();
 
        var heights = terrainData.GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);
 
        for (var y = 0; y < brushSize.y; y++)
        {
            for (var x = 0; x < brushSize.x; x++)
            {
                heights[y, x] -= strength * Time.deltaTime;
            }
        }
 
        terrainData.SetHeights(brushPosition.x, brushPosition.y, heights);
    }
 
    public void FlattenTerrain(Vector3 worldPosition, float height, int brushWidth, int brushHeight)
    {
        var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);
 
        var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);
 
        var terrainData = GetTerrainData();
 
        var heights = terrainData.GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);
 
        for (var y = 0; y < brushSize.y; y++)
        {
            for (var x = 0; x < brushSize.x; x++)
            {
                heights[y, x] = height;
            }
        }
 
        terrainData.SetHeights(brushPosition.x, brushPosition.y, heights);
    }
    public void ResetTerrain()
    {
        // Create a new TerrainData object based on the original terrain data
        TerrainData newTerrainData = new TerrainData();
        newTerrainData.heightmapResolution = originalTerrainData.heightmapResolution;
        newTerrainData.size = originalTerrainData.size;

        // Apply the new TerrainData to the terrain
        terrain.terrainData = newTerrainData;
    }

    private void OnApplicationQuit()
    {
        ResetTerrain();
    }

 
    public float SampleHeight(Vector3 worldPosition)
    {
        var terrainPosition = WorldToTerrainPosition(worldPosition);
 
        return GetTerrainData().GetInterpolatedHeight((int)terrainPosition.x, (int)terrainPosition.z);
    }
 
    public float SampleAverageHeight(Vector3 worldPosition, int brushWidth, int brushHeight)
    {
        var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);
 
        var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);
 
        var heights2D = GetTerrainData().GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);
 
        var heights = new float[heights2D.Length];
 
        var i = 0;
 
        for (int y = 0; y <= heights2D.GetUpperBound(0); y++)
        {
            for (int x = 0; x <= heights2D.GetUpperBound(1); x++)
            {
                heights[i++] = heights2D[y, x];
            }
        }
 
        return heights.Average();
    }
}