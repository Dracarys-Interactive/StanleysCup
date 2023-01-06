using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace so can reference UI elements
using UnityEngine.SceneManagement; // include so we can manipulate SceneManager
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{

    // static reference to game manager so can be called from other scripts directly (not just through gameobject component)
    public static GameManager gm;

    // levels to move to on victory and lose
    public string levelAfterVictory;
    public string levelAfterGameOver;
    public int victoryScore;

    // game performance
    public int score = 0;
    public int startLives = 3;
    public int lives = 3;

    // UI elements to control
    public TextMeshProUGUI UIScore;
    public TextMeshProUGUI UILevel;
    public TextMeshProUGUI UISplash;
    public GameObject[] UIExtraLives;
    public float splashFade = 0.005f;
    public GameObject miniMap;

    public bool enableDoubleJump = false;

    // private variables
    GameObject _player;
    Scene _scene;
    AudioSource audioSource;
    bool doPause = false;

    // set things up here
    void Awake()
    {
        // setup reference to game manager
        if (gm == null)
            gm = this.GetComponent<GameManager>();

        // setup all the variables, the UI, and provide errors if things not setup properly.
        setupDefaults();

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        doPause = context.performed;
    }

    // game loop
    void Update()
    {
        // if pause key pressed then pause the game
        if (doPause)
        {
            if (Time.timeScale > 0f)
            {
                Time.timeScale = 0f; // this pauses the game action
                Color c = UISplash.color;
                c.a = 1.0f;
                UISplash.color = c;
            }
            else
            {
                Time.timeScale = 1f; // this unpauses the game action (ie. back to normal)
            }

            doPause = false;
        }

        if (Time.timeScale > 0f && UISplash.color.a > 0)
        {
            Color c = UISplash.color;
            c.a -= splashFade;
            UISplash.color = c;
        }
    }

    // setup all the variables, the UI, and provide errors if things not setup properly.
    void setupDefaults()
    {
        // setup reference to player
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player");


        if (_player == null)
            Debug.LogError("Player not found in Game Manager");

        // get current scene
        _scene = SceneManager.GetActiveScene();

        // if levels not specified, default to current level
        if (levelAfterVictory == "")
        {
            Debug.LogWarning("levelAfterVictory not specified, defaulted to current level");
            levelAfterVictory = _scene.name;
        }

        if (levelAfterGameOver == "")
        {
            Debug.LogWarning("levelAfterGameOver not specified, defaulted to current level");
            levelAfterGameOver = _scene.name;
        }

        /*
		// friendly error messages
		if (UIScore==null)
			Debug.LogError ("Need to set UIScore on Game Manager.");
		
		if (UILevel==null)
			Debug.LogError ("Need to set UILevel on Game Manager.");
		*/

        // get the UI ready for the game
        refreshGUI();
    }

    // refresh all the GUI elements
    void refreshGUI()
    {
        if (!UIScore || !UILevel)
            return;

        // set the text elements of the UI
        UIScore.text = "Score: " + score.ToString();
        UILevel.text = _scene.name;

        // turn on the appropriate number of life indicators in the UI based on the number of lives left
        for (int i = 0; i < UIExtraLives.Length; i++)
        {
            if (i < (lives - 1))
            { // show one less than the number of lives since you only typically show lifes after the current life in UI
                UIExtraLives[i].SetActive(true);
            }
            else
            {
                UIExtraLives[i].SetActive(false);
            }
        }
    }

    // public function to add points and update the gui and highscore player prefs accordingly
    public void AddPoints(int amount)
    {
        // increase score
        score += amount;

        // update UI
        UIScore.text = "Score: " + score.ToString();

        // Check for victory.
        if (score >= victoryScore)
            LevelComplete();
    }

    // public function to remove player life and reset game accordingly
    public void ResetGame()
    {
        // remove life and update GUI
        lives--;
        refreshGUI();

        if (lives <= 0)
        { // no more lives
          // load the gameOver screen
            SceneManager.LoadScene(levelAfterGameOver);
        }
    }

    // public function for level complete
    public void LevelComplete()
    {
        // use a coroutine to allow the player to get fanfare before moving to next level
        StartCoroutine(LoadNextLevel());
    }

    // load the nextLevel after delay
    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene(levelAfterVictory);
    }

    public void TogglePause()
    {
        if (Time.timeScale > 0)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void ToggleMiniMap()
    {
        miniMap.SetActive(!miniMap.activeSelf);
    }
}
