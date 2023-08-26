using UnityEngine;
using UnityEngine.SceneManagement;

namespace DracarysInteractive.StanleysCup
{
    public class GameState : Singleton<GameState>
    {
        public string _levelName;
        public int? _levelScore;
        public int? _levelLivesLost;

        public string prefix;

        protected override void Awake()
        {
            base.Awake();
            prefix = Application.productName + "." + SceneManager.GetActiveScene().name + ".";
        }

        public string LevelName
        {
            set
            {
                PlayerPrefs.SetString(prefix + "LevelName", _levelName = value);
            }
            get
            {
                if (_levelName == null || _levelName.Length == 0)
                {
                    _levelName = PlayerPrefs.GetString(prefix + "LevelName", "None");
                }

                return _levelName;
            }
        }

        public int LevelScore
        {
            set
            {
                PlayerPrefs.SetInt(prefix + "LevelScore", (int)(_levelScore = value));
            }
            get
            {
                if (_levelScore == null)
                {
                    _levelScore = PlayerPrefs.GetInt(prefix + "LevelScore", 0);
                }

                return (int)_levelScore;
            }
        }

        public int LevelLivesLost
        {
            set
            {
                PlayerPrefs.SetInt(prefix + "LevelLivesLost", (int)(_levelLivesLost = value));
            }
            get
            {
                if (_levelLivesLost == null)
                {
                    _levelLivesLost = PlayerPrefs.GetInt(prefix + "LevelLivesLost", 0);
                }

                return (int)_levelLivesLost;
            }
        }

        public void Clear()
        {
            PlayerPrefs.DeleteKey(prefix + "LevelName");
            PlayerPrefs.DeleteKey(prefix + "LevelScore");
            PlayerPrefs.DeleteKey(prefix + "LevelLivesLost");

            _levelName = null;
            _levelScore = null;
            _levelLivesLost = null;
        }
    }
}

