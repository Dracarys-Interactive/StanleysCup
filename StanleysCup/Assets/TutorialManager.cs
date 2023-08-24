using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DracarysInteractive.StanleysCup
{
    public class TutorialManager : MonoBehaviour
    {
        public TextMeshProUGUI tutorialText;
        public string[] tutorials;
        public int lastTutorialIndex = -1;
        public int currentTutorialIndex = -1;

        private void Start()
        {
            Next();
        }

        public void Next()
        {
            if (currentTutorialIndex + 1 >= tutorials.Length)
            {
                lastTutorialIndex = currentTutorialIndex = -1;
                Next();
            }
            else
            {
                lastTutorialIndex = currentTutorialIndex++;
                tutorialText.text = tutorials[currentTutorialIndex];
            }
        }

        public void Play()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
