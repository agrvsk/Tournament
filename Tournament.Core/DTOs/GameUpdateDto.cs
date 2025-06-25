using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs;

public record GameUpdateDto
{
    public int Id { get; init; }

    [Required(ErrorMessage = "Title is a required field.")]
    [MaxLength(25, ErrorMessage = "Maximum length for Title is 25 characters.")]
    public string Title { get; init; }
    public DateTime StartDate { get; init; }

}
