using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ServiceTestLog
{
	public partial class Service1 : ServiceBase
	{
		Timer timer;
		String data = "";
		List<String> listData = new List<String>();
		int num = 0;
		public Service1()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			timer = new Timer();
			timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
			timer.Interval = 2 * 60 * 1000;
			timer.Enabled = true;
			WriteToFile("Service start at: " + DateTime.Now);
		}

		protected override void OnStop()
		{

			WriteToFile("Service stop at: " + DateTime.Now);
		}

		private void OnElapsedTime(object source, ElapsedEventArgs e)
		{
			DateTime now = DateTime.Now;
			
			string machineName = Environment.MachineName;
			
			Process[] list = null;
			try
			{
				if (string.IsNullOrEmpty(machineName))
					list = Process.GetProcesses();
				else
					list = Process.GetProcesses(machineName);


			}
			catch (Exception ex) 
			{ 
				Console.WriteLine(ex.Message);
			}

			foreach (Process p in list)
			{
				int a = 0;
				for(int i=0;i< listData.Count; i++)
				{
					if (p.ProcessName.Equals(listData[i]))
					{
						a = 1;
						break;
					}
				}
				if (a == 0)
				{
					data += (p.ProcessName + Environment.NewLine);
					num++;
					listData.Add(p.ProcessName);
				}
			}
			if (now.Minute >= 58 && now.Minute <= 59)
			{
				data = (num + Environment.NewLine) + data;
				WriteToFile(data);
				num = 0;
				listData.Clear();
			}
		}
		
		public void WriteToFile(string Message)
		{
			string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_')+"_"+ DateTime.Now.Hour + ".txt";
			if (!File.Exists(filepath))
			{
				// Create a file to write to.
				using (StreamWriter sw = File.CreateText(filepath))
				{
					sw.WriteLine(Message);
				}
			}
			else
			{
				using (StreamWriter sw = File.AppendText(filepath))
				{
					sw.WriteLine(Message);
				}
			}
		}
	}
}
