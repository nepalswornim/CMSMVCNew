using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSMVCFinal.Models.Models
{
    public class PostViewModel
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string Post { get; set; }
        public string FeaturedPhoto { get; set; }
        public string PostCreator { get; set; }
    }
}