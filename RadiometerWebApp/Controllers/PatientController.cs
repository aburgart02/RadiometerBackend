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
    public IActionResult AddPatient(Patient patient)
    {
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
    
    [Authorize]
    [HttpGet]
    [Route("patients")]
    public IActionResult GetPatients()
    {
        var patients = _db.Patients.ToList();
        return Ok(JsonSerializer.Serialize(patients));
    }
}