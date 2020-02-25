using ImageSizeReader;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
		private const bool CompareWithImageFromStream = true;

		static void Main(string[] args)
        {
			Console.Write("Path: ");
			var files = Directory.EnumerateFiles(Console.ReadLine(), "*.*", SearchOption.AllDirectories);

			Console.WriteLine("Starting");
			var stopwatch = Stopwatch.StartNew();

			var util = new ImageSizeReaderUtil();

			var successCounter = 0;
			var doesNotMatchCounter = 0;
			var failureCounter = 0;
			foreach (var file in files)
			{
				var loadable = false;
				var size = new System.Drawing.Size();
				try
				{
					using (var image = Image.FromStream(File.OpenRead(file), false, false))
					{
						loadable = true;
						size = image.Size;
					}
				}
				catch (Exception)
				{
					// ignore
				}

				try
				{
					using (var stream = File.OpenRead(file))
					{
						var dimensions = util.GetDimensions(stream);

						if (loadable)
						{
							if (dimensions.Width != size.Width || dimensions.Height != size.Height)
							{
								doesNotMatchCounter++;
								Console.WriteLine("does not match: " + file);
							}
						}
						else
						{
							Console.WriteLine("unknown if not match: " + file + " " + dimensions.Width +"x"+ dimensions.Height);
						}

						Console.WriteLine("Processing: " + new FileInfo(file).Name + " - OK - " + dimensions.Width + "x" + dimensions.Height);
						successCounter++;
					}
				}
				catch (Exception exception)
				{
					Console.WriteLine("Processing: " + new FileInfo(file).Name + " - Failed - " + "(" + exception.GetType().Name + ")" + exception.Message + "  -  " + file + " loadable: " + loadable);
					failureCounter++;
				}
			}

			stopwatch.Stop();

			Console.WriteLine("End, Duration: " + stopwatch.Elapsed);
			Console.WriteLine("Success: " + successCounter);
			Console.WriteLine("DoesNotMatchCounter: " + doesNotMatchCounter);
			Console.WriteLine("Failed: " + failureCounter);

			Console.WriteLine("\nPress enter to continue...");
			Console.ReadLine();
		}
    }
}
