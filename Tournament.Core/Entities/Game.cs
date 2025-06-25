using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Entities;

public class Game
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is a required field.")]
    [MaxLength(25, ErrorMessage = "Maximum length for Title is 25 characters.")]
    public string Title { get; set; }
    public DateTime Time { get; set; }
    
    public int TournamentDetailsId { get; set; }
    // Navigation property to the Company entity
    public TournamentDetails? TournamentDetails { get; set; }

}
