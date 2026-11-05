using System;
using System.Collections.Generic;
using UnityEngine;

public class CoralSpawner : MonoBehaviour
{
    public float offset = 0;
    public float gridSize = 16; // Size of the Voronoi grid
    public float randomness = 1f; // Randomness factor for coral placement
    public RockPerlinNoise rockPerlinNoise; // Reference to the RockPerlinNoise script
    public Dictionary<Vector2Int, GameObject> coralDictionary = new Dictionary<Vector2Int, GameObject>(); // Dictionary to store spawned corals
    public Sprite[] coralSprites; // Array of coral sprites to choose from
    public Sprite palette; 
    public Sprite greyPallete;
    public Sprite[] sprites;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generateSprites();
        updateCoral();
    }

    //Takes grey coral sprites and generates colored coral sprites based on the palette.
    void generateSprites()
    {
        sprites = new Sprite[28];

        // Grey keys once
        Color32[] greyKeys = new Color32[6];
        for (int k = 0; k < 6; k++)
            greyKeys[k] = greyPallete.texture.GetPixel(k, 31);

        for (int i = 0; i < 4; i++)
        {
            Sprite src = coralSprites[i];
            Rect r = src.textureRect;
            int w = (int)r.width, h = (int)r.height;
            Color32[] srcPixels = Texture2DGetPixels32(src.texture, (int)r.x, (int)r.y, w, h);

            for (int j = 0; j < 7; j++)
            {
                Color32[] outPixels = new Color32[srcPixels.Length];

                for (int p = 0; p < srcPixels.Length; p++)
                {
                    Color32 c = srcPixels[p];
                    if (c.a > 0)
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            if (c.r == greyKeys[k].r && c.g == greyKeys[k].g && c.b == greyKeys[k].b)
                            {
                                c = palette.texture.GetPixel(k, 31-j); // flip to (k, palette.texture.height - 1 - j) if rows are reversed
                                break;
                            }
                        }
                    }
                    outPixels[p] = c;
                }

                Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
                tex.filterMode = FilterMode.Point;
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.SetPixels32(outPixels);
                tex.Apply();

                sprites[i * 7 + j] = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0f), 16);
            }
        }
    }

    // Reads a sub-rectangle of a texture as Color32
    static Color32[] Texture2DGetPixels32(Texture2D t, int x, int y, int w, int h)
    {
        Color[] block = t.GetPixels(x, y, w, h);
        Color32[] result = new Color32[block.Length];
        for (int i = 0; i < block.Length; i++) result[i] = block[i];
        return result;
    }

 
// Updates coral positions based on the current offset and grid size
    public void updateCoral()
    {
        int firstCellX = Mathf.FloorToInt((offset - gridSize * 2) / gridSize);
        int lastCellX  = Mathf.CeilToInt((offset + 320 + gridSize * 2) / gridSize);
        int firstCellY = -1;
        int lastCellY  = Mathf.CeilToInt((180 + gridSize) / gridSize);
        for (int cx = firstCellX; cx <= lastCellX; cx++)
        {
            for (int cy = firstCellY; cy <= lastCellY; cy++)
            {
                float worldX = (cx + Hash01(cx, cy, 0) * randomness) * gridSize;
                float worldY = (cy + Hash01(cx, cy, 1) * randomness) * gridSize;

                if (worldY >= rockPerlinNoise.getHeight(worldX))
                {
                    continue;
                }
                // World -> screen (scroll) -> Unity units
                Vector2 spawnPosition = new Vector2(worldX - offset, worldY) / 16f + new Vector2(-10f, -5.625f);
                spawnPosition.x = Mathf.Round(spawnPosition.x * 16f) / 16f; // Round to nearest pixel
                spawnPosition.y = Mathf.Round(spawnPosition.y * 16f) / 16f; // Round to nearest pixel
                Vector2Int key = new Vector2Int(cx, cy);
                if (coralDictionary.TryGetValue(key, out var coral))
                {
                    coral.transform.position = spawnPosition;
                }
                else
                {
                    coralDictionary[key] = spawnCoral(spawnPosition, (int)(Hash01(cx, cy, 2) * sprites.Length));
                    print((int)(Hash01(cx, cy, 2) * sprites.Length));
                }
            }
        }
    }

    // Deterministic 0..1 value per cell
    static float Hash01(int x, int y, int salt)
    {
        unchecked
        {
            uint h = (uint)(x * 73856093) ^ (uint)(y * 19349663) ^ (uint)(salt * 83492791);
            h ^= h >> 13;
            h *= 0x5bd1e995;
            h ^= h >> 15;
            return (h & 0xFFFFFF) / 16777216f;
        }
    }
// Spawns a coral GameObject at the specified position with the specified sprite index
    GameObject spawnCoral(Vector2 position, int spriteIndex)
    {
        GameObject coral = new GameObject("Coral");
        coral.transform.position = position;
        coral.transform.localScale = new Vector3(1, 1, 1);
        coral.AddComponent<SpriteRenderer>().sprite = sprites[spriteIndex];
        return coral;
    }

//Kills coral so that too much coral doesn't spawn when the grid size is changed.
    public void killCoral()
    {
        foreach (var coral in coralDictionary.Values)
        {
            Destroy(coral);
        }
        coralDictionary.Clear();
    }
}
