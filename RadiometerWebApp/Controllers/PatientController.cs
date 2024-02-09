using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RadiometerWebApp.Models;

namespace RadiometerWebApp.Controllers;

public class PatientController : Controller
{
    private ApplicationContext _db;
    
    public PatientController(ApplicationContext context)
    {
        _db = context;
    }

    [Authorize]
    [HttpPost]
    [Route("add-patient")]
    public IActionResult AddPatient([FromBody] Patient patient)
    {
        patient.BirthDate = patient.BirthDate.ToUniversalTime();
        if (_db.Patients.Any(x => x.Name == patient.Name
                                  && x.Surname == patient.Surname
                                  && x.BirthDate == patient.BirthDate)) 
            return BadRequest();
        
        _db.Patients.Add(patient);
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize]
    [HttpPut]
    [Route("update-patient")]
    public IActionResult UpdatePatient([FromBody] Patient patient)
    {
        var dbPatient = _db.Patients.FirstOrDefault(x => x.Id == patient.Id);
        if (dbPatient == null)
            return BadRequest();

        dbPatient.Name = patient.Name;
        dbPatient.Surname = patient.Surname;
        dbPatient.Patronymic = patient.Patronymic;
        dbPatient.BirthDate = patient.BirthDate.ToUniversalTime();
        dbPatient.Sex = patient.Sex;
        dbPatient.Notes = patient.Notes;
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize]
    [HttpGet]
    [Route("patients")]
    public IActionResult GetPatients()
    {
        var patients = _db.Patients.ToList();
        return Ok(JsonSerializer.Serialize(patients));
    }
}