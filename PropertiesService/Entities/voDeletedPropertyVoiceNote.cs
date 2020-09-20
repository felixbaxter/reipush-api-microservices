using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PropertiesMicroService.Entities
{
    public class voDeletedPropertyVoiceNote
    {
        public int VoiceNoteId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
