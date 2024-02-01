using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost]
    [Route("add-patient")]
    public IActionResult AddPatient(Patient patient)
    {
        if (!TokenValidator.IsTokenValid(_db, Request.Headers["Token"]))
            return Unauthorized();
        
        patient.BirthDate = patient.BirthDate.ToUniversalTime();
        if (!_db.Patients.Any(x => x.Name == patient.Name
                                   && x.Surname == patient.Surname
                                   && x.BirthDate == patient.BirthDate))
        {
            _db.Patients.Add(patient);
            _db.SaveChanges();
        }

        return Ok();
    }
    
    [HttpGet]
    [Route("patients")]
    public IActionResult GetPatients()
    {
        if (!TokenValidator.IsTokenValid(_db, Request.Headers["Token"]))
            return Unauthorized();
        
        var patients = _db.Patients.ToList();
        return Ok(JsonSerializer.Serialize(patients));
    }
}