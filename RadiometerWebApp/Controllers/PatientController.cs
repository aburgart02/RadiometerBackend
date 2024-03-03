using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiometerWebApp.Models;
using RadiometerWebApp.Utils;

namespace RadiometerWebApp.Controllers;

public class PatientController : Controller
{
    private ApplicationContext _db;
    
    public PatientController(ApplicationContext context)
    {
        _db = context;
    }

    [Authorize(Roles = $"{Role.Researcher},{Role.Admin}")]
    [HttpPost]
    [Route("add-patient")]
    public IActionResult AddPatient([FromBody] Patient patient)
    {
        patient.BirthDate = patient.BirthDate.ToUniversalTime();
        if (_db.Patients.Any(x => x.Name == patient.Name
                                  && x.Surname == patient.Surname
                                  && x.BirthDate == patient.BirthDate)) 
            return BadRequest("Patient already exist");
        
        _db.Patients.Add(patient);
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin}")]
    [HttpPut]
    [Route("update-patient")]
    public IActionResult UpdatePatient([FromBody] Patient patient)
    {
        var dbPatient = _db.Patients.FirstOrDefault(x => x.Id == patient.Id);
        if (dbPatient == null)
            return NotFound("Patient doesn't exist");

        dbPatient.Name = patient.Name;
        dbPatient.Surname = patient.Surname;
        dbPatient.Patronymic = patient.Patronymic;
        dbPatient.BirthDate = patient.BirthDate.ToUniversalTime();
        dbPatient.Sex = patient.Sex;
        dbPatient.Notes = patient.Notes;
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin}")]
    [HttpPost]
    [Route("delete-patient")]
    public IActionResult DeletePatient([FromBody] Patient patient)
    {
        var dbPatient = _db.Patients.Include(x => x.Measurements)
            .FirstOrDefault(x => x.Id == patient.Id);
        if (dbPatient == null)
            return NotFound("Patient doesn't exist");
        if (dbPatient.Measurements.Count > 0)
            return Conflict("Patient has dependent records");

        _db.Remove(dbPatient);
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin},{Role.ApiUser}")]
    [HttpGet]
    [Route("patients")]
    public IActionResult GetPatients()
    {
        if (TokenValidator.IsTokenInvalid(_db, Request.Headers["Authorization"]))
            return Unauthorized();
        
        var patients = _db.Patients.ToList();
        return Ok(JsonSerializer.Serialize(patients));
    }
}