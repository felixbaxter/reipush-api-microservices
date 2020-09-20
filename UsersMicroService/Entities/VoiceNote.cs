using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroService.Entities
{
	[Table("VoiceNotes")]

	public class VoiceNote
    {
	public VoiceNote()
	{
		CreatedOn = DateTime.UtcNow;
		UpdatedOn = DateTime.UtcNow;
	}

	[Key]
	public int VoiceNoteId { get; set; }
	public int UserId { get; set; }
	public string FileName { get; set; }
	public string LocalLocation { get; set; }
	public string RemoteLocation { get; set; }
	public bool Deleted { get; set; }
	public DateTime CreatedOn { get; set; }
	public DateTime UpdatedOn { get; set; }

	}
}
