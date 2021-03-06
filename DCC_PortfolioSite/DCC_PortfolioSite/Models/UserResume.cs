namespace DCC_PortfolioSite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserResume")]
    public partial class UserResume
    {

        public UserResume()
        {

        }
        
        [Key]
        public int UserResumeID { get; set; }

        public int ProfileID { get; set; }

        
        [DisplayName("Resume")]
        public string HtmlUpload { get; set; }

        public virtual ContactProfile ContactProfile { get; set; }

        public byte[] ResumeImg { get; set; }
    }
}
