using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathExplosion;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip startLevel;

    [SerializeField] ParticleSystem engineThrust;
    [SerializeField] ParticleSystem winEffect;
    [SerializeField] ParticleSystem deathEffect;


    Rigidbody rigidbody;
    AudioSource audioSource;


    enum State { Alive, Dying, Transcending };
    State state = State.Alive;
    bool collisionsDisabled = false ;
    // Start is called before the first frame update
    void Start()
    {   
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    { // TODO: Stop Sound on Death
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
            
        }
        if(Debug.isDebugBuild)
        {
            advanceNextLevel();
            toggleCollision(); 
        }
    }

    void advanceNextLevel()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
    }

    void toggleCollision()
    {
        if (Input.GetKey(KeyCode.C)) {
            collisionsDisabled = !collisionsDisabled; // toggle
        }
    }

    void OnCollisionEnter(Collision collision)
	{
        if (state != State.Alive || collisionsDisabled ) { return; }

        switch(collision.gameObject.tag)
		{

            case "Friendly":
                break;

            case "Finish":
                StartSuccessSequence();
                break;
      
            default:
                StartDeathSequence();
				break;
		}
	}

     

    void LoadFirstLevel()
	{
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
	}

    void StartSuccessSequence()
	{
        state = State.Transcending;
        Invoke("LoadNextLevel", levelLoadDelay);
        audioSource.Stop();
        audioSource.PlayOneShot(winSound);
        engineThrust.Stop();
        winEffect.Play();
	}

    void StartDeathSequence()
	{
        state = State.Dying;
        Invoke("LoadFirstLevel", levelLoadDelay);
        audioSource.Stop();
        audioSource.PlayOneShot(deathExplosion);
        engineThrust.Stop();
        deathEffect.Play();
    }

    void RespondToThrustInput()
	{
        if (Input.GetKey(KeyCode.Space)) // can thrust and rotate at the same time
        {
            ApplyThrustUp();
        }
        else
        {
            StopApplyingThrust();
        }
    }

    void StopApplyingThrust()
    {
        audioSource.Stop();
        engineThrust.Stop();
    }

    void ApplyThrustUp()
	{
        rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
            engineThrust.Play();
        }
        
    }

    void RespondToRotateInput()
	{
        rigidbody.angularVelocity = Vector3.zero; // remove rotation due to physics

        
        float rotationThisFrame = Time.deltaTime * rcsThrust;

        if (Input.GetKey(KeyCode.A))
		{
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
		{
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidbody.freezeRotation = false;
	}
}
