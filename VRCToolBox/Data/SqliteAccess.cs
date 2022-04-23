using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;
using VRCToolBox.Settings;

namespace VRCToolBox.Data
{
    internal static class SqliteAccess
    {
        internal static void InsertUserActivity(List<string[]> parametersList)
        {
            if (!Directory.Exists(ProgramSettings.Settings.UserActivityDBPath)) Directory.CreateDirectory(ProgramSettings.Settings.UserActivityDBPath);
            using (SqliteConnection connection = new SqliteConnection($@"Data Source={ProgramSettings.Settings.UserActivityDBPath}\{ProgramConst.VRChatLogDBName}{ProgramConst.FileExtensionSQLite3}"))
            {
                connection.Open();
                string commandText = @"CREATE TABLE IF NOT EXISTS USER_ACTIVITY ( ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  ACTIVITY_TIME TEXT NOT NULL, ACTIVITY_TYPE TEXT NOT NULL, WORLD_NAME TEXT NOT NULL, USER_NAME TEXT NOT NULL, FILE_NAME TEXT NOT NULL);";
                using(SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();

                    commandText = $@"INSERT INTO USER_ACTIVITY (ACTIVITY_TIME, ACTIVITY_TYPE, WORLD_NAME, USER_NAME, FILE_NAME) VALUES (:ACTIVITY_TIME, :ACTIVITY_TYPE, :WORLD_NAME, :USER_NAME, :FILE_NAME);";
                    command.CommandText = commandText;

                    foreach (string[] parameter in parametersList)
                    {
                        command.Parameters.AddWithValue(@$":ACTIVITY_TIME", parameter[0]);
                        command.Parameters.AddWithValue(@$":ACTIVITY_TYPE", parameter[1]);
                        command.Parameters.AddWithValue($@":WORLD_NAME", parameter[2]);
                        command.Parameters.AddWithValue(@$":USER_NAME", parameter[3]);
                        command.Parameters.AddWithValue(@$":FILE_NAME", parameter[4]);
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
            }
        }
        internal static void InsertWorldVisit(List<string> parametersList)
        {
            if (!Directory.Exists(ProgramSettings.Settings.WorldDataDBPath)) Directory.CreateDirectory(ProgramSettings.Settings.WorldDataDBPath);
            if (!Directory.Exists(ProgramSettings.Settings.UserActivityDBPath)) Directory.CreateDirectory(ProgramSettings.Settings.UserActivityDBPath);
            using (SqliteConnection connection = new SqliteConnection($@"Data Source={ProgramSettings.Settings.WorldDataDBPath}\{ProgramConst.VRChatWorldDBName}{ProgramConst.FileExtensionSQLite3}"))
            {
                connection.Open();
                string commandText = @"CREATE TABLE IF NOT EXISTS VRCHAT_WORLD ( ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, WORLD_NAME TEXT NOT NULL, WORLD_AUTHOR TEXT NOT NULL);";
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();

                    commandText = @"SELECT ID FROM VRCHAT_WORLD WHERE WORLD_NAME = :WORLD_NAME;";
                    command.CommandText = commandText;
                    command.Parameters.AddWithValue(@$":WORLD_NAME", parametersList[2]);
                    object? ScalarValue = command.ExecuteScalar();
                    if (ScalarValue == DBNull.Value || ScalarValue == null)
                    {
                        commandText = @"INSERT INTO VRCHAT_WORLD (WORLD_NAME, WORLD_AUTHOR) VALUES (:WORLD_NAME, :WORLD_AUTHOR); SELECT last_insert_rowid();";
                        command.CommandText = commandText;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue(@$":WORLD_NAME", parametersList[2]);
                        command.Parameters.AddWithValue(@$":WORLD_AUTHOR", " ");
                        ScalarValue = command.ExecuteScalar();
                        command.Parameters.Clear();
                    }

                    commandText = $@"ATTACH '{ProgramSettings.Settings.UserActivityDBPath}\{ProgramConst.VRChatLogDBName}{ProgramConst.FileExtensionSQLite3}' AS USER_ACTIVITY;";
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();

                    commandText = @"CREATE TABLE IF NOT EXISTS USER_ACTIVITY.WORLD_VISIT ( ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  VISIT_TIME TEXT NOT NULL, WORLD_ID TEXT NOT NULL, WORLD_NAME TEXT NOT NULL, FILE_NAME TEXT NOT NULL);";
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();

                    commandText = $@"INSERT INTO USER_ACTIVITY.WORLD_VISIT (VISIT_TIME, WORLD_NAME, WORLD_ID, FILE_NAME) VALUES (:VISIT_TIME, :WORLD_NAME, :WORLD_ID, :FILE_NAME);";
                    command.CommandText = commandText;

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue(@$":VISIT_TIME", parametersList[0]);
                    command.Parameters.AddWithValue(@$":WORLD_NAME", parametersList[2]);
                    command.Parameters.AddWithValue(@$":WORLD_ID"  , ScalarValue);
                    command.Parameters.AddWithValue(@$":FILE_NAME" , parametersList[4]);
                    command.ExecuteNonQuery();

                    commandText = $@"DETACH USER_ACTIVITY;";
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
