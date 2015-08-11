namespace DCC_PortfolioSite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProjectSpotlight")]
    public partial class ProjectSpotlight
    {
        public ProjectSpotlight()
        {
            TeamMembers = new HashSet<TeamMember>();
        }

        public int ProjectSpotlightID { get; set; }

        public int ProfileID { get; set; }

        [Required]
        [StringLength(255)]
        public string ProjectName { get; set; }

        [StringLength(255)]
        public string Technologies { get; set; }

        [StringLength(255)]
        public string DevelopmentTime { get; set; }

        public string ProjectDescription { get; set; }

        public string RepoLink { get; set; }

        [Column(TypeName = "image")]
        public byte[] Image_1 { get; set; }

        [Column(TypeName = "image")]
        public byte[] Image_2 { get; set; }

        public virtual ContactProfile ContactProfile { get; set; }

        public virtual ICollection<TeamMember> TeamMembers { get; set; }
    }
}