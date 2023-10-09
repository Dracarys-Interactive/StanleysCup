using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using Cinemachine;
using UnityEngine.Rendering.Universal;
using System;

namespace DracarysInteractive.StanleysCup
{
    public class GameManager : Singleton<GameManager>
    {
        public GameObject playerPrefab;
        public CinemachineVirtualCamera followCam;

        public LevelSO currentLevel;
        public TextMeshProUGUI levelName;

        public int score = 0;
        public int lives = 0;

        public TextMeshProUGUI UIScore;
        public TextMeshProUGUI UILevel;
        public TextMeshProUGUI UISplash;
        public GameObject[] UIExtraLives;
        public float splashFade = 0.005f;
        public GameObject miniMap;
        public Button miniMapToggleButton;

        public bool enableDoubleJump = false;

        public bool resetGameState = false;

        public bool paused = false;
        public Sprite[] pausePlaySprites;
        public Image pausePlayImage;

        public GameObject[] tutorialOnlyGameObjects;

        private GameObject _player;
        private Scene _scene;
        private AudioSource _audioSource;
        private bool _doPause = false;

        protected override void Awake()
        {
            base.Awake();

            if (resetGameState)
            {
                GameState.Instance.Clear();
            }

            _audioSource = GetComponent<AudioSource>();

            if (!currentLevel.isTutorial)
            {
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
            }

            refreshGUI();
        }

        public void ResumeLastLevel()
        {
            string levelName = GameState.Instance.LevelName;
            LevelSO level = Resources.Load("Levels/" + levelName) as LevelSO;

            if (!level)
            {
                level = currentLevel = currentLevel.nextLevel;

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

            StartLevel();
        }

        public void PlaySound(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip);
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            _doPause = context.performed;
        }

        void Update()
        {
            if (!_player)
            {
                InstantiatePlayer();
            }

            if (_doPause)
            {
                if (Time.timeScale > 0f)
                {
                    Time.timeScale = 0f;
                    Color c = UISplash.color;
                    c.a = 1.0f;
                    UISplash.color = c;
                }
                else
                {
                    Time.timeScale = 1f;
                }

                _doPause = false;
            }

            if (Time.timeScale > 0f && UISplash.color.a > 0)
            {
                Color c = UISplash.color;
                c.a -= splashFade;
                UISplash.color = c;
            }
        }

        private void InstantiatePlayer()
        {
            Platform platform = findPlatformClosestToOrigin();

            if (platform)
            {
                _player = Instantiate(playerPrefab);
                _player.transform.parent = platform.transform;
                _player.transform.localPosition = new Vector2(0, 0.4f);
                _player.GetComponent<Animator>().SetBool("Grounded", true);
                _player.GetComponent<Light2D>().enabled = currentLevel.useSpotLight;

                followCam.Follow = _player.transform;
            }
        }

        private void Start()
        {
            ParticleSystem snow = FindObjectOfType<ParticleSystem>();
            var emission = snow.emission;
            emission.rateOverTime = 20;

            StartLevel();
        }

        void StartLevel()
        {
            Destroy(_player);

            if (!currentLevel.isTutorial)
            {
                foreach (GameObject go in tutorialOnlyGameObjects)
                {
                    go.SetActive(false);
                }
            }

            miniMapToggleButton.gameObject.SetActive(currentLevel.hasMiniMap);
            enableDoubleJump = currentLevel.canDoubleJump;

            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            levelName.text = currentLevel.levelName;

            if (currentLevel.collectables != null)
            {
                foreach (CollectableSO collectable in currentLevel.collectables)
                {
                    GameObject go = new GameObject(collectable.spawnableName + " Spawner");
                    go.transform.SetParent(transform);

                    RandomSpawner randomSpawner = go.AddComponent<RandomSpawner>();
                    randomSpawner.prefab = collectable.prefab;
                    randomSpawner.maximumInstances = collectable.maximumInstances;
                    randomSpawner.secondsBetweenSpawns = collectable.secondsBetweenSpawns;
                    randomSpawner.spawnableSO = collectable;
                }
            }

            if (currentLevel.platforms != null)
            {
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
            }

            if (currentLevel.enemies != null)
            {
                foreach (EnemySO enemy in currentLevel.enemies)
                {
                    GameObject go = new GameObject(enemy.spawnableName + " Spawner");
                    go.transform.SetParent(transform);

                    RandomSpawner randomSpawner = go.AddComponent<RandomSpawner>();
                    randomSpawner.prefab = enemy.prefab;
                    randomSpawner.maximumInstances = enemy.maximumInstances;
                    randomSpawner.secondsBetweenSpawns = enemy.secondsBetweenSpawns;
                    randomSpawner.spawnableSO = enemy;
                }
            }

            refreshGUI();
        }

        void refreshGUI()
        {
            UIScore.text = "Score: " + score.ToString() + "/" + currentLevel.pointsToAdvance;

            for (int i = 0; i < UIExtraLives.Length; i++)
            {
                if (i < (lives - 1))
                {
                    UIExtraLives[i].SetActive(true);
                }
                else
                {
                    UIExtraLives[i].SetActive(false);
                }
            }
        }

        public void AddPoints(int amount)
        {
            score += amount;

            GameState.Instance.LevelScore = score;

            UIScore.text = "Score: " + score.ToString() + "/" + currentLevel.pointsToAdvance;

            if (score >= currentLevel.pointsToAdvance)
                LevelComplete();
        }

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

        private Platform findPlatformClosestToOrigin()
        {
            Platform closestToOrigin = null;
            float minDistance = float.MaxValue;

            foreach (Platform platform in GameObject.FindObjectsOfType<Platform>())
            {
                float distanceFromOrigin = Vector2.Distance(platform.transform.position, Vector2.zero);

                if (distanceFromOrigin < minDistance && currentLevel.playerSpawningRect.Contains(platform.transform.position))
                {
                    closestToOrigin = platform;
                    minDistance = distanceFromOrigin;
                }
            }

            return closestToOrigin;
        }

        public void LevelComplete()
        {
            if (currentLevel.isTutorial)
            {
                score = 0;
                lives = currentLevel.lives;
            }
            else
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
            }

            StartLevel();
        }

        public void TogglePause()
        {
            if (!paused)
            {
                Time.timeScale = 0;
                _audioSource.Pause();
            }
            else
            {
                Time.timeScale = 1;
                _audioSource.UnPause();
            }

            paused = !paused;

            pausePlayImage.sprite = pausePlaySprites[paused ? 0 : 1];
        }

        public void ToggleMiniMap()
        {
            miniMap.SetActive(!miniMap.activeSelf);
        }

        public void NextLevel()
        {
            if (Debug.isDebugBuild && currentLevel.nextLevel)
            {
                LevelComplete();
            }
        }

        public void PrevLevel()
        {
            if (Debug.isDebugBuild && currentLevel.prevLevel)
            {
                currentLevel = currentLevel.prevLevel;

                GameState.Instance.LevelName = currentLevel.name;
                GameState.Instance.LevelScore = 0;
                GameState.Instance.LevelLivesLost = 0;

                score = 0;
                lives = currentLevel.lives;

                StartLevel();
            }
        }
    }
}
