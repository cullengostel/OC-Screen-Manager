﻿using Microsoft.Data.Sqlite;
using Screen_Manager_Forms_Application.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen_Manager_Forms_Application.Controllers
{
    internal class ScreenController
    {
        private static List<PrintScreen> Screens = new();

        public ScreenController()
        {
            Screens = LoadAllScreensFromDatabase();
        }
        private static List<PrintScreen> LoadAllScreensFromDatabase()
        {
            List<PrintScreen> screens = new();

            string connectionString = MainController.ConnectionString;
            
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ScreenID, LocationID, Quantity, Design, CustomerName, Description FROM Screens";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    { 
                        int sid = reader.GetInt32(0);
                        int lid = reader.GetInt32(1);
                        int quantity = reader.GetInt32(2);
                        string design = reader.GetString(3);
                        string name = reader.GetString(4);
                        string desc = reader.GetString(5);
                        Location loc = LocationController.GetLocation(lid);
                        screens.Add(new PrintScreen(sid, loc, quantity, design, name, desc));
                    }
                }
            }

            return screens;
        }

        public static void Debug_CheckScreensLoaded()
        {
            foreach(PrintScreen s in Screens)
            {
                Debug.WriteLine(s);
            }
        }

        public static bool IsLocationInUse(int id)
        {
            foreach(PrintScreen s in Screens)
            {
                if(s.ScreenLocation.GetID() == id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
