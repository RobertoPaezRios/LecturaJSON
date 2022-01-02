using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using System.Data.SqlClient;

namespace LecturaJsonResponse
{
    class Program
    {
        public class Data
        {
            public string libro { get; set; }
            public string categoria { get; set; }
            public string descripcion { get; set; }
        }

        public class Root
        {
            public string status { get; set; }
            public string time_result { get; set; }
            public Data data { get; set; }
        }

        public static string JsonConver { get; private set; }

        static void Main(string[] args)
        {
            SqlConnection conexionDB = new SqlConnection("Data Source = localhost; Initial Catalog = bibliotecum; Integrated Security = True");
            conexionDB.Open();

            string url = "http://bibliotecum.herokuapp.com/api/getbook";

            Console.WriteLine("Introduce el isbn del libro: ");
            string isbnPost = Console.ReadLine();

            //string resultComprobar = getPost()

            Console.WriteLine("Introduce la api key: ");
            string apiKeyPost = Console.ReadLine();


            string result = getPost(url, isbnPost, apiKeyPost);

            JObject jsonIdentado = JObject.Parse(result);

            var rs = JsonConvert.DeserializeObject<Root>(result);

            //Console.WriteLine(rs.data.libro);
            //Console.WriteLine(jsonIdentado);

            addData(rs.data.libro.ToString(), rs.data.descripcion.ToString(), rs.data.categoria.ToString(), conexionDB);

            Console.ReadKey();
        }

        public static string getPost (string url, string isbnPost, string apiKeyPost)
        {
            Book book = new Book() { isbn = isbnPost, api_key = apiKeyPost };
            string result = "";

            WebRequest request = WebRequest.Create(url);
            request.Method = "post";
            request.ContentType = "application/json; charset=UTF-8";

            using(var oSW = new StreamWriter(request.GetRequestStream()))
            {
                //string json = "{\"isbn\" : \"1234\", \"api_key\" : \"pepe\"}";
                string json = JsonConvert.SerializeObject(book, Formatting.Indented);

                oSW.Write(json);
                oSW.Flush();
                oSW.Close();
            }

            WebResponse response = request.GetResponse(); 

            using(var oSR = new StreamReader(response.GetResponseStream()))
            {
                result = oSR.ReadToEnd().Trim();
            }

            return result;
        }

        public static void addData(string nombre, string descripcion, string categoria, SqlConnection conexionDB)
        {
            string query = "INSERT INTO libros (nombre, descripcion, categoria) VALUES ('" + nombre + "', '" + descripcion + "', '" + categoria + "')";

            SqlCommand comando = new SqlCommand(query, conexionDB);
            int r = comando.ExecuteNonQuery();
        }

        public class Book
        {
            public string isbn { get; set; }
            public string api_key { get; set; }
        }
    }
}
