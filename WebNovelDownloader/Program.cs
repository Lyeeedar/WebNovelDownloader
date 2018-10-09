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
			//DownloadWarlockoftheMagusWorld();
			//DownloadDungeonSeeker();
			DownloadReincarnationOfTheStrongestSwordGod();
		}

		private static string FetchPageContent(string url)
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				using (Stream stream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream))
				{
					var html = reader.ReadToEnd();
					return html;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static void DownloadReincarnationOfTheStrongestSwordGod()
		{
			var baseaddress = "http://gravitytales.com/novel/reincarnation-of-the-strongest-sword-god/rssg-chapter-";

			// in batches of 100
			// parallel try to find all pages
			var currentBatch = 1;
			var batchSize = 99;

			while (true)
			{
				System.Diagnostics.Debug.WriteLine("Reading batch " + currentBatch + " to " + (currentBatch + batchSize));

				string[] contents = new string[batchSize];
				Parallel.For(0, batchSize, (i) => 
				{
					var url = baseaddress + (currentBatch + i);
					var content = FetchPageContent(url);

					if (content == null)
					{
						// try slapping an extension on it, maybe itll work
						for (int ii = 0; ii < 10; ii++)
						{
							url = baseaddress + (currentBatch + i) + "-" + ii;
							content = FetchPageContent(url);

							if (content != null)
							{
								break;
							}
						}
					}

					contents[i] = content;
				});

				var firstnull = -1;
				var lastnotnull = -1;
				for (int i = 0; i < batchSize; i++)
				{
					if (firstnull == -1 && contents[i] == null)
					{
						firstnull = i;
					}

					if (contents[i] != null)
					{
						lastnotnull = i;
					}
				}

				// we have them all!
				if (firstnull == -1)
				{
					System.Diagnostics.Debug.WriteLine("Found all chapters in batch");
				}
				else if (firstnull < lastnotnull)
				{
					// we failed to find a chapter at the expected url, error so we can modify the search
					throw new Exception("Missing chapter at '" + (currentBatch + firstnull) + "'!");
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("Found up to chapter '" + (currentBatch + firstnull) + "'");
				}

				// convert all the chapters
				Parallel.For(0, batchSize, (i) =>
				{
					var html = contents[i];
					if (html != null)
					{
						var output = new StringBuilder();

						var split1 = html.Split(new string[] { "<div id=\"chapterContent\" class=\"innerContent fr-view\">" }, 2, StringSplitOptions.None);
						var split2 = split1[1].Split(new string[] { "</p>" }, 2, StringSplitOptions.None);

						var title = split2[0].Replace("<p>", "").Replace("<p style=\"line-height: 1.15\">", "").Replace("<p dir=\"ltr\">", "").Replace(" <span>", "").Replace("</span>", "").Trim();

						output.AppendLine("<h3>" + title + "</h3>");

						var split3 = split2[1].Split(new string[] { "</div>" }, 2, StringSplitOptions.None);

						var body = split3[0];

						body = body.Replace("<div style=\"border: 1px solid black\">", "");

						var lines = body.Split('\n');

						for (int l = 0; l < lines.Length; l++)
						{
							var line = lines[l];

							if (line.Contains("</p>"))
							{
								var sublines = line.Split(new string[] { "</p>" }, StringSplitOptions.None);
								foreach (var subline in sublines)
								{
									output.AppendLine(subline.Trim() + "</p>");
								}
							}
							else
							{
								line = line.Trim();

								output.AppendLine(line);
							}
						}

						contents[i] = output.ToString();
					}
				});

				// write out found chapters
				var finalOutput = new StringBuilder();
				for (int i = 0; i < batchSize; i++)
				{
					if (contents[i] != null)
					{
						finalOutput.Append(contents[i]);
					}
				}

				File.WriteAllText("D:/ReincarnationOfTheStrongestSwordGod_" + currentBatch + ".html", finalOutput.ToString(), Encoding.UTF8);

				if (firstnull != -1)
				{
					// we failed to find one, so bail
					break;
				}

				currentBatch += batchSize + 1;
			}
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

		private static void DownloadDungeonSeeker()
		{
			var indexPage = @"https://paichuntranslations.com/tds/";

			var output = new StringBuilder();

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(indexPage);

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				var html = reader.ReadToEnd();

				var split1 = html.Split(new string[] { "(Taken from MAL)<br />" }, 2, StringSplitOptions.None);
				var split2 = split1[1].Split(new string[] { "<span class=\"wpa-about\">Advertisements</span>" }, 2, StringSplitOptions.None);

				var linklist = split2[0];
				var lines = linklist.Split('\n').Select(e => e.Trim());

				var currentVolume = "Volume 0";

				foreach (var line in lines)
				{
					if (line.StartsWith("<h4 style=\"text-align: left;\"><strong>"))
					{
						var volume = line.Replace("<h4 style=\"text-align: left;\"><strong>", "");
						volume = volume.Replace("</strong></h4>", "");

						currentVolume = volume;

						output.Append("<h1>" + currentVolume + "</h1>\n\n");
					}
					else if (line.StartsWith("<a href="))
					{
						var pathsplit = line.Split('"');
						var path = pathsplit[1];

						var chaptersplit = line.Split('>');
						var chapter = chaptersplit[1].Replace(" ▼ ▼ ▼ ▼ ▼ ▼</a", "");

						var content = LoadDungeonSeekerChapter(path);

						output.Append(content);
					}
				}
			}

			File.WriteAllText("D:/TheDungeonSeeker.html", output.ToString());
		}

		private static string LoadDungeonSeekerChapter(string url)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				var html = reader.ReadToEnd();

				var split1 = html.Split(new string[] { "<div class=\"entry-content\">" }, 2, StringSplitOptions.None);
				var split2 = split1[1].Split(new string[] { "<p style=\"text-align:center;\">" }, 2, StringSplitOptions.None);

				var body = split2[0];

				body = body.Replace("<p>&nbsp;</p>", "");
				body = body.Replace("style=\"font-weight:400;\"", "");
				body = body.Replace("「", "\"");
				body = body.Replace("」", "\"");

				return body;
			}
		}
	}
}
