using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Corsi_Ouputer.C_Files
{
    class MainClass
    {
        //Public variables that are altered in multiple functions.
        public static TeamClass team1, team2;
        public static List<string> team1PlayersForwards, team2PlayersForwards, team1PlayersDefense, team2PlayersDefense;

        static void Main(string[] args)
        {
            //Variable to output the information to excel.
            ExcelOutput excel = new ExcelOutput();
            //Variable for the folder.
            string folderName;

            //Gets the folder locations.
            Console.WriteLine("Please enter the folder location of the game files.");
            folderName = Console.ReadLine();

            //Checks to see if it's a directory or not, if not tells the user.
            if (!Directory.Exists(folderName))
            {
                Console.WriteLine("Directory does not exists");
            }
            else
            {
                //Gets all the files in the directory.
                string[] files = Directory.GetFiles(folderName);

                //For loop that goes for the number of files in the folder.
                for (int i = 0; i < files.Length; i++)
                {
                    Console.Clear();
                    Console.WriteLine("Currently going through game " + (i + 1));

                    //Creates a string array for the full file.
                    string[] txt = System.IO.File.ReadAllLines(files[i]);

                    //Creates a refresh of all the global variables.
                    team1PlayersForwards = new List<string>();
                    team2PlayersForwards = new List<string>();
                    team1PlayersDefense = new List<string>();
                    team2PlayersDefense = new List<string>();

                    team1 = new TeamClass();
                    team2 = new TeamClass();

                    //Runs through each line, and remembers the index of the line.
                    int txtIndex = 0;
                    foreach (string line in txt)
                    {
                        //Gets the name of the two teams.
                        if (line.Contains("<title>"))
                        {
                            SetTeamNames(line);
                        }
                        //Gets the player for each team.
                        else if (line.Contains("PlayerStatTitle"))
                        {
                            if (line.Contains(team1.GetTeamName()))
                            {
                                string[] txtPlayers = new string[20];
                                int x = txtIndex + 3;
                                int y = 0;
                                do
                                {
                                    y++;
                                    x++;
                                } while (!txt[x].Contains("</div>"));
                                Array.Copy(txt, txtIndex + 3, txtPlayers, 0, y);
                                SetPlayers(team1.GetTeamName(), txtPlayers, out team1);
                            }
                            else if (line.Contains(team2.GetTeamName()))
                            {
                                string[] txtPlayers = new string[18];
                                Array.Copy(txt, txtIndex + 3, txtPlayers, 0, 18);
                                SetPlayers(team2.GetTeamName(), txtPlayers, out team2);
                            }
                        }
                        //Gets the plays in the game.
                        else if (line.Contains("FullPlayByPlayPeriod") && !(line.Contains("ShootOut")))
                        {
                            RunThroughPBP(txt[txtIndex + 1]);
                        }
                        txtIndex++;
                    }

                    //Adds the two teams for each game.
                    excel.AddTeam(team1);
                    excel.AddTeam(team2);
                }
                Console.Clear();
                Console.Write("Writing out to excel");
                //Outputs the information to excel.
                excel.Output();

                Console.Clear();
                Console.WriteLine("Outputed to excel, all done");
            }
            Console.ReadKey();
        }


        //Gets the two team names from the a line in the file that is "<title>NHL - Game 1 - Team 1 vs Team 2</title>"
        static void SetTeamNames(string str)
        {
            string[] strNoSpace = str.Split(' ');

            strNoSpace[strNoSpace.Length - 1] = strNoSpace[strNoSpace.Length - 1].Replace("</title>", "");

            string team =  "";

            for(int i = 0; i < strNoSpace.Length; i++)
            {
                switch (strNoSpace[i])
                {
                    case "-":
                        break;
                    case "Game":
                        break;
                    default:
                        int x;
                        if(!int.TryParse(strNoSpace[i], out x))
                        {
                            if(strNoSpace[i] == "vs")
                            {
                                team1.SetTeamNames(team);
                                team = "";
                            }
                            else if(strNoSpace[i].Contains("<title>"))
                            {
                                //Do Nothing.
                            }
                            else
                            {
                                if(team == "")
                                {
                                    team = team + strNoSpace[i] + " ";
                                }
                                else
                                {
                                    team = team + strNoSpace[i];
                                }
                            }
                        }
                        break;
                }
            }
            team2.SetTeamNames(team);
        }

        //Sets the players using the stats line. Stats line looks like "Player Name              1  1  1   1  1   1  1  1  1  1  1/1    60:00   10:00   10:00"
        static void SetPlayers(string teamName, string[] playerLines, out TeamClass team)
        {
            team = new TeamClass();
            team.SetTeamNames(teamName);

            for(int i = 0; i < playerLines.Length; i++)
            {
                if (playerLines[i] != null)
                {
                    team.SetPlayerName(getPlayerName(playerLines[i]));
                }
            }
        }

        //Gets the player name from each line of the stats.
        static string getPlayerName(string line)
        {
            string playerName = "";
            string[] words = line.Split(' ');

            int x = 0;
            do
            {
                playerName = playerName + words[x] + " ";
                x++;
            } while (words[x] != "");

            playerName = playerName.TrimEnd(' ');

            return playerName;
        }

        //Run through the PBP of the game.
        static void RunThroughPBP(string str)
        {
            string[] pbpLines = str.Split('.');
            for(int i = 0; i < pbpLines.Length; i++)
            {
                //Gets what players are on the line.
                if (pbpLines[i].Contains("Line"))
                {
                    if (pbpLines[i].Contains(team1.GetTeamName()))
                    {
                        if (pbpLines[i].Contains("Forward"))
                        {
                            team1PlayersForwards.Clear();
                            team1PlayersForwards.AddRange(team1.CheckForPlayer(pbpLines[i]));
                        }
                        else if (pbpLines[i].Contains("Defense"))
                        {
                            team1PlayersDefense.Clear();
                            team1PlayersDefense.AddRange(team1.CheckForPlayer(pbpLines[i]));
                        }
                        else if (pbpLines[i].Contains("Offensive") || pbpLines[i].Contains("Defensive"))
                        {
                            team1PlayersDefense.Clear();
                            team1PlayersForwards.Clear();
                            team1PlayersForwards.AddRange(team1.CheckForPlayer(pbpLines[i]));
                            team1PlayersDefense.AddRange(team1.CheckForPlayer(pbpLines[i + 1]));
                        }
                    }
                    else if (pbpLines[i].Contains(team2.GetTeamName()))
                    {
                        if (pbpLines[i].Contains("Forward"))
                        {
                            team2PlayersForwards.Clear();
                            team2PlayersForwards.AddRange(team2.CheckForPlayer(pbpLines[i]));
                        }
                        else if (pbpLines[i].Contains("Defense"))
                        {
                            team2PlayersDefense.Clear();
                            team2PlayersDefense.AddRange(team2.CheckForPlayer(pbpLines[i]));
                        }
                        else if (pbpLines[i].Contains("Offensive") || pbpLines[i].Contains("Defensive"))
                        {
                            team2PlayersDefense.Clear();
                            team2PlayersForwards.Clear();
                            team2PlayersForwards.AddRange(team2.CheckForPlayer(pbpLines[i]));
                            team2PlayersDefense.AddRange(team2.CheckForPlayer(pbpLines[i + 1]));
                        }
                    }
                }
                else if (pbpLines[i].Contains("Shot by") && !pbpLines[i].Contains("Penalty"))
                {
                        if (team1.CheckIfOnTeam(pbpLines[i]))
                        {
                            team1.AddShots(team1PlayersForwards);
                            team1.AddShots(team1PlayersDefense);
                            team2.RemoveShots(team2PlayersForwards);
                            team2.RemoveShots(team2PlayersDefense);
                        }
                        else if (team2.CheckIfOnTeam(pbpLines[i]))
                        {
                            team2.AddShots(team2PlayersForwards);
                            team2.AddShots(team2PlayersDefense);
                            team1.RemoveShots(team1PlayersForwards);
                            team1.RemoveShots(team1PlayersDefense);
                        }
                }
            }
        }
    }
}
