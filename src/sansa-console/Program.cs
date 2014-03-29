using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;

namespace sansaconsole
{
	class Program
	{
		static Stack<System.IO.DirectoryInfo> directoriesQueue = new Stack<System.IO.DirectoryInfo>();
		static Int64 nbFiles = 0;
		static Int64 nbFilesNameChars = 0;
		static Int64 nbFilesFullNameChars = 0;
		static Int64 filesTotalSize= 0;
		static Int64 nbUnauthorizedAccessException= 0;
		static Int64 nbException= 0;
		public static void Main (string[] args)
		{
			System.IO.DirectoryInfo mainDir = new System.IO.DirectoryInfo("/mnt");
			directoriesQueue.Push(mainDir);

			TimerCallback tcb = DisplayStatus;

			Timer t = new Timer(tcb,null,1000,1000);

			while (directoriesQueue.Count > 0) {
				System.IO.DirectoryInfo curDir = directoriesQueue.Pop ();
				if (Regex.Match(curDir.FullName, @"dosdevices/[A-Za-z]:/").Success)
				{
					continue;
				}
				try
				{
					foreach (System.IO.DirectoryInfo subDir in curDir.EnumerateDirectories()) {
						directoriesQueue.Push (subDir);
					}
					foreach (System.IO.FileInfo file in curDir.EnumerateFiles()) {
						nbFiles++;
						nbFilesNameChars += file.Name.ToCharArray().Length;
						nbFilesFullNameChars += file.FullName.ToCharArray().Length;
						filesTotalSize += file.Length;
					}
				} catch (UnauthorizedAccessException) {
					nbUnauthorizedAccessException++;
				} catch (Exception ex) {
					nbException++;
					Console.WriteLine ("Unexpected exception for dir '" + curDir.ToString() + "' : " + ex.ToString());
				} 
			}

			Console.WriteLine (nbFiles  + " files found ! |" + nbFilesNameChars  + "|" + nbFilesFullNameChars  + "|" + filesTotalSize + "|" + nbUnauthorizedAccessException + "|" + nbException);
		}

		static void DisplayStatus(Object stateInfo)
		{
			Console.WriteLine (directoriesQueue.Peek().ToString() + "|" + directoriesQueue.Count + "|" + nbFiles  + "|" + nbFilesNameChars  + "|" + nbFilesFullNameChars  + "|" + filesTotalSize);

		}
	}
}
