using System;
using System.Collections.Generic;
using System.Linq;

// Generic repository for any entity
public class Repository<T>
{
    private List<T> items = new();

    public void Add(T item) => items.Add(item);

    public List<T> GetAll() => new List<T>(items);

    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

// Patient entity
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString() => $"{Name} (ID: {Id}, Age: {Age}, Gender: {Gender})";
}

// Prescription entity
public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString() =>
        $"{MedicationName} (Issued: {DateIssued:dd-MMM-yyyy})";
}

// Main healthcare system app
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new();
    private Repository<Prescription> _prescriptionRepo = new();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new();

    public void SeedData()
    {
        // Add patients
        _patientRepo.Add(new Patient(1, "Ama Mensah", 29, "Female"));
        _patientRepo.Add(new Patient(2, "Kwame Boateng", 42, "Male"));
        _patientRepo.Add(new Patient(3, "Akua Asante", 35, "Female"));

        // Add prescriptions (with valid PatientIds)
        _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(102, 2, "Paracetamol", DateTime.Now.AddDays(-3)));
        _prescriptionRepo.Add(new Prescription(103, 1, "Ibuprofen", DateTime.Now.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(104, 3, "Vitamin C", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(105, 2, "Cetirizine", DateTime.Now.AddDays(-2)));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();
        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("\n=== All Patients ===");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine(patient);
        }
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        Console.WriteLine($"\n=== Prescriptions for Patient ID {patientId} ===");
        if (_prescriptionMap.ContainsKey(patientId))
        {
            foreach (var prescription in _prescriptionMap[patientId])
            {
                Console.WriteLine(prescription);
            }
        }
        else
        {
            Console.WriteLine("No prescriptions found for this patient.");
        }
    }
}

class Program
{
    static void Main()
    {
        var app = new HealthSystemApp();

        app.SeedData();
        app.BuildPrescriptionMap();

        app.PrintAllPatients();

        Console.Write("\nEnter Patient ID to view prescriptions: ");
        if (int.TryParse(Console.ReadLine(), out int selectedId))
        {
            app.PrintPrescriptionsForPatient(selectedId);
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid Patient ID.");
        }
    }
}
