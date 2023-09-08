using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maggots
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField] private UIController uiController;

        private void Awake()
        {
            uiController.Init();
            SceneManager.LoadScene("MaggotsAssets/Scenes/Main", LoadSceneMode.Single);
        }
    }

}
