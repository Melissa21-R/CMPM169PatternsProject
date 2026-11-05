using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public RockPerlinNoise rockPerlinNoise;
    public CoralSpawner coralSpawner;
    private float moveVelocity = 0f; // Velocity for smooth movement
    public float moveSpeed = 1f; // Speed at which the rock pattern moves
    public float x = 0f;
    private float densityVelocity = 0f; // Density of the rock pattern
    public float densitySpeed = 1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveVelocity = input.x * moveSpeed; // Adjust the multiplier for desired speed
        densityVelocity = input.y * densitySpeed; // Adjust the multiplier for desired speed
    }

    // Update is called once per frame
    void Update()
    {
        if(moveVelocity != 0f)
        {
            x += moveVelocity * Time.deltaTime; // Update the x position based on the velocity and time
            rockPerlinNoise.offset = (int)x; // Snap the offset to the nearest multiple of 16
            rockPerlinNoise.updateRock(); // Update the rock pattern based on the new offset
            coralSpawner.offset = (int)x; // Snap the offset to the nearest multiple of 16
            coralSpawner.updateCoral(); // Update the coral positions based on the new offset
        }
        if(densityVelocity != 0f)
        {
            coralSpawner.gridSize -= densityVelocity * Time.deltaTime; // Update the grid size based on the velocity and time
            if(coralSpawner.gridSize < 13f)
            {
                coralSpawner.gridSize = 13f; // Limit the minimum grid size to 13
            }
            else if(coralSpawner.gridSize > 30f)
            {
                coralSpawner.gridSize = 30f; // Limit the maximum grid size to 30
            }
            coralSpawner.killCoral(); // Remove existing corals before updating
            coralSpawner.updateCoral(); // Update the coral positions based on the new grid size
        }
    }
}

