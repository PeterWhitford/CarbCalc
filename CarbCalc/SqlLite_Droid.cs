using System;
using System.Diagnostics;
using System.IO;
using Android.Content.Res;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;
using Environment = System.Environment;
using CarbCalc;
using Java.IO;
using Java.Nio.Channels;
using Org.Apache.Http.Impl.Cookie;
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

        //public void exportDatabse(String databaseName)
        //{
        //    try
        //    {
        //        File sd = Environment.getExternalStorageDirectory();
        //        File data = Environment.getDataDirectory();

        //        if (sd.canWrite())
        //        {
        //            String currentDBPath = "//data//" + getPackageName() + "//databases//" + databaseName + "";
        //            String backupDBPath = "backupname.db";
        //            File currentDB = new File(data, currentDBPath);
        //            File backupDB = new File(sd, backupDBPath);

        //            if (currentDB.exists())
        //            {
        //                FileChannel src = new FileInputStream(currentDB).getChannel();
        //                FileChannel dst = new FileOutputStream(backupDB).getChannel();
        //                dst.transferFrom(src, 0, src.size());
        //                src.close();
        //                dst.close();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}

    }
}