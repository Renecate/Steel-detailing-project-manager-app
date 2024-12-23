using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESD.PM.Models
{
    public class SessionModel
    {
        public string User { get; set; }

        public int Id { get; set; }

        Random rng = new Random();

        public SessionModel(string user) 
        { 
            User = user;

            Id = rng.Next(1, 100000);
        }
    }
}
