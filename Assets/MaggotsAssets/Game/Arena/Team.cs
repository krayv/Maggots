using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maggots
{
    public class Team
    {
        public int Number;
        public int CharacterCounts;
        public int HealthPerCharacter;

        public string TeamName;
        public Color TeamColor = Color.white;
        private int currentMaggotIndex;
        private List<Maggot> maggots;

        private Inventory _inventory;

        public Inventory Inventory
        {
            get
            {
                return _inventory;
            }
        }
        public List<Maggot> Maggots
        {
            get
            {
                return maggots;
            }
        }

        public Team(string teamName, Dictionary<Weapon, int> startInventory)
        {

            this.TeamName = teamName;
            _inventory = new(startInventory);
        }

        public void SetSpawnedMaggots(List<Maggot> maggots)
        {
            this.maggots = maggots;
        }

        public Maggot CurrentMaggot()
        {
            return maggots[currentMaggotIndex];
        }

        public Maggot GetNextMaggot()
        {
            if (currentMaggotIndex >= maggots.Count - 1)
            {
                currentMaggotIndex = 0;
            }
            else
            {
                currentMaggotIndex++;
            }
            return maggots[currentMaggotIndex];
        }

        public void OnPlayerDeath(Maggot maggot)
        {
            maggots.Remove(maggot);
            maggot.OnDeath -= OnPlayerDeath;
        }

        public void DestroyAllMaggots()
        {
            foreach (var maggot in maggots)
            {
               GameObject.Destroy(maggot.gameObject);
            }
            maggots.Clear();
        }
    }
}
