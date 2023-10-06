using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BB
{
	public class MenuUI : MonoBehaviour
	{
		public Text menuText;

        private void Awake()
        {
            ConfigurationManager.OnSyncComplete += ChangeMenuText; // Adding a listener to check when data is fetched
        }

        private void Start()
        {
            // Since we go back and forth in the scenes, we need to make sure the text stays updated with the value fetched from firebase
            if (ConfigurationManager.fetchSucceeded)
            {
                ChangeMenuText(ConfigurationManager.GetConfig<BBConfig>());
            }
        }

        //Called when the play button is pressed
        public void PlayButton()
		{
			SceneManager.LoadScene(1);  //Loads the 'Game' scene to begin the game
		}

		//Called when the quit button is pressed
		public void QuitButton()
		{
			Application.Quit();         //Quits the game
		}

        // Changing the menu text according to the task given
        private void ChangeMenuText(BBConfig conf)
        {
            if (conf != null)
            {
                menuText.text = conf.menuText;
            }
        }
    }
}
