using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PropertiesMicroService.ViewModels
{
    public class viDeletePropertyVoiceNote
    {
        [Required]
        public int PropertyId { get; set; }
        public int VoiceNoteId { get; set; }

    }
}

