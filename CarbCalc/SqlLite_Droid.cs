using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Android.Content.Res;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;
using Environment = System.Environment;
using CarbCalc;
using Java.IO;
using Java.Nio.Channels;
using Org.Apache.Http.Impl.Cookie;
using ServiceStack;
using ServiceStack.Text;
using File = System.IO.File;

namespace CarbCalc
{
    public class SqlLiteDroid : FoodItem
    {
        public static string FileName = "CarbCalc.db";

        public static SQLiteConnection GetSqLiteConnection()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), FileName);

            var platform = new SQLitePlatformAndroid();

            var connection = new SQLiteConnection(platform, path);

            return connection;
        }

        public static void ExtractDb(AssetManager assets, string path)
        {
            var dbPath = path + "/" +FileName;

            // Check if your DB has already been extracted.
            if (File.Exists(dbPath)) return;

            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            var fromPath = Path.Combine(folderPath, FileName);

            using (var br = new BinaryReader(File.Open(fromPath, FileMode.Open)))
            {
                using (var bw = new BinaryWriter(new FileStream(dbPath, FileMode.CreateNew)))
                {
                    var buffer = new byte[2048];
                    var len = 0;
                    while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, len);
                    }
                }
            }
        }

        public static void ExportDatabaseToCsv()
        {
            try
            {
                var folderPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);

                var filePath = Path.Combine(folderPath, "CarbCalc Export.csv");
                
                var sql = SqlLiteDroid.GetSqLiteConnection();

                var cmd = sql.CreateCommand("Select * from FoodItem");

                var items = cmd.ExecuteQuery<FoodItem>().OrderBy(x => x.ItemName);

                //var csvWriter = new CsvWriter<FoodItem>();
                //var csvString = CsvSerializer.SerializeToCsv(items);

                File.WriteAllText(filePath, string.Join("," , items.Select(x => $"\"{x.ItemName}\"").ToList()));


                //using (var writeStream = new File(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                //{
                //    writeStream.WriteTo(csvString);
                //}

                //System.IO.Stream st = MyDownloadService.ContentResolver.OpenInputStream(my_download_id);
                //var newFile = System.IO.File.Create(new_file_path);
                //st.CopyTo(newFile);
                //fileStream.Close();


            }
            catch (Exception e)
            {

            }
        }

    }
}