using maps.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace maps.Controllers
{
    public class LocationsController : Controller
    {
        // GET: Locations
        public ActionResult Index()
        {
            return View();
        }

        // GET: Locations/Create
        public ActionResult Create()
        {
            Locations locations = new Locations();
            return View("Form", locations);
        }

        // POST: Locations/Create
        [HttpPost]
        public ActionResult Create(Locations model)
        {
            using (MySqlConnection conn = new ConnectionMysql().GetConnection())
            {
                conn.Open();

                try
                {
                    var sql = "INSERT INTO locations(Code, Address, Type, Longitude, Latitude) VALUES(@Code, @Address, @Type, @Longitude, @Latitude)";
                    var mySqlCommand = new MySqlCommand(sql, conn);

                    mySqlCommand.Parameters.AddWithValue("@Code", model.Code);
                    mySqlCommand.Parameters.AddWithValue("@Address", model.Address);
                    mySqlCommand.Parameters.AddWithValue("@Type", model.Type);
                    mySqlCommand.Parameters.AddWithValue("@Longitude", model.Longitude);
                    mySqlCommand.Parameters.AddWithValue("@Latitude", model.Latitude);
                    mySqlCommand.ExecuteNonQuery();

                    return RedirectToAction("Index", "Home");
                }
                catch
                {
                    return View("Form");
                } 
                finally
                {
                    conn.Close();
                }
            }           
        }

        // GET: Locations/Edit/5
        public ActionResult Edit(int id)
        {
            Locations location = new Locations();
            using (MySqlConnection conn = new ConnectionMysql().GetConnection())
            {
                try
                {
                    conn.Open();

                    MySqlCommand mySqlCommand = new MySqlCommand("SELECT * FROM Locations WHERE id = @id", conn);
                    mySqlCommand.Parameters.AddWithValue("@id", id);

                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            location.Id = Convert.ToInt32(reader["Id"]);
                            location.Code = reader["Code"].ToString();
                            location.Address = reader["Address"].ToString();
                            location.Type = Convert.ToInt32(reader["Type"]);
                            location.Longitude = Convert.ToDouble(reader["Longitude"]);
                            location.Latitude = Convert.ToDouble(reader["Latitude"]);
                        }
                    }
                }
                catch
                {
                    return View("Index", "Home");
                }
                finally
                {
                    conn.Close();
                }
            }

            return View("Form", location);
        }

        // POST: Locations/Edit/5
        [HttpPost]
        public ActionResult Edit(Locations model)
        {
            using (MySqlConnection conn = new ConnectionMysql().GetConnection())
            {
                conn.Open();

                try
                {
                    var sql = "UPDATE locations SET  Code = @Code, Address = @Address, Type = @Type, Longitude = @Longitude, Latitude =  @Latitude WHERE Id = @Id";
                    var mySqlCommand = new MySqlCommand(sql, conn);

                    mySqlCommand.Parameters.AddWithValue("@Id", model.Id);
                    mySqlCommand.Parameters.AddWithValue("@Code", model.Code);
                    mySqlCommand.Parameters.AddWithValue("@Address", model.Address);
                    mySqlCommand.Parameters.AddWithValue("@Type", model.Type);
                    mySqlCommand.Parameters.AddWithValue("@Longitude", model.Longitude);
                    mySqlCommand.Parameters.AddWithValue("@Latitude", model.Latitude);
                    mySqlCommand.ExecuteNonQuery();

                    return RedirectToAction("Index", "Home");
                }
                catch
                {
                    return View("Form", model);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        // POST: Locations/Delete/5
        [HttpPost]
        public void Delete(int id)
        {
            using (MySqlConnection conn = new ConnectionMysql().GetConnection())
            {
                conn.Open();

                try
                {
                    var sql = "DELETE FROM locations WHERE Id = @Id";
                    var mySqlCommand = new MySqlCommand(sql, conn);

                    mySqlCommand.Parameters.AddWithValue("@Id", id);
                    mySqlCommand.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public ActionResult FindRoute(string ids)
        {
            List<Locations> locations = new List<Locations>();
            LocationLists model = new LocationLists();

            using (MySqlConnection conn = new ConnectionMysql().GetConnection())
            {
                try
                {
                    conn.Open();

                    MySqlCommand mySqlCommand = new MySqlCommand("SELECT * FROM Locations WHERE FIND_IN_SET (Id, @ids)", conn);
                    mySqlCommand.Parameters.AddWithValue("@ids", ids);

                    using (var reader = mySqlCommand.ExecuteReader())
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
                finally
                {
                    conn.Close();
                }
            }

            model.LocationList = locations;
            List<Locations> locationLists = model.LocationList.ToList(),
                            resultLocation =  new List<Locations>();
            Locations point = model.LocationList.OrderBy(x => x.Id).FirstOrDefault();
            
            locationLists.Remove(point);
            resultLocation.Add(point);

            while(locationLists.Count > 0)
            {
                double? distance = null;
                Locations nextLocation = null;
                foreach(Locations location in locationLists)
                {
                    if (distance == null)
                    {
                        distance = Math.Sqrt(Math.Pow((location.Longitude - point.Longitude), 2) + Math.Pow((location.Latitude - point.Latitude), 2));
                        nextLocation = location;
                    }
                    else
                    {
                        double nextDistance = Math.Sqrt(Math.Pow((location.Longitude - point.Longitude), 2) + Math.Pow((location.Latitude - point.Latitude), 2));
                        if (nextDistance < distance)
                        {
                            distance = nextDistance;
                            nextLocation = location;
                        }
                    }
                }

                if(nextLocation != null)
                {
                    point = nextLocation;
                    locationLists.Remove(nextLocation);
                    resultLocation.Add(nextLocation);
                }
            }

            model.LocationList = resultLocation;
            return View("Route", model);
        }
    }
}
