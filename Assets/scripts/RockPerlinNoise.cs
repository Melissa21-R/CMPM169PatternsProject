using UnityEngine;

public class RockPerlinNoise : MonoBehaviour
{
    public float offset = -1; //all different pieces of rock will connect
    public Color rockColor;
    public float scale = 0.5f; // Scale of the Perlin noise
    public int octaves = 4; // Number of octaves for the Perlin noise
    Texture2D texture;
    void Awake() 
    {
        if(offset == -1) 
        {
            offset = Random.Range(0, 1000);
        }
    }
    void Start()
    {
        texture = new Texture2D(320, 180);
        texture.filterMode = FilterMode.Point; // Set the filter mode to Point for pixelated effect
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit: 16);
        GetComponent<SpriteRenderer>().sprite = sprite; 
        updateRock();
    }   

    /*
    void OnValidate()
    {
        updateRock();
    }
    */

    public void updateRock()
    {
        for (int x = 0; x < texture.width; x++)
        {

            int yThreshold = getHeight(x + offset); // Get the height of the rock at this x position
            for (int y = 0; y < texture.height; y++)
            {
                Color color = new Color(0, 0, 0, 0); // Transparent color
                if (y < yThreshold)
                {
                    color = rockColor; // Set the rock color for pixels above the threshold
                }
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();

    }

    public int getHeight(float x)
    {
         float xCoord = (float)x * scale + 1000f; // Offset the x coordinate by the seed value
            float sample = 0.5f;
            float frequency = 1f;
            for (int i = 0; i < octaves; i++)
            {
                sample += (Mathf.PerlinNoise(xCoord * frequency, .5f + i * 10)-0.5f) / frequency;
                frequency *= 2f;
            }
        return Mathf.RoundToInt(sample * 180); // Scale the sample to the height of the texture
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
