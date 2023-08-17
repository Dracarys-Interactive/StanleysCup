using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace so can reference UI elements
using UnityEngine.SceneManagement; // include so we can manipulate SceneManager
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.Rendering.Universal;

namespace DracarysInteractive.StanleysCup
{
    public class GameManager : Singleton<GameManager>
    {
        public GameObject playerPrefab;
        public int minPlatformsToSpawnPlayer = 2;
        public CinemachineVirtualCamera followCam;

        public LevelSO currentLevel;
        public TextMeshProUGUI levelName;

        // game performance
        public int score = 0;
        public int lives = 0;

        // UI elements to control
        public TextMeshProUGUI UIScore;
        public TextMeshProUGUI UILevel;
        public TextMeshProUGUI UISplash;
        public GameObject[] UIExtraLives;
        public float splashFade = 0.005f;
        public GameObject miniMap;
        public Button miniMapToggleButton;

        public bool enableDoubleJump = false;

        public bool resetGameState = false;

        // private variables
        GameObject _player;
        Scene _scene;
        AudioSource audioSource;
        bool doPause = false;

        // set things up here
        protected override void Awake()
        {
            base.Awake();

            if (resetGameState)
            {
                GameState.Instance.Clear();
            }

            audioSource = GetComponent<AudioSource>();

            string levelName = GameState.Instance.LevelName;
            LevelSO level = Resources.Load("Levels/" + levelName + "/" + levelName) as LevelSO;

            if (!level)
            {
                GameState.Instance.LevelName = currentLevel.name;
                GameState.Instance.LevelScore = 0; 
                GameState.Instance.LevelLivesLost = 0;

                score = 0;
                lives = currentLevel.lives;
            }
            else
            {
                currentLevel = level;
                score = GameState.Instance.LevelScore;
                lives = currentLevel.lives - GameState.Instance.LevelLivesLost;
            }

            refreshGUI();
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
            if (_player == null)
            {
                GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");

                if (platforms.Length >= minPlatformsToSpawnPlayer)
                {
                    _player = Instantiate(playerPrefab);
                    _player.GetComponent<Light2D>().enabled = currentLevel.useSpotLight;

                    GameObject closestToOrigin = null;
                    float minDistance = 0;

                    foreach (GameObject platform in platforms)
                    {
                        float distance = Vector2.Distance(Vector2.zero, platform.gameObject.transform.position);

                        if (closestToOrigin == null || distance < minDistance)
                        {
                            closestToOrigin = platform;
                            minDistance = distance;
                        }
                    }

                    if (closestToOrigin)
                    {
                        Vector2 playerOffset = new Vector2(0, 1f); // new Vector2(0, 0.056f);
                        _player.transform.parent = closestToOrigin.transform;
                        _player.transform.localPosition = playerOffset;
                        _player.GetComponent<Animator>().SetBool("Grounded", true);
                        followCam.Follow = _player.transform;
                    }
                }
            }
            
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

        private void Start()
        {
            StartLevel();
        }

        void StartLevel()
        {
            miniMapToggleButton.gameObject.SetActive(currentLevel.hasMiniMap);
            enableDoubleJump = currentLevel.canDoubleJump;

            if (_player)
            {
                Destroy(_player.gameObject);
            }

            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            levelName.text = currentLevel.levelName;

            foreach(CollectableSO collectable in currentLevel.collectables)
            {
                GameObject go = new GameObject(collectable.spawnableName + " Spawner");
                go.transform.SetParent(transform);

                RandomSpawner randomSpawner = go.AddComponent<RandomSpawner>();
                randomSpawner.prefab = collectable.prefab;
                randomSpawner.maximumInstances = collectable.maximumInstances;
                randomSpawner.secondsBetweenSpawns = collectable.secondsBetweenSpawns;
                randomSpawner.spawnableSO = collectable;
            }

            foreach (PlatformSO platform in currentLevel.platforms)
            {
                GameObject go = new GameObject(platform.spawnableName + " Spawner");
                go.transform.SetParent(transform);

                RandomSpawner randomSpawner = go.AddComponent<RandomSpawner>();
                randomSpawner.prefab = platform.prefab;
                randomSpawner.maximumInstances = platform.maximumInstances;
                randomSpawner.secondsBetweenSpawns = platform.secondsBetweenSpawns;
                randomSpawner.spawnableSO = platform;
            }

            ParticleSystem snow = FindObjectOfType<ParticleSystem>();
            var emission = snow.emission;
            emission.rateOverTime = 20;

            refreshGUI();
        }

        // refresh all the GUI elements
        void refreshGUI()
        {
            if (!UIScore || !UILevel)
                return;

            // set the text elements of the UI
            UIScore.text = "Score: " + score.ToString();

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
            GameState.Instance.LevelScore = score;

            // update UI
            UIScore.text = "Score: " + score.ToString();

            // Check for victory.
            if (score >= currentLevel.pointsToAdvance)
                LevelComplete();
        }

        // public function to remove player life and reset game accordingly
        public void ResetGame()
        {
            Destroy(_player);
            lives--;
            GameState.Instance.LevelLivesLost++;

            if (lives <= 0)
            {
                GameState.Instance.LevelScore = 0;
                GameState.Instance.LevelLivesLost = 0;
                score = 0;
                lives = currentLevel.lives;
                StartLevel();
            }

            refreshGUI();
        }

        // public function for level complete
        public void LevelComplete()
        {
            currentLevel = currentLevel.nextLevel;

            if (!currentLevel)
            {
                GameState.Instance.Clear();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                GameState.Instance.LevelName = currentLevel.name;
                GameState.Instance.LevelScore = 0;
                GameState.Instance.LevelLivesLost = 0;

                score = 0;
                lives = currentLevel.lives;
            }

            StartLevel();
        }

        public void TogglePause()
        {
            if (Time.timeScale > 0)
            {
                Time.timeScale = 0;
                audioSource.Pause();
            }
            else
            {
                Time.timeScale = 1;
                audioSource.UnPause();
            }
        }

        public void ToggleMiniMap()
        {
            miniMap.SetActive(!miniMap.activeSelf);
        }
    }
}
