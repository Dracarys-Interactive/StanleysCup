using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace so can reference UI elements
using UnityEngine.SceneManagement; // include so we can manipulate SceneManager
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

namespace DracarysInteractive.StanleysCup
{
    public class GameManager : MonoBehaviour
    {
        // static reference to game manager so can be called from other scripts directly (not just through gameobject component)
        public static GameManager gm;

        public GameObject playerPrefab;

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

        public bool enableDoubleJump = false;

        // private variables
        GameObject _player;
        Scene _scene;
        AudioSource audioSource;
        bool doPause = false;

        // set things up here
        void Awake()
        {
            // TODO: use Singleton
            if (gm == null)
                gm = this.GetComponent<GameManager>();
            /*
            if (_player == null)
                _player = GameObject.FindGameObjectWithTag("Player");

            if (_player == null)
                Debug.LogError("Player not found in Game Manager");

            */

            audioSource = GetComponent<AudioSource>();

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

                if (platforms.Length > currentLevel.platforms.Length / 2)
                {
                    _player = Instantiate(playerPrefab);

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
                        Vector2 playerOffset = new Vector2(0, 0.056f);
                        _player.transform.parent = closestToOrigin.transform;
                        _player.transform.localPosition = playerOffset;
                        _player.GetComponent<Animator>().SetBool("Grounded", true);
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
            StartLevel(currentLevel);
        }

        void StartLevel(LevelSO levelData)
        {
            if (!levelData)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }

            lives = levelData.lives;
            score = 0;
            enableDoubleJump = levelData.canDoubleJump;

            if (_player)
            {
                Destroy(_player.gameObject);
            }

            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            levelName.text = levelData.levelName;

            foreach(CollectableSO collectable in levelData.collectables)
            {
                GameObject go = new GameObject(collectable.spawnableName + " Spawner");
                go.transform.SetParent(transform);

                RandomSpawner randomSpawner = go.AddComponent<RandomSpawner>();
                randomSpawner.prefab = collectable.prefab;
                randomSpawner.maximumInstances = collectable.maximumInstances;
                randomSpawner.secondsBetweenSpawns = collectable.secondsBetweenSpawns;
                randomSpawner.spawnableSO = collectable;
            }

            foreach (PlatformSO platform in levelData.platforms)
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

            // update UI
            UIScore.text = "Score: " + score.ToString();

            // Check for victory.
            if (score >= currentLevel.pointsToAdvance)
                LevelComplete();
        }

        // public function to remove player life and reset game accordingly
        public void ResetGame()
        {
            lives--;
            refreshGUI();

            if (lives <= 0)
            {
                StartLevel(currentLevel);
            }
        }

        // public function for level complete
        public void LevelComplete()
        {
            StartLevel(currentLevel = currentLevel.nextLevel);
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
