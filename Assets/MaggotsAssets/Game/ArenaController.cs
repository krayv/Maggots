using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

namespace Maggots
{
    public class ArenaController : MonoBehaviour
    {
        private List<Vector2> spawnPoints;

        [SerializeField] private Terrain terrain;
        [SerializeField] private int charactersCount = 2;
        [SerializeField] private Maggot playerPrefab;
        [SerializeField] private InputSystem inputSystem;       
        [SerializeField] private PlayerController playerController;
        [SerializeField] private BattleStarter battleStarter;
        [SerializeField] private ArenaData arenaData;

        private CameraController cameraController;
        private readonly List<Maggot> spawnedPlayers = new();

        private int currentEntityIndex = 0;
        private Maggot CurrentEntity => spawnedPlayers[currentEntityIndex];

        public Action<Maggot> OnChangeSelectedEntity;

        private void Start()
        {
            cameraController = battleStarter.cameraController;
            inputSystem.Init();
            playerController.Init(inputSystem);
            GenerateTerrain();
            SwitchToNewPlayer();
            arenaData.arenaController = this;
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

        public void LeaveBattle()
        {
            var task = SceneManager.UnloadSceneAsync(battleStarter.ArenaScene);
            currentEntityIndex = 0;
            foreach (var player in spawnedPlayers)
            {
                Destroy(player.gameObject);
            }
        }

        private void GenerateTerrain()
        {
            spawnPoints = terrain.Generate(battleStarter);
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
