using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class GameState : Singleton<GameState>
    {
        public string _levelName;
        public int? _levelScore;
        public int? _levelLivesLost;

        public string LevelName
        {
            set
            {
                PlayerPrefs.SetString("LevelName", _levelName = value);
            }
            get
            {
                if (_levelName == null)
                {
                    _levelName = PlayerPrefs.GetString("LevelName", "None");
                }

                return _levelName;
            }
        }

        public int LevelScore
        {
            set
            {
                PlayerPrefs.SetInt("LevelScore", (int)(_levelScore = value));
            }
            get
            {
                if (_levelScore == null)
                {
                    _levelScore = PlayerPrefs.GetInt("LevelScore", 0);
                }

                return (int)_levelScore;
            }
        }

        public int LevelLivesLost
        {
            set
            {
                PlayerPrefs.SetInt("LevelLivesLost", (int)(_levelLivesLost = value));
            }
            get
            {
                if (_levelLivesLost == null)
                {
                    _levelLivesLost = PlayerPrefs.GetInt("LevelLivesLost", 0);
                }

                return (int)_levelLivesLost;
            }
        }

        public void Clear()
        {
            PlayerPrefs.DeleteKey("LevelName");
            PlayerPrefs.DeleteKey("LevelScore");
            PlayerPrefs.DeleteKey("LevelLivesLost");
        }
    }
}

