using Sahityak.Models;
using System.Data;
using System.Text;

namespace Sahityak.Helper
{
    public static class CommonHelper
    {
        public static IConfiguration Configuration;

        public static string CurrentConnString;

        public static int DefaultRoleId = 2;

        public static Response AddLog(string Descr)
        {
            return AddLog(Descr, "");
        }
        public static Response AddLog(string Descr, string Log)
        {
            try
            {
                DataHelper dataHelper = new DataHelper();
                string sqlExp = "Insert into Logs(Descr, LogData, InsertedAt) values('" + Descr.Replace("'", "''") + "', '" + Log.Replace("'", "''") + "', GetDate())";
                dataHelper.ExecuteNonQuery(sqlExp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed in create log, Error : " + ex.Message);
                return new Response(false, ex.Message);
            }

            return new Response(true, "Log Created successfully");
        }

        public static Response AddUser(int ITSID, string FullName, char Gender, int Age, string Jamaat)
        {
            //Console.WriteLine("Before Create User");

            try
            {
                DataHelper dataHelper = new DataHelper();
                string sqlExp = "Insert into Users(ITSID, FullName, Gender, Age, Jamaat, InsertedAt) " +
                    "values(" + ITSID + ", '" + FullName + "', '" + Gender + "', " + Age + ", '" + Jamaat + "', GetUtcDate())";
                dataHelper.ExecuteNonQuery(sqlExp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed in create user, Error : " + ex.Message);
                return new Response(false, ex.Message);
            }

            //Console.WriteLine("After Create User");
            return new Response(true, "User Created successfully");
        }

        public static DataSet GetDataSetFromStringXml(string strXml)
        {
            //var strSchema = arrayOfXElement.Nodes[0].ToString();
            //var strData = arrayOfXElement.Nodes[1].ToString();
            //var strXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n\t<DataSet>";
            //strXml += strSchema + strData;
            //strXml += "</DataSet>";

            DataSet ds = new DataSet("TestDataSet");
            ds.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(strXml)));

            return ds;
        }

        public async static Task<string> DownloadString(string strFileUrlToDownload)

        {
            HttpClient client = new HttpClient();
            string downloadString = await client.GetStringAsync(new Uri(strFileUrlToDownload));
            return downloadString;

            //MemoryStream storeStream = new MemoryStream();

            //storeStream.SetLength(myDataBuffer.Length);

            //storeStream.Write(myDataBuffer, 0, (int)storeStream.Length);

            //storeStream.Flush();



            ////TO save into certain file must exist on Local

            //SaveMemoryStream(storeStream, "C:\\TestFile.txt");



            ////The below Getstring method to get data in raw format and manipulate it as per requirement

            //string download = Encoding.ASCII.GetString(myDataBuffer);



            //Console.WriteLine(download);

            //Console.ReadLine();

        }
    }
}
