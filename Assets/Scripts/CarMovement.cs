using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 10f;

    public float acceleration = 5f;
    private float maxSpeed = 50f;
    private float usedSpeed = 10f;


    private RNBOgenHelper helper;
    private RNBOgenHandle plugin;
    const int pluginId = 0; // Assuming the plugin ID is 0, change as needed
    readonly System.Int32 freqParam = (int)RNBOgenHandle.GetParamIndexById("freq");

    public float frequency = 100f;
    // Start is called before the first frame update
    void Start()
    {
        helper = RNBOgenHelper.FindById(pluginId);
        plugin = helper.Plugin;

        Debug.Log("Plugin: " + plugin);
        Debug.Log("param: " + freqParam);
        plugin.SetParamValue(freqParam, frequency);
    }

    // Update is called once per frame
    void Update()
    {
        // Get the input from the user
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        // Create a new Vector3 for the movement
        Vector3 direction = new Vector3(moveHorizontal, 0.0f, moveVertical);
        if (direction.magnitude >= 0.1f)
        {
            usedSpeed += acceleration * Time.deltaTime;
            if (usedSpeed > maxSpeed)
            {
                usedSpeed = maxSpeed;
            }
            controller.Move(direction * usedSpeed * Time.deltaTime);
        }
        else
        {
            usedSpeed = speed;
        }
    }
    
    void FixedUpdate()
    {
    }
}
