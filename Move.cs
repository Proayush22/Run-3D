using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Move : MonoBehaviour
{
    public static Move instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject[] floorsDark;
    public GameObject[] floorsLight;
    public Animator sphere;
    public Animator animator;
    public Animator cameraA;
    private string currentAnimation;
    private string currentSphereAnimation;
    public GameObject camera;

    public Rigidbody rb;
    public Transform transform;
    float speed = 10.0f;
    float spawnpos;
    float playerspawnpos;

    private bool move;
    private bool landed = false;
    private bool inverted = false;
    bool isGrounded = false;
    int jumps = 0;

    public float zpos;

    float xAxis;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;    
        move = false;

        zpos = 0;
        spawnpos = 20;
        for (int i = 0; i < 10; i++)
        {
            Instantiate(floorsDark[Random.Range(0, floorsDark.Length)], new Vector3(0, 0, spawnpos), Quaternion.identity);
            Instantiate(floorsLight[Random.Range(0, floorsLight.Length)], new Vector3(0, 15, spawnpos), Quaternion.Euler(0, 0, 180));
            spawnpos += 10;
        }
    }

    // Update is called once per frame
    void Update()
    {
        zpos = transform.position.z;

        xAxis = Input.GetAxisRaw("Horizontal");

        if (zpos > spawnpos - 60f)
        {
            Instantiate(floorsDark[Random.Range(0, floorsDark.Length)], new Vector3(0, 0, spawnpos), Quaternion.identity);
            Instantiate(floorsLight[Random.Range(0, floorsLight.Length)], new Vector3(0, 15, spawnpos), Quaternion.Euler(0, 0, 180));
            spawnpos += 10;
        }

        if(transform.position.y < -5)
        {
            transform.position = new Vector3(0, 7.5f, transform.position.z);
            move = false;
        }
        if (transform.position.y > 20)
        {
            transform.position = new Vector3(0, 7.5f, transform.position.z);
            move = false;
        }

        scoreText.text = "Distance Travelled: " + (int)(zpos) + "m";
        if (zpos > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", (int)zpos);
        }
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore") + "m";

        if((int)zpos % 100 == 0)
        {
            speed += 1f;
        }
    }
    private void FixedUpdate()
    {
        if (transform.position.y > 7.5)
        {
            //Physics.gravity = new Vector3(0, 350f, 0);
            Quaternion target = Quaternion.Euler(0, 0, 180);
            //transform.Rotate(transform.rotation.x, transform.rotation.y, 180f);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime);
            inverted = true;
        }
        else if (transform.position.y < 7.5)
        {
            //Physics.gravity = new Vector3(0, -350f, 0);
            Quaternion target = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime);
            inverted = false;
        }


        rb.velocity = new Vector3(0, 0, speed);
        camera.transform.position = new Vector3(0, 7.5f, transform.position.z - 26f);


        if(xAxis == 1)
        {
        rb.velocity = new Vector3(speed, 0, speed);
            if(inverted)
            rb.velocity = new Vector3(-speed, 0, speed);
            ChangeSphereState("SLeft");
        }
        else if(xAxis == -1)
        {
            rb.velocity = new Vector3(-speed, 0, speed);
            if (inverted)
                rb.velocity = new Vector3(speed, 0, speed);
            ChangeSphereState("SRight");
        }
        else
        {
            ChangeAnimationState("Take 001");
            ChangeSphereState("Default");
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            UnityEngine.Debug.Log("Grounded");

            ChangeAnimationState("Take 001");

            isGrounded = true;
            jumps = 1;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            UnityEngine.Debug.Log(jumps);
            if (jumps > 0)
            {
                jumps--;
                isGrounded = false;
                if (move)
                {

                    if (inverted)
                    {
                        Physics.gravity = new Vector3(0, -350f, 0);
                        cameraA.Play("Flip 181");
                    }
                    else
                    {
                        Physics.gravity = new Vector3(0, 350f, 0);
                        cameraA.Play("Flip 180");
                    }
                } else { move = true;}
                ChangeAnimationState("Jump");
            }
        }
    }
    /*
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }
    */
    void ChangeAnimationState(string newState)
    {
        if (currentAnimation == newState) return;

        animator.Play(newState);

        currentAnimation = newState;
    }

    void ChangeSphereState(string newState)
    {
        if (currentSphereAnimation == newState) return;

        sphere.Play(newState);

        currentSphereAnimation = newState;
    }
}
