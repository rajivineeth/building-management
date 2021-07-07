using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManagement.Models
{
    public class MaintenanceRequest
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column, ForeignKey("UserDetails")]
        public int UserId { get; set; }
        public virtual Registration UserDetails { get; set; }

        public string UserRequest { get; set; }
        public string ImagePath { get; set; }

        [NotMapped]
        public HttpPostedFileBase UploadImage { get; set; }     
        public string Status { get; set; }
       
        public DateTime CreateDate { get; set; }
    }
}