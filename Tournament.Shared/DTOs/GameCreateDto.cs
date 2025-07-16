using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Shared.DTOs;

public record GameCreateDto : GameDto
{
    //[Required(ErrorMessage = "Title is a required field.")]
    //[MaxLength(25, ErrorMessage = "Maximum length for Title is 25 characters.")]
    //public string Title { get; init; }
    //public DateTime Time { get; init; }

    public int TournamentDetailsId { get; init; }

}
