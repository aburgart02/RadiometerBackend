using System.Text.Json;
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

    [HttpPost]
    [Route("add-patient")]
    public void AddPatient(Patient patient)
    {
        patient.BirthDate = patient.BirthDate.ToUniversalTime();
        if (!_db.Patients.Any(x => x.Name == patient.Name
                                   && x.Surname == patient.Surname
                                   && x.BirthDate == patient.BirthDate))
        {
            _db.Patients.Add(patient);
            _db.SaveChanges();
        }
    }
    
    [HttpGet]
    [Route("patients")]
    public string GetPatients()
    {
        var patients = _db.Patients.ToList();
        return JsonSerializer.Serialize(patients);
    }
}