using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maggots
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField] private UIController uiController;
        [SerializeField] private InputSystem inputSystem;
        [SerializeField] private ArenaData arenaData;

        private void Awake()
        {
            uiController.Init();
            arenaData.InputSystem = inputSystem;
            inputSystem.Init();
            SceneManager.LoadScene("MaggotsAssets/Scenes/Main", LoadSceneMode.Single);
        }
    }

}
