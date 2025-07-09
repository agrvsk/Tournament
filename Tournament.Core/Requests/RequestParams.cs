using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Requests;

public class RequestParams
{
    [Range(1, int.MaxValue)]
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    [Range(2, 100)]
    [DefaultValue(20)]
    public int PageSize { get; set; } = 5;
}

public class TournamentRequestParams : RequestParams
{
    [DefaultValue(false)]
    public bool ShowGames { get; set; } = false;
    [DefaultValue(false)]
    public bool Sort { get; set; } = false;
}
public class GameRequestParams : RequestParams
{
    [DefaultValue(false)]
    public bool Sort { get; set; } = false;

    public string? Title { get; set; }
}

