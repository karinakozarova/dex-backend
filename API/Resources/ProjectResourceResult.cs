﻿using System;

namespace API.Resources
{
    public class ProjectResourceResult
    {
        
        public int Id { get; set; }
        
        public UserResourceResult User { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string ShortDescription { get; set; }
        
        public string Uri { get; set; }
        
        public string[] Contributors { get; set; }
        
        public DateTime Created { get; set; }
        
        public DateTime Updated { get; set; }
        
    }
}