using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace BuildingManagement.Models
{
    public class OurDbContext:DbContext
    {
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<MaintenanceRequest> maintenanceRequests { get; set; }
    }
}