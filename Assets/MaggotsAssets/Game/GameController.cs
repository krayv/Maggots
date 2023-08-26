using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Maggots
{
    public class GameController : MonoBehaviour
    {

        private List<Vector2> spawnPoints;

        [SerializeField] private Terrain terrain;
        [SerializeField] private int charactersCount = 2;
        [SerializeField] private MaggotEntity playerPrefab;
        [SerializeField] private InputSystem inputSystem;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private PlayerController playerController;

        private readonly List<MaggotEntity> spawnedPlayers = new();

        private int currentEntityIndex = 0;
        private MaggotEntity CurrentEntity => spawnedPlayers[currentEntityIndex];

        private void Start()
        {
            inputSystem.Init();
            playerController.Init(inputSystem);
            GenerateTerrain();
            SwitchToNewPlayer();
        }

        public void NextPlayer()
        {
            if (currentEntityIndex >= spawnedPlayers.Count - 1)
            {
                currentEntityIndex = 0;             
            }
            else
            {
                currentEntityIndex++;
            }
            SwitchToNewPlayer();
        }

        private void GenerateTerrain()
        {
            spawnPoints = terrain.Generate();
            SpawnPlayers();
        }

        private void SpawnPlayers()
        {
            for (int i = 0; i < charactersCount; i++)
            {
                spawnedPlayers.Add(SpawnPlayer(spawnPoints[i]));
            }
        }

        private void SwitchToNewPlayer()
        {
            playerController.TrackNewMovement(new List<RigidbodyMovement>() { CurrentEntity.RigidbodyMovement });
            TrackNewObject(CurrentEntity.gameObject);
        }

        private void TrackNewObject(GameObject newTrackObject)
        {
            cameraController.TrackNewObject(newTrackObject);
        }

        private MaggotEntity SpawnPlayer(Vector2 position)
        {
            MaggotEntity player = GameObject.Instantiate(playerPrefab);
            player.transform.position = position;
            return player;
        }
    }
}
