using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebNovelDownloader
{
	class Program
	{
		static void Main(string[] args)
		{
			//DownloadTalesOfReincarnatedLord();
			DownloadWarlockoftheMagusWorld();
		}

		private static void DownloadTalesOfReincarnatedLord()
		{
			var baseaddress = "https://liberspark.com/read/tales-of-the-reincarnated-lord/chapter-";

			var output = new StringBuilder();

			int i = 0;
			while (true)
			{
				i++;
				System.Diagnostics.Debug.WriteLine("Reading chapter " + i);

				try
				{
					var url = baseaddress + i;

					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

					using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
					using (Stream stream = response.GetResponseStream())
					using (StreamReader reader = new StreamReader(stream))
					{
						var html = reader.ReadToEnd();

						var split1 = html.Split(new string[] { "<h3 id=\"reader-title\" class=\"text-links\">" }, 2, StringSplitOptions.None);
						var split2 = split1[1].Split(new string[] { "</h3>" }, 2, StringSplitOptions.None);

						var title = split2[0];

						output.AppendLine("<h3>Chapter " + i + ": " + title + "</h3>");

						var split3 = split2[1].Split(new string[] { "<div class=\"reader-style reader-content dont-break-out\" id=\"reader-content\"" }, 2, StringSplitOptions.None);

						var split4 = split3[1].Split(new string[] { "</div>" }, 2, StringSplitOptions.None);

						var body = split4[0];
						var lines = body.Split('\n');

						for (int l = 2; l < lines.Length; l++)
						{
							var line = lines[l];
							line = line.Trim();

							output.AppendLine(line);
						}
					}
				}
				catch (Exception)
				{
					break;
				}
			}

			System.Diagnostics.Debug.WriteLine("Read " + i + " chapters.");

			File.WriteAllText("D:/TalesOfTheReincarnatedLord.html", output.ToString());
		}

		private static void DownloadWarlockoftheMagusWorld()
		{
			var baseaddress = "http://www.wuxiaworld.com/novel/warlock-of-the-magus-world/wmw-chapter-";

			var output = new StringBuilder();

			int i = 0;
			while (true)
			{
				i++;
				System.Diagnostics.Debug.WriteLine("Reading chapter " + i);

				try
				{
					var url = baseaddress + i;

					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

					using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
					using (Stream stream = response.GetResponseStream())
					using (StreamReader reader = new StreamReader(stream))
					{
						var html = reader.ReadToEnd();

						var split1 = html.Split(new string[] { "<div class=\"fr-view\">" }, 2, StringSplitOptions.None);
						var title = "";
						var rawBody = split1[1].Trim();
						if (rawBody.StartsWith("<p><span style=\"text-decoration: underline\"><strong>"))
						{
							var text = rawBody.Replace("<p><span style=\"text-decoration: underline\"><strong>", "");

							var split2 = text.Split(new string[] { "</strong></span></p>" }, 2, StringSplitOptions.None);

							title = split2[0];
							rawBody = split2[1];
						}

						output.AppendLine("<h3>Chapter " + i + ": " + title + "</h3>");

						var split3 = rawBody.Split(new string[] { "<a href=" }, 2, StringSplitOptions.None);

						var body = split3[0];
						output.AppendLine(body.Trim());
					}
				}
				catch (Exception)
				{
					break;
				}
			}

			System.Diagnostics.Debug.WriteLine("Read " + i + " chapters.");

			File.WriteAllText("D:/WarlockoftheMagusWorld.html", output.ToString());
		}
	}
}
