using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public OwnerRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Owner> GetAllOwners()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT o.Id AS 'Owner Id', o.[Name] AS 'Owner Name', o.Address, o.NeighborhoodId, o.Phone, o.Email,
                 d.Id, d.[Name], d.Breed, d.Notes, d.ImageUrl, d.OwnerId
                        FROM Owner o
                        Join Dog d on o.Id = d.OwnerId
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Owner> walkers = new List<Owner>();
                    while (reader.Read())
                    {
                        
                            Owner owner = new Owner
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Owner Id")),
                                Name = reader.GetString(reader.GetOrdinal("Owner Name")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                Email = reader.GetString(reader.GetOrdinal("Email"))
                            };
                        
                 
                 if (!reader.IsDBNull(reader.GetOrdinal("Id")))
                        {

                            Dog dog = new Dog();

                            dog.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            dog.Name = reader.GetString(reader.GetOrdinal("Name"));
                            dog.Breed = reader.GetString(reader.GetOrdinal("Breed"));
                            dog.OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"));
                            if (!reader.IsDBNull(reader.GetOrdinal("Notes")))
                            {
                                dog.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("ImageUrl")))
                            {
                                dog.ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"));
                            }

                            if (walkers.Any(x => x.Id == owner.Id ))
                            {

                                walkers.FirstOrDefault(x => x.Id == owner.Id).Dogs.Add(dog);

                            }
                            else 
                            {
                                owner.Dogs.Add(dog);
                                walkers.Add(owner);
                            }

                               
                        }
                 


                       
                        
                     
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }

        public Owner GetOwnerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], ImageUrl, NeighborhoodId
                        FROM Owner
                        WHERE Id = @id
                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Owner walker = new Owner
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        reader.Close();
                        return walker;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }
    }
}
