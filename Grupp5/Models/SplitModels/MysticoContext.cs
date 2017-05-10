using Grupp5.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public partial class MysticoContext: DbContext
    {
        public MysticoContext(DbContextOptions<MysticoContext> options) : base (options)
        {
            
        }

        public void AddUser(string id, string firstName, string lastName)
        {
            try
            {
                User.Add(new User
                {
                    AspId = id,
                    FirstName = firstName,
                    LastName = lastName
                });

                SaveChanges();
            }
            catch (Exception x)
            {
                Debug.Write(x.Message);
            }
        }
    }
}
