using UnityEngine;

public class WaterVoronoi : MonoBehaviour
{
    public Vector2 offset = Vector2.zero; // Offset for the Voronoi pattern
    public int gridSize = 16; // Size of the Voronoi grid
    public float threshold1 = 0.5f; // Threshold for Voronoi edges
    public float threshold2 = 0.75f; // Another threshold for Voronoi edges
    public float speed = 1f; // Speed of the water movement
    public Vector2 direction = new Vector2(1, 1); // Direction of the water movement
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateWater();
    }

    void OnValidate()
    {
        updateWater();
    }

    void updateWater()
    {
        Texture2D texture = new Texture2D(320, 180);

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {

                float gridX = Mathf.Floor((x + offset.x) / gridSize);
                float gridY = Mathf.Floor((y + offset.y) / gridSize);

                float minDistance1 = float.MaxValue;
                float minDistance2 = float.MaxValue;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        float neighborX = gridX + i;
                        float neighborY = gridY + j;

                        float randomOffsetX = Hash01((int)neighborX, (int)neighborY, 0) * gridSize;
                        float randomOffsetY = Hash01((int)neighborX, (int)neighborY, 1) * gridSize;

                        float distance = Vector2.Distance(new Vector2(x, y) + offset, new Vector2(neighborX * gridSize + randomOffsetX, neighborY * gridSize + randomOffsetY));
                        
                        if(distance < minDistance1) {
                        minDistance2 = minDistance1;
                        minDistance1 = distance;
                        }
                        else if(distance < minDistance2) {
                        minDistance2 = distance;
                        }
                    }
                }

                float distancedif = minDistance2 - minDistance1;


                Color color = new Color(0.6f, 0.8f, 0.9f, 1); // Blue color for water
                if(distancedif < gridSize * threshold2)
                {
                    color = new Color(0.96f, 0.96f, 0.96f, 1); // Lighter blue for Voronoi edges
                }
                else if(distancedif < gridSize * threshold1)
                {
                    color = new Color(0.65f, 0.87f, 0.93f, 1); // Darker blue for Voronoi edges
                }
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        texture.filterMode = FilterMode.Point; // Set the filter mode to Point for pixelated effect
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit: 16);

        GetComponent<SpriteRenderer>().sprite = sprite; 
    }
    // Update is called once per frame
    void Update()
    {
        offset += direction * speed * Time.deltaTime;
        updateWater();
    }

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
}
