using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maggots
{
    public class Team
    {
        public int number;
        public string TeamName => "Player " + number;
        public Color TeamColor = Color.white;
        private int currentMaggotIndex;
        private readonly List<Maggot> maggots;

        public List<Maggot> Maggots
        {
            get
            {
                return maggots;
            }
        }

        public Team(List<Maggot> maggots, int number)
        {
            this.maggots = maggots;
            this.number = number;
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
