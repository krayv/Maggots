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
        [SerializeField] private Maggot playerPrefab;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private BattleStarter battleStarter;
        [SerializeField] private ArenaData arenaData;
        [SerializeField] private float nextTurnDelay = .5f;

        private CameraController cameraController;
        private readonly List<Team> teams = new();

        private int currentTeamIndex = 0;
        private InputSystem inputSystem;
        public Team CurrentTeam => teams[currentTeamIndex];

        public Action<Team> OnChangeTeam;
        public Action<Maggot> OnChangeSelectedMaggot;

        private void Start()
        {
            inputSystem = arenaData.InputSystem;
            cameraController = battleStarter.cameraController;
            playerController.Init(inputSystem);
            GenerateTerrain();
            SwitchToNewTeam(false);
            arenaData.ArenaController = this;
            arenaData.Teams = teams;
            arenaData.CameraController = cameraController;
            arenaData.OnNewBattle?.Invoke();
        }

        public void NextTurn()
        {
            currentTeamIndex++;
            if (currentTeamIndex >= teams.Count)
            {
                currentTeamIndex = 0;
            }
            SwitchToNewTeam(true, true);
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
            List<int> usedSpawnPoints = new List<int>(); 
            for (int i = 0; i < battleStarter.Teams.Count; i++)
            {
                List<Maggot> maggots = new();
                
                Team team = battleStarter.Teams[i];
                for (int j = 0; j < team.CharacterCounts; j++)
                {
                    int indexPos = SelectRandomPointIndex(usedSpawnPoints);
                    usedSpawnPoints.Add(indexPos);
                    Maggot maggot = SpawnPlayer(spawnPoints[indexPos]);
                    maggot.Init(this, team);
                    maggot.OnDeath += OnPlayerDeath;
                    maggot.OnDeath += team.OnPlayerDeath;
                    maggots.Add(maggot);
                }
                team.SetSpawnedMaggots(maggots);
                teams.Add(team);
            }            
        }

        private int SelectRandomPointIndex(List<int> usedSpawnPoints)
        {
            int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            if (usedSpawnPoints.Count == 0 || usedSpawnPoints.Count >= spawnPoints.Count)
            {
                return randomIndex;
            }
            while (usedSpawnPoints.Any(i=>i == randomIndex))
            {
                randomIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            }
            return randomIndex;
        }

        private void SwitchToNewTeam(bool withDelay, bool nextMaggot = false)
        {
            Maggot currentMaggot = nextMaggot ? CurrentTeam.GetNextMaggot() : CurrentTeam.CurrentMaggot();
            playerController.TrackNewMovement(new List<Maggot>());
            StartCoroutine(DelayedSwitchingToNextPlayer(currentMaggot, withDelay? nextTurnDelay : 0f));
        }

        private IEnumerator DelayedSwitchingToNextPlayer(Maggot maggot, float delay)
        {
            yield return new WaitForSeconds(delay);
            playerController.TrackNewMovement(new List<Maggot>() { maggot });
            maggot.OnEndTurn += OnPlayerEndTurn;
            TrackNewObject(maggot.gameObject);
            OnChangeSelectedMaggot.Invoke(maggot);
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
