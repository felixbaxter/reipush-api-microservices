using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PropertiesMicroService.Entities
{
    [Table("PropertyVoiceNotes")]
    public class PropertyVoiceNote
    {
        [Key]
        public int PropertyVoiceNoteId { get; set; }
        public int PropertyId { get; set; }
        public int VoiceNoteId { get; set; }
    }
}
