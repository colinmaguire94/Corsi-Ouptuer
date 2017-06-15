using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corsi_Ouputer.C_Files
{
    class TeamClass
    {
        private string TeamName;
        //Dictionary for the shots when a player is on the ice.
        private Dictionary<string, int> Players;
        //Dictionary for the shots when a player isn't on the ice.
        private Dictionary<string, int> ShotsNotOnIce;

        public TeamClass()
        {
            TeamName = "";
            Players = new Dictionary<string, int>();
            ShotsNotOnIce = new Dictionary<string, int>();
        }

        //Sets the team name.
        public void SetTeamNames(string str)
        {
            TeamName = str;
        }

        //Gets the team name.
        public string GetTeamName()
        {
            return TeamName;
        }

        //Sets the key for both dictionaries.
        public void SetPlayerName(string name)
        {
            Players.Add(name, 0);
            ShotsNotOnIce.Add(name, 0);
        }

        //Calculates the relative corsi.
        public int CalulateCorsiRelative(string name)
        {
            return Players[name] - ShotsNotOnIce[name];
        }

        //Checks for if the player is in the dictionary.
        public List<string> CheckForPlayer(string line)
        {
            List<string> playersOn = new List<string>();
            for(int i = 0; i < Players.Count; i++)
            {
                if(line.Contains(Players.Keys.ElementAt(i)))
                {
                    playersOn.Add(Players.Keys.ElementAt(i));
                }
            }
            return playersOn;
        }

        //Check if the player is on the team.
        public bool CheckIfOnTeam(string line)
        {
            for(int i = 0; i < Players.Count; i++)
            {
                if(line.Contains(Players.Keys.ElementAt(i)))
                {
                    return true;
                }
            }
            return false;
        }

        //Adds shots for each player on the ice, and the shots not on for those not on the ice.
        public void AddShots(List<string> playerShots)
        {
            for(int i = 0; i < playerShots.Count; i++)
            {
                Players[playerShots[i]]++;
            }

            for(int i = 0; i < ShotsNotOnIce.Count; i++)
            {
                if(!playerShots.Contains(ShotsNotOnIce.Keys.ElementAt(i)))
                {
                    ShotsNotOnIce[ShotsNotOnIce.Keys.ElementAt(i)]++;
                }
            }
        }

        //Adds the shots against for both on and off ice.
        public void RemoveShots(List<string> playerShots)
        {
            for(int i = 0; i < playerShots.Count; i++)
            {
                Players[playerShots[i]]--;
            }

            for (int i = 0; i < ShotsNotOnIce.Count; i++)
            {
                if (!playerShots.Contains(ShotsNotOnIce.Keys.ElementAt(i)))
                {
                    ShotsNotOnIce[ShotsNotOnIce.Keys.ElementAt(i)]--;
                }
            }
        }

        //Gets the number of players.
        public int NumberOfPlayers()
        {
            return Players.Count();
        }

        public bool CheckForPlayerOnTeam(string name)
        {
            return Players.ContainsKey(name);
        }

        public string GetPlayerAt(int index)
        {
            return Players.Keys.ElementAt(index);
        }

        //Add shots.
        public void AddShots(string key, int shots, int shotsNotOn)
        {
            Players[key] += shots;
            ShotsNotOnIce[key] += shotsNotOn;
        }

        //Gets shots for.
        public int GetShots(int index)
        {
            return Players.Values.ElementAt(index);
        }
        
        //Gets shots not on ice.
        public int GetShotsNotOn(int index)
        {
            return ShotsNotOnIce.Values.ElementAt(index);
        }

        //Adds player and the shots.
        public void AddPlayer(string name, int shots, int shotsNotOnIce)
        {
            Players.Add(name, shots);
            ShotsNotOnIce.Add(name, shotsNotOnIce);
        }

        //Gets the number of players.
        public int GetNumberOfPlayers()
        {
            return Players.Count;
        }
    }
}
