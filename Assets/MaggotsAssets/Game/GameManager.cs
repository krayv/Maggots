using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Maggots
{
    public class GameManager : MonoBehaviour
    {

        private List<Vector2> spawnPoints;

        [SerializeField] private Terrain terrain;
        [SerializeField] private int charactersCount = 2;
        [SerializeField] private Maggot playerPrefab;
        [SerializeField] private InputSystem inputSystem;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private PlayerController playerController;

        private readonly List<Maggot> spawnedPlayers = new();

        private int currentEntityIndex = 0;
        private Maggot CurrentEntity => spawnedPlayers[currentEntityIndex];

        public Action<Maggot> OnChangeSelectedEntity;

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
                Maggot maggot = SpawnPlayer(spawnPoints[i]);
                maggot.Init(this);
                maggot.OnDeath += OnPlayerDeath;
                spawnedPlayers.Add(maggot);
            }
        }

        private void SwitchToNewPlayer()
        {
            OnChangeSelectedEntity.Invoke(CurrentEntity);
            playerController.TrackNewMovement(new List<Maggot>() { CurrentEntity });
            TrackNewObject(CurrentEntity.gameObject);
        }

        private void TrackNewObject(GameObject newTrackObject)
        {
            cameraController.TrackNewObject(newTrackObject);
        }

        private Maggot SpawnPlayer(Vector2 position)
        {
            Maggot player = GameObject.Instantiate(playerPrefab);
            player.transform.position = position;
            return player;
        }

        private void OnPlayerDeath(Maggot maggot)
        {
            maggot.OnDeath -= OnPlayerDeath;
            if (spawnedPlayers.IndexOf(maggot) == currentEntityIndex)
            {
                NextPlayer();
            }
            spawnedPlayers.Remove(maggot);
        }
    }
}
