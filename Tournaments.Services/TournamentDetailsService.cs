using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.VisualBasic;
using Service.Contracts;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Exceptions;
using Tournament.Core.Repositories;
using Tournament.Core.Requests;
using Tournament.Core.Responses;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tournaments.Services;

public class TournamentDetailsService(ITournamentUoW _uow, IMapper _mapper) : ITournamentDetailsService
{
//  public async Task<ResultObjectDto<IEnumerable<TournamentDto>>> GetAllAsync(bool showGames = false, bool sorted = false, int pageNr=1, int pageSize=20 )
    public async Task<ApiBaseResponse> GetAllAsync(TournamentRequestParams tParams)
    {
        //(IEnumerable objects, var pg) = 
        var pgList = await _uow.TournamentRepository.GetAllAsync(tParams);
        
        if (pgList.Items == null)
        {
            throw new NoTournamentsFoundException();
            return new NoTournamentsFoundResponse();
        }
        var dtos = _mapper.Map<IEnumerable<TournamentDto>>(pgList.Items);
        return new ApiOkResponse<IEnumerable<TournamentDto>>(dtos, pgList.MetaData);
    }

    public async Task<ApiBaseResponse> GetAsync(int id, bool showGames = false)
    {
        TournamentDetails? tournament = await _uow.TournamentRepository.GetAsync(id, showGames);
        if (tournament == null)
        {
            throw new TournamentNotFoundException(id);
            //return new TournamentNotFoundResponse(id);
        }
        var data = _mapper.Map<TournamentDto>(tournament);

        return new ApiOkResponse<TournamentDto>(data);
    }

    public async Task<ApiBaseResponse> CreateAsync(TournamentCreateDto dto)
    {
        //ResultObjectDto<TournamentDto> retur = new ResultObjectDto<TournamentDto>();
        //retur.IsSuccess = false;
        //retur.Data = null;
        //retur.Pagination = null;
        //retur.StatusCode = 500;

        var context = new ValidationContext(dto, null, null);
        var fel = new List<ValidationResult>();

        var ok =  Validator.TryValidateObject(dto, context, fel, validateAllProperties: true);
        if (!ok)
        {
            ValidationResult xx = fel[0];
            throw new ValidationException(xx.ErrorMessage);

            //foreach (ValidationResult result in fel)
            //{
            //    retur.Message = result.ErrorMessage;
            //}
        }

        var torment = _mapper.Map<TournamentDetails>(dto);
        _uow.TournamentRepository.Add(torment);
        await _uow.CompleteAsync();

        //if (retur.IsSuccess)
        //{
        //    retur.Message = string.Empty;
        //    retur.StatusCode = 201;
        //    retur.Id = torment.Id;
        //    retur.Data = _mapper.Map<TournamentDto>(torment);
        //    retur.Pagination = null;
        //}
        //return retur;

        var data = _mapper.Map<TournamentUpdateDto>(torment);
        return new ApiOkResponse<TournamentUpdateDto>(data);

        //try
        //{
        //    await _uow.CompleteAsync();
        //}
        //catch (DBConcurrencyException e)
        //{
        //    return (new ResultObjectDto<TournamentDto>
        //    {
        //        IsSuccess = false,
        //        Message = e.Message,
        //        StatusCode = 500,
        //        Data = null,
        //        Id = torment.Id,
        //        Pagination = null
        //    });
        //}
        //catch (Exception e)
        //{
        //    return (new ResultObjectDto<TournamentDto>()
        //    {
        //        IsSuccess = false,
        //        Message = e.Message,
        //        StatusCode = 500,
        //        Data = null,
        //        Id = torment.Id,
        //        Pagination = null
        //    });
        //}

        //return (new ResultObjectDto<TournamentDto>
        //{
        //    IsSuccess = true,
        //    Message = string.Empty,
        //    StatusCode = 200,
        //    Id = torment.Id,
        //    Data = _mapper.Map<TournamentDto>(torment),
        //    Pagination = null
        //});


    }

    public async Task<ApiBaseResponse> UpdateAsync(int Id, JsonPatchDocument<TournamentUpdateDto> patchDocument)
    {
        //ResultObjectDto<TournamentDto> retur = new ResultObjectDto<TournamentDto>();
        //retur.IsSuccess = false;
        //retur.Data = null;
        //retur.Pagination = null;
        //retur.StatusCode = 500;

        TournamentDetails? tournament = await _uow.TournamentRepository.GetAsync(Id, showGames: false);
        if (tournament == null)
        {
            throw new TournamentNotFoundException(Id);
        }
        var dto = _mapper.Map<TournamentUpdateDto>(tournament);

        patchDocument.ApplyTo(dto);
//      patchDocument.ApplyTo(dto, ModelState);

        var context = new ValidationContext(dto, null, null);
        var fel = new List<ValidationResult>();
        var ok = Validator.TryValidateObject(dto, context, fel, validateAllProperties: true);
        if (!ok)
        {
            ValidationResult xx = fel[0];
            throw new ValidationException(xx.ErrorMessage);
        }
        //TryValidateModel(dto);

        //if (!ModelState.IsValid)
        //{
        //    //return BadRequest(); //400
        //    //return UnprocessableEntity(ModelState); //422
        //}

        _mapper.Map(dto, tournament);
        await _uow.CompleteAsync();

        //if (retur.IsSuccess)
        //{
        //    retur.Data = _mapper.Map<TournamentDto>(tournament);
        //    retur.Message = string.Empty;
        //    return retur;
        //}
        //return retur;

        var data = _mapper.Map<TournamentUpdateDto>(tournament);
        return new ApiOkResponse<TournamentUpdateDto>(data);
    }
    public async Task<ApiBaseResponse> UpdateAsync (TournamentUpdateDto update)
    {
        //ResultObjectDto<TournamentDto> retur = new ResultObjectDto<TournamentDto>();
        //retur.IsSuccess = false;
        //retur.Data = null;
        //retur.Pagination = null;
        //retur.StatusCode = 500;

        TournamentDetails? tournament = await _uow.TournamentRepository.GetAsync(update.Id, showGames:false);
        if (tournament == null)
        {
            throw new TournamentNotFoundException(update.Id);
        }

        var torment = _mapper.Map(update, tournament);
        await _uow.CompleteAsync();

        //if (retur.IsSuccess)
        //{
        //    retur.StatusCode = 204;
        //    retur.Id = update.Id;
        //    retur.Message = string.Empty;
        //    retur.Data = _mapper.Map<TournamentDto>(torment);
        //}
        //return retur;

        var data = _mapper.Map<TournamentDto>(torment);
        return new ApiOkResponse<TournamentDto>(data);

        //try
        //{
        //    await _uow.CompleteAsync();
        //}
        ////      catch (DbUpdateConcurrencyException)
        //catch (DBConcurrencyException e)
        //{
        //    return new ResultObjectDto<TournamentDto>
        //    {
        //        IsSuccess = false,
        //        Message = e.Message,
        //        StatusCode = 500,
        //        Data = null,
        //        Pagination = null
        //    };

        //    //if (!await TournamentDetailsExists(id))
        //    //{
        //    //    return NotFound();
        //    //}
        //    //else
        //    //{
        //    //    return StatusCode(500);
        //    //}
        //}

        //return new ResultObjectDto<TournamentDto>
        //{
        //    IsSuccess = true,
        //    Message = string.Empty,
        //    StatusCode = 200,
        //    Data = _mapper.Map<TournamentDto>(torment),
        //    Pagination = null
        //};
    }

    public async Task<ApiBaseResponse> DeleteAsync(int id)
    {
        TournamentDetails? tournament = await _uow.TournamentRepository.GetAsync(id, showGames: false);
        
        if (tournament == null)
        {
            throw new TournamentNotFoundException(id);
        }

        _uow.TournamentRepository.Remove(tournament);
        await _uow.CompleteAsync();

        return new ApiOkResponse<int>(id);

    }
}
