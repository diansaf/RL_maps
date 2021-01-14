using maps.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace maps.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<Locations> locations = new List<Locations>();
            LocationLists model = new LocationLists();

            using (MySqlConnection conn = new ConnectionMysql().GetConnection())
            {
                try
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM Locations", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            locations.Add(new Locations()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Code = reader["Code"].ToString(),
                                Address = reader["Address"].ToString(),
                                Type = Convert.ToInt32(reader["Type"]),
                                Longitude = Convert.ToDouble(reader["Longitude"]),
                                Latitude = Convert.ToDouble(reader["Latitude"])
                            });
                        }
                    }
                }
                catch
                {
                    return View(model);
                }
                finally
                {
                    conn.Close();
                }
            }

            model.LocationList = locations;
            return View(model);
        }
    }
}