using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.IO;

namespace Corsi_Ouputer.C_Files
{
    class ExcelOutput
    {
        List<TeamClass> teams;

        public ExcelOutput()
        {
            teams = new List<TeamClass>();
        }

        //Adds a team to the list.
        public void AddTeam(TeamClass team)
        {
            if(teams.Count == 0)
            {
                teams.Add(team);
                return;
            }
            else
            {
                for(int i = 0; i < teams.Count; i++)
                {
                    if(teams[i].GetTeamName() == team.GetTeamName())
                    {
                        AddExistingTeam(team, i);
                        i = teams.Count;
                    }
                    if(i == teams.Count - 1)
                    {
                        teams.Add(team);
                        i = teams.Count;
                    }
                }
            }
        }

        //Adds shots if the team is already in the list.
        private void AddExistingTeam(TeamClass team, int index)
        {
            for(int i = 0; i < team.NumberOfPlayers(); i++)
            {
                if(teams[index].CheckForPlayerOnTeam(team.GetPlayerAt(i)))
                {
                    teams[index].AddShots(team.GetPlayerAt(i), team.GetShots(i), team.GetShotsNotOn(i));
                }
                else
                {
                    teams[index].AddPlayer(team.GetPlayerAt(i), team.GetShots(i), team.GetShotsNotOn(i));
                }
            }
        }
        
        //Outputs the players, shots when on ice and shots when off ice to excel, then saves.
        public void Output()
        {
            Dealphabetize();

            Application app = new Application();
            Workbook workbook = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet worksheet = new Worksheet();

            worksheet = app.Worksheets["Sheet1"];
            worksheet.Name = teams[0].GetTeamName();

            worksheet = PutInCells(worksheet, teams[0]);

            for (int i = 1; i < teams.Count; i++)
            {
                worksheet = app.Worksheets.Add();
                worksheet.Name = teams[i].GetTeamName();
                worksheet = PutInCells(worksheet, teams[i]);
            }

            string path = Directory.GetCurrentDirectory();

            if(File.Exists(path + "\\CorsiOutput.xlsx"))
            {
                File.Delete(path + "\\CorsiOutput.xlsx");
            }

            workbook.SaveAs(path + "\\CorsiOutput.xlsx");

            app.Quit();
        }

        //Adds the player name, shot on ice and shots off ice to the cell for the excel sheets.
        public Worksheet PutInCells(Worksheet ws, TeamClass team)
        {
            ws.Cells[1, 1].Value = "Player";
            ws.Cells[1, 2].Value = "Shots For";
            ws.Cells[1, 3].Value = "Shots Against";
            ws.Cells[1, 4].Value = "Corsi Relative";
            for (int i = 0; i < team.GetNumberOfPlayers(); i++)
            {
                ws.Cells[i + 2, 1].Value = team.GetPlayerAt(i);
                ws.Cells[i + 2, 2].Value = team.GetShots(i);
                ws.Cells[i + 2, 3].Value = team.GetShotsNotOn(i);
                ws.Cells[i + 2, 4].Value = team.CalulateCorsiRelative(team.GetPlayerAt(i));
            }

            return ws;
        }

        //Reverse alphabetize the teams, so that a team that starts with A's sheet is before a team that starts with B's sheet.
        public void Dealphabetize()
        {
            List<TeamClass> dealphaClass = new List<TeamClass>();

            dealphaClass.Add(teams.First());

            for(int i = 1; i < teams.Count; i++)
            {
                for(int x = 0; x < dealphaClass.Count; x++)
                {
                    if(string.Compare(teams[i].GetTeamName(), dealphaClass[x].GetTeamName(), StringComparison.CurrentCultureIgnoreCase) > 0)
                    {
                        dealphaClass.Insert(x, teams[i]);
                        x = teams.Count;
                    }
                    if(x == dealphaClass.Count - 1)
                    {
                        dealphaClass.Add(teams[i]);
                        x = teams.Count;
                    }
                }
            }

            teams = dealphaClass;
        }
    }
}
