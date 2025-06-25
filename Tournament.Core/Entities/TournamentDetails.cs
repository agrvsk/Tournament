using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Entities;

public class TournamentDetails
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is a required field.")]
    [MaxLength(25, ErrorMessage = "Maximum length for Title is 25 characters.")]
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    // Navigation property to the collection of Games
    public ICollection<Game>? Games { get; set; }



}
