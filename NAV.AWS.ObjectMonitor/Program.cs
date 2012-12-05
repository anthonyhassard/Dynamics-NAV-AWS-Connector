using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using NAV.AWS.ObjectMonitor.Properties;

namespace NAV.AWS.ObjectMonitor
{
	/// <summary>
	/// </summary>
	internal class Program
	{
		/// <summary>
		/// </summary>
		/// <param name="type"></param>
		/// <param name="id"></param>
		private static void ExportObject(int type, int id)
		{
			string filename = string.Format(
				@"C:\AWS\{0}.{1}.txt",
				Enum.GetName(typeof (ObjectType), type),
				Convert.ToString(id).PadLeft(10, '0'));
			var processStartInfo = new ProcessStartInfo
			{
				FileName = @"C:\Program Files (x86)\Microsoft Dynamics NAV\70\RoleTailored Client\finsql.exe",
				Arguments = new StringBuilder()
					.Append("command=").Append("exportobjects").Append(", ")
					.Append("file=").Append("\"").Append(filename).Append("\"").Append(", ")
					.Append("servername=").Append("\"").Append("WIN7-PC\\SQL2012").Append("\"").Append(", ")
					.Append("database=").Append("\"").Append("Demo Database NAV (7-0)").Append("\"").Append(", ")
					.Append("filter=").Append("\"").Append("Type=").Append(type).Append(";")
					.Append("ID=").Append(id).Append("\"").Append(", ")
					.Append("ntauthentication=").Append("yes").Append(", ")
					.Append("logfile=").Append("\"").Append(filename.Replace(".txt", ".log")).Append("\"")
					.ToString(),
				CreateNoWindow = true, RedirectStandardError = true, RedirectStandardOutput = true, UseShellExecute = false
			};

			var proc = new Process();
			proc.StartInfo = processStartInfo;
			proc.Start();
			string error = proc.StandardError.ReadToEnd();
			if (!string.IsNullOrWhiteSpace(error))
				Console.WriteLine("!! ERROR: {0}", error);
			string result = proc.StandardOutput.ReadToEnd();
			if (!string.IsNullOrWhiteSpace(result))
				Console.WriteLine("-> {0}", result);
			Console.WriteLine("-> Exported to {0}", filename);
			Console.WriteLine();
		}


		private static void Main(string[] args)
		{
			using (DbConnection connection = SqlClientFactory.Instance.CreateConnection())
			{
				connection.ConnectionString = Settings.Default.ConnectionString;
				connection.Open();

				Console.WriteLine("Connected to {0}", connection.Database);
				Console.WriteLine();

				ConsoleKeyInfo key;
				do
				{
					using (DbCommand cmd = connection.CreateCommand())
					{
						int priorVersion = Settings.Default.LastSyncVersion;
						Console.WriteLine("Previous Change Tracking Version: {0}", priorVersion);
						Console.WriteLine();

						cmd.CommandText =
							"SELECT SYS_CHANGE_VERSION, SYS_CHANGE_OPERATION, Type, ID " +
							"FROM CHANGETABLE(CHANGES Object, @version) CT WHERE CT.Type > 0";
						DbParameter param1 = cmd.CreateParameter();
						param1.ParameterName = "@version";
						param1.DbType = DbType.Int32;
						param1.Value = priorVersion < 0 ? 0 : priorVersion;
						cmd.Parameters.Add(param1);
						using (DbDataReader reader = cmd.ExecuteReader())
						{
							if (!reader.HasRows)
								Console.WriteLine("-- No new changes --");
							while (reader.Read())
							{
								Console.WriteLine(
									"Version: {0}, Operation: {1}, Type: {2}, ID: {3}",
									reader["SYS_CHANGE_VERSION"], reader["SYS_CHANGE_OPERATION"],
									reader["Type"], reader["ID"]);
								ExportObject(
									Convert.ToInt32(reader["Type"]),
									Convert.ToInt32(reader["ID"]));
							}
						}

						Console.WriteLine();

						cmd.CommandText = "SELECT CHANGE_TRACKING_CURRENT_VERSION()";
						int currentVersion = Convert.ToInt32(cmd.ExecuteScalar());
						Console.WriteLine("Current Change Tracking Version: {0}", currentVersion);

						if (priorVersion != currentVersion)
						{
							Settings.Default.LastSyncVersion = currentVersion;
							Settings.Default.Save();
						}

						key = Console.ReadKey(true);
					}
				}
				while (key.KeyChar != 'q' && key.KeyChar != 'Q' && key.KeyChar != 27);
			}
		}


		private enum ObjectType
		{
			Table = 1,
			Form = 2,
			Report = 3,
			Dataport = 4,
			Codeunit = 5,
			XMLport = 6,
			MenuSuite = 7,
			Page = 8,
			Query = 9
		}
	}
}
