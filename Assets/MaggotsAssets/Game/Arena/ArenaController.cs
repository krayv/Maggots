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
        [SerializeField] private int teamsCount = 2;
        [SerializeField] private int maggotsPerTeam = 2;
        [SerializeField] private Maggot playerPrefab;
        [SerializeField] private InputSystem inputSystem;       
        [SerializeField] private PlayerController playerController;
        [SerializeField] private BattleStarter battleStarter;
        [SerializeField] private ArenaData arenaData;

        private CameraController cameraController;
        private readonly List<Team> teams = new();

        private int currentTeamIndex = 0;
        private Team CurrentTeam => teams[currentTeamIndex];

        public Action<Team> OnChangeTeam;
        public Action<Maggot> OnChangeSelectedMaggot;

        private void Start()
        {
            cameraController = battleStarter.cameraController;
            inputSystem.Init();
            playerController.Init(inputSystem);
            GenerateTerrain();
            SwitchToNewTeam();
            arenaData.arenaController = this;
        }

        public void NextTurn()
        {
            currentTeamIndex++;
            if (currentTeamIndex >= teams.Count)
            {
                currentTeamIndex = 0;
            }
            SwitchToNewTeam();
        }

        public void LeaveBattle()
        {
            var task = SceneManager.UnloadSceneAsync(battleStarter.ArenaScene);
            teams.ForEach(t => t.DestroyAllMaggots());
            currentTeamIndex = 0;
        }

        private void GenerateTerrain()
        {
            spawnPoints = terrain.Generate(battleStarter);
            SpawnPlayers();
        }

        private void SpawnPlayers()
        {
            for (int i = 0; i < teamsCount; i++)
            {
                List<Maggot> maggots = new();
                Team team = new(maggots, i);
                for (int j = 0; j < maggotsPerTeam; j++)
                {
                    Maggot maggot = SpawnPlayer(spawnPoints[i]);
                    maggot.Init(this);
                    maggot.OnDeath += OnPlayerDeath;
                    maggot.OnDeath += team.OnPlayerDeath;
                    maggots.Add(maggot);
                }
                teams.Add(team);
            }            
        }

        private void SwitchToNewTeam(bool nextMaggot = false)
        {
            Maggot currentMaggot = nextMaggot ? CurrentTeam.GetNextMaggot() : CurrentTeam.CurrentMaggot();
            playerController.TrackNewMovement(new List<Maggot>() { currentMaggot });
            currentMaggot.OnEndTurn += OnPlayerEndTurn;
            TrackNewObject(currentMaggot.gameObject);
            OnChangeSelectedMaggot.Invoke(currentMaggot);
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

        private void OnPlayerEndTurn(Maggot maggot)
        {
            maggot.OnEndTurn -= OnPlayerEndTurn;
            NextTurn();
        }

        private void OnPlayerDeath(Maggot maggot)
        {
            maggot.OnDeath -= OnPlayerDeath;
        }
    }
}
