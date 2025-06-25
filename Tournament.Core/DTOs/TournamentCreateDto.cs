using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs;

public class TournamentCreateDto //(string Title, DateTime StartDate)
{
    [Required(ErrorMessage = "Title is a required field.")]
    [MaxLength(25, ErrorMessage = "Maximum length for Title is 25 characters.")]
    public string Title { get; init; } = null!;
    public DateTime StartDate { get; init; }
//    public DateTime EndDate { get; init; } // = StartDate.AddMonths(3);
}
