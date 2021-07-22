using UnityEngine;
using UnityEngine.UI;

public class Robot : MonoBehaviour
{
    [HideInInspector] public bool isReady = false;
    [HideInInspector] public string switchKey;
    [HideInInspector] public int playerIndex;
    [Range(10, 100)] public float powerbarSpeed = 25;
    [Range(50, 200)] public float rotationSpeed = 75;
    public Phase phase;
    public Slider powerSlider;
    public Animator animator;
    public Image v1, v2;
    SwitchManager switchManager;
    GameManager manager;
    //Animator animator;
    Rigidbody rb;
    float power;
    float powerDirection;
    bool directionSelected;
    bool rotate = true;
    bool rollingBall;
    int score = 0;

	public enum Phase
	{
		direction, power, ready, roll, stop, wait
	}

    void Awake()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        switchManager = GameObject.Find("GameManager").GetComponent<SwitchManager>();
        phase = Phase.wait;
    }

	void Start()
    {
        rb = GetComponent<Rigidbody>();
        //animator = transform.GetChild(1).GetComponent<Animator>();
        directionSelected = false;
        powerDirection = 1;
	}

	void Update()
    {
        powerSlider.value = power;
        animator.SetFloat("Speed", rb.velocity.magnitude);
        animator.speed = Mathf.Clamp(rb.velocity.magnitude * 0.2f, 0, 3);

        switch (phase)
		{
			case Phase.direction:
                power = 0;

                if (directionSelected == false)
				{
					rollingBall = false;

                    if (rotate)
                    {
                        transform.Rotate(Vector3.up, (rotationSpeed / Settings.speed) * Time.deltaTime);
                    }
				}

				if (Input.GetButtonDown(switchKey))
				{
					directionSelected = true;
				}

				if (Input.GetButtonUp(switchKey))
                {
                    phase = manager.NextPhase(phase);
                }

				break;

			case Phase.power:
                if (power <= 0)
                {
                    powerDirection = 1;
                }

                if (power >= 100)
                {
                    powerDirection = -1;
                }

                power += powerDirection * Time.deltaTime * (powerbarSpeed / Settings.speed);
                powerSlider.value = power;

				if (Input.GetButtonUp(switchKey))
                {
                    phase = manager.NextPhase(phase);
                }

				break;

            case Phase.ready:
                //animator.SetBool("isIdle", true);
                manager.SetText("READY", playerIndex);
                isReady = true;
                break;

            case Phase.roll:
                rb.AddForce(transform.right * power * 2800);
                phase = manager.NextPhase(phase);
                manager.SetText("", playerIndex);
                isReady = false;
                break;

            case Phase.stop:
				if (Mathf.Abs(rb.velocity.x) > 0.01f || Mathf.Abs(rb.velocity.y) > 0.01f || Mathf.Abs(rb.velocity.z) > 0.01f)
				{
					rollingBall = true;
				}

				if (rollingBall)
				{
					if (Mathf.Abs(rb.velocity.x) < 0.01f && Mathf.Abs(rb.velocity.y) < 0.01f && Mathf.Abs(rb.velocity.z) < 0.01f)
					{
						rb.velocity = Vector3.zero;
						rb.angularVelocity = Vector3.zero;
                        directionSelected = false;

                        switch (manager.gamePlay)
                        {
                            case GameManager.GamePlay.turn:
                                phase = manager.NextPhase(phase);
                                manager.nextTurn = true;
                                break;
                            case GameManager.GamePlay.arena:
                                phase = manager.NextPhase(phase);
                                break;
                            default:
                                break;
                        }
                    }
				}

				break;

            case Phase.wait:
                //animator.SetBool("isIdle", true);
                break;

            default:
				break;
		}
    }

    void OnTriggerEnter(Collider target)
    {
        if (target.CompareTag("Battery"))
        {
            score++;
            switchManager.HideBattery();
        }

        if (score == 1)
        {
            v1.enabled = true;
        }

        if (score == 2)
        {
            v2.enabled = true;
        }

        if (score == 3)
        {
            score = 0;
            manager.toast.Toast("P" + (playerIndex + 1) + " WINS");
            manager.gameover = true;
            StartCoroutine(manager.LoadMenu());
        }
    }
}
