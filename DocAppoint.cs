using System;
using System.Collections.Generic;
using System.Linq;

// Enum for User Roles
public enum UserRole
{
    Admin,
    Doctor,
    Patient
}

// User Class for Authentication and Role Management
public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }

    public User(string username, string password, UserRole role)
    {
        Username = username;
        Password = password;
        Role = role;
    }

    // Method for User Authentication
    public bool Authenticate(string username, string password)
    {
        return Username == username && Password == password;
    }

    // Method for Access Control based on Role
    public void AccessControl()
    {
        switch (Role)
        {
            case UserRole.Admin:
                Console.WriteLine("Admin access granted.");
                break;
            case UserRole.Doctor:
                Console.WriteLine("Doctor access granted.");
                break;
            case UserRole.Patient:
                Console.WriteLine("Patient access granted.");
                break;
            default:
                Console.WriteLine("Access denied.");
                break;
        }
    }
}

// Patient Class for Patient Details and Medical History
public class Patient
{
    public int PatientId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public List<string> MedicalHistory { get; private set; }

    public Patient(int patientId, string name, string email, string phoneNumber)
    {
        PatientId = patientId;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        MedicalHistory = new List<string>();
    }

    // Method to Add Entry to Medical History
    public void AddToMedicalHistory(string entry)
    {
        MedicalHistory.Add(entry);
    }

    // Method to Display Medical History
    public void ShowMedicalHistory()
    {
        Console.WriteLine($"Medical History for {Name}:");
        foreach (var entry in MedicalHistory)
        {
            Console.WriteLine(entry);
        }
    }

    public override string ToString()
    {
        return $"ID: {PatientId}, Name: {Name}, Email: {Email}, Phone: {PhoneNumber}";
    }
}

// Doctor Class for Doctor Details and Availability Management
public class Doctor
{
    public int DoctorId { get; set; }
    public string Name { get; set; }
    public string Specialty { get; set; }
    public Dictionary<DateTime, bool> Availability { get; private set; }

    public Doctor(int doctorId, string name, string specialty)
    {
        DoctorId = doctorId;
        Name = name;
        Specialty = specialty;
        Availability = new Dictionary<DateTime, bool>();
    }

    // Method to Set Availability of Doctor
    public void SetAvailability(DateTime date, bool isAvailable)
    {
        Availability[date] = isAvailable;
    }

    // Method to Check Doctor's Availability
    public bool IsAvailable(DateTime date)
    {
        return Availability.ContainsKey(date) && Availability[date];
    }

    public override string ToString()
    {
        return $"ID: {DoctorId}, Name: {Name}, Specialty: {Specialty}";
    }
}

// Appointment Class for Appointment Management
public class Appointment
{
    public int AppointmentId { get; set; }
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
    public DateTime AppointmentDate { get; set; }
    public bool IsConfirmed { get; set; }

    public Appointment(int appointmentId, Patient patient, Doctor doctor, DateTime appointmentDate)
    {
        AppointmentId = appointmentId;
        Patient = patient;
        Doctor = doctor;
        AppointmentDate = appointmentDate;
        IsConfirmed = false;
    }

    // Method to Confirm Appointment
    public void ConfirmAppointment()
    {
        IsConfirmed = true;
        Console.WriteLine($"Appointment {AppointmentId} confirmed for {Patient.Name} with Dr. {Doctor.Name} on {AppointmentDate}");
    }

    // Method to Send Reminder for Appointment
    public void SendReminder()
    {
        Console.WriteLine($"Reminder: Appointment {AppointmentId} is scheduled for {Patient.Name} with Dr. {Doctor.Name} on {AppointmentDate}");
    }

    // Method to Reschedule Appointment
    public void RescheduleAppointment(DateTime newDate)
    {
        AppointmentDate = newDate;
        Console.WriteLine($"Appointment {AppointmentId} rescheduled to {newDate} for {Patient.Name} with Dr. {Doctor.Name}");
    }
}

// NotificationService Class for Sending Email and SMS Notifications
public class NotificationService
{
    public void SendEmail(string to, string subject, string body)
    {
        Console.WriteLine($"Email sent to {to} with subject: {subject} and body: {body}");
    }

    public void SendSMS(string phoneNumber, string message)
    {
        Console.WriteLine($"SMS sent to {phoneNumber} with message: {message}");
    }
}

// AppointmentManager Class for Managing Appointments, Patients, and Doctors
public class AppointmentManager
{
    private List<Appointment> _appointments;
    private List<Patient> _patients;
    private List<Doctor> _doctors;
    private NotificationService _notificationService;

    public AppointmentManager()
    {
        _appointments = new List<Appointment>();
        _patients = new List<Patient>();
        _doctors = new List<Doctor>();
        _notificationService = new NotificationService();
    }

    // Method to Add a Patient
    public void AddPatient(Patient patient)
    {
        _patients.Add(patient);
        Console.WriteLine($"Patient {patient.Name} added.");
    }

    // Method to List All Patients
    public void ListPatients()
    {
        Console.WriteLine("Patients List:");
        foreach (var patient in _patients)
        {
            Console.WriteLine(patient);
        }
    }

    // Method to Add a Doctor
    public void AddDoctor(Doctor doctor)
    {
        _doctors.Add(doctor);
        Console.WriteLine($"Doctor {doctor.Name} added.");
    }

    // Method to List All Doctors
    public void ListDoctors()
    {
        Console.WriteLine("Doctors List:");
        foreach (var doctor in _doctors)
        {
            Console.WriteLine(doctor);
        }
    }

    // Method to Schedule an Appointment
    public void ScheduleAppointment(Appointment appointment)
    {
        if (!appointment.Doctor.IsAvailable(appointment.AppointmentDate))
        {
            Console.WriteLine($"Doctor {appointment.Doctor.Name} is not available on {appointment.AppointmentDate}");
            return;
        }

        _appointments.Add(appointment);
        appointment.Doctor.SetAvailability(appointment.AppointmentDate, false); // Mark the time slot as booked

        Console.WriteLine($"Appointment {appointment.AppointmentId} scheduled for {appointment.Patient.Name} with Dr. {appointment.Doctor.Name} on {appointment.AppointmentDate}");

        // Send confirmation notifications
        _notificationService.SendEmail(appointment.Patient.Email, "Appointment Confirmation",
            $"Your appointment with Dr. {appointment.Doctor.Name} is confirmed for {appointment.AppointmentDate}");
        _notificationService.SendSMS(appointment.Patient.PhoneNumber,
            $"Your appointment with Dr. {appointment.Doctor.Name} is confirmed for {appointment.AppointmentDate}");
    }

    // Method to Cancel an Appointment
    public void CancelAppointment(int appointmentId)
    {
        var appointment = _appointments.Find(a => a.AppointmentId == appointmentId);
        if (appointment != null)
        {
            _appointments.Remove(appointment);
            appointment.Doctor.SetAvailability(appointment.AppointmentDate, true); // Free up the time slot

            Console.WriteLine($"Appointment {appointmentId} for {appointment.Patient.Name} with Dr. {appointment.Doctor.Name} has been canceled.");

            // Send cancellation notifications
            _notificationService.SendEmail(appointment.Patient.Email, "Appointment Cancellation",
                $"Your appointment with Dr. {appointment.Doctor.Name} on {appointment.AppointmentDate} has been canceled.");
            _notificationService.SendSMS(appointment.Patient.PhoneNumber,
                $"Your appointment with Dr. {appointment.Doctor.Name} on {appointment.AppointmentDate} has been canceled.");
        }
        else
        {
            Console.WriteLine($"Appointment {appointmentId} not found.");
        }
    }

    // Method to Reschedule an Appointment
    public void RescheduleAppointment(int appointmentId, DateTime newDate)
    {
        var appointment = _appointments.Find(a => a.AppointmentId == appointmentId);
        if (appointment != null)
        {
            if (!appointment.Doctor.IsAvailable(newDate))
            {
                Console.WriteLine($"Doctor {appointment.Doctor.Name} is not available on {newDate}");
                return;
            }

            appointment.Doctor.SetAvailability(appointment.AppointmentDate, true); // Free up old time slot
            appointment.RescheduleAppointment(newDate);
            appointment.Doctor.SetAvailability(newDate, false); // Mark new time slot as booked

            // Send rescheduling notifications
            _notificationService.SendEmail(appointment.Patient.Email, "Appointment Rescheduled",
                $"Your appointment with Dr. {appointment.Doctor.Name} has been rescheduled to {newDate}");
            _notificationService.SendSMS(appointment.Patient.PhoneNumber,
                $"Your appointment with Dr. {appointment.Doctor.Name} has been rescheduled to {newDate}");
        }
        else
        {
            Console.WriteLine($"Appointment {appointmentId} not found.");
        }
    }

    // Method to Send Reminders for Appointments within 24 Hours
    public void SendReminders()
    {
        foreach (var appointment in _appointments)
        {
            if ((appointment.AppointmentDate - DateTime.Now).TotalHours <= 24)
            {
                appointment.SendReminder();
                _notificationService.SendEmail(appointment.Patient.Email, "Appointment Reminder",
                    $"This is a reminder for your appointment with Dr. {appointment.Doctor.Name} on {appointment.AppointmentDate}");
                _notificationService.SendSMS(appointment.Patient.PhoneNumber,
                    $"Reminder: Appointment with Dr. {appointment.Doctor.Name} on {appointment.AppointmentDate}");
            }
        }
    }

    // Method to List All Appointments
    public void ListAppointments()
    {
        Console.WriteLine("Current Appointments:");
        foreach (var appointment in _appointments)
        {
            Console.WriteLine($"ID: {appointment.AppointmentId}, Patient: {appointment.Patient.Name}, Doctor: {appointment.Doctor.Name}, Date: {appointment.AppointmentDate}, Confirmed: {appointment.IsConfirmed}");
        }
    }

    // Method to Search for Appointments based on Patient Name, Doctor Name, or Appointment Date
    public void SearchAppointments(string patientName = null, string doctorName = null, DateTime? appointmentDate = null)
    {
        var query = _appointments.AsQueryable();

        if (!string.IsNullOrEmpty(patientName))
        {
            query = query.Where(a => a.Patient.Name.Equals(patientName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(doctorName))
        {
            query = query.Where(a => a.Doctor.Name.Equals(doctorName, StringComparison.OrdinalIgnoreCase));
        }

        if (appointmentDate.HasValue)
        {
            query = query.Where(a => a.AppointmentDate.Date == appointmentDate.Value.Date);
        }

        var results = query.ToList();

        if (results.Any())
        {
            Console.WriteLine("Search Results:");
            foreach (var appointment in results)
            {
                Console.WriteLine($"ID: {appointment.AppointmentId}, Patient: {appointment.Patient.Name}, Doctor: {appointment.Doctor.Name}, Date: {appointment.AppointmentDate}, Confirmed: {appointment.IsConfirmed}");
            }
        }
        else
        {
            Console.WriteLine("No appointments found matching the criteria.");
        }
    }
}

// Program Class to Run the Main Logic
public class Program
{
    public static void Main(string[] args)
    {
        AppointmentManager manager = new AppointmentManager();

        // Add Users
        User adminUser = new User("admin", "adminpass", UserRole.Admin);
        User doctorUser = new User("drsmith", "docpass", UserRole.Doctor);
        User patientUser = new User("john", "patientpass", UserRole.Patient);

        // User Authentication and Access Control
        if (adminUser.Authenticate("admin", "adminpass"))
        {
            adminUser.AccessControl();

            // Add Patients
            Patient patient1 = new Patient(1, "John Doe", "john@example.com", "1234567890");
            Patient patient2 = new Patient(2, "Jane Doe", "jane@example.com", "0987654321");
            manager.AddPatient(patient1);
            manager.AddPatient(patient2);

            // Add Doctors
            Doctor doctor1 = new Doctor(1, "Dr. Smith", "Cardiology");
            Doctor doctor2 = new Doctor(2, "Dr. Adams", "Dermatology");
            doctor1.SetAvailability(DateTime.Now.AddHours(48), true);
            doctor1.SetAvailability(DateTime.Now.AddHours(72), true);
            doctor2.SetAvailability(DateTime.Now.AddHours(24), true);
            manager.AddDoctor(doctor1);
            manager.AddDoctor(doctor2);

            // Schedule Appointments
            Appointment appointment1 = new Appointment(1, patient1, doctor1, DateTime.Now.AddHours(48));
            Appointment appointment2 = new Appointment(2, patient2, doctor2, DateTime.Now.AddHours(24));
            manager.ScheduleAppointment(appointment1);
            manager.ScheduleAppointment(appointment2);

            // Patient Medical History
            patient1.AddToMedicalHistory("Check-up on " + DateTime.Now.AddHours(48).ToString());
            patient1.ShowMedicalHistory();

            // List All Appointments
            manager.ListAppointments();

            // Send Reminders
            manager.SendReminders();

            // Reschedule an Appointment
            manager.RescheduleAppointment(1, DateTime.Now.AddHours(72));

            // List All Appointments after Rescheduling
            manager.ListAppointments();

            // Cancel an Appointment
            manager.CancelAppointment(2);

            // List All Appointments after Cancellation
            manager.ListAppointments();

            // Access Doctor and Patient Roles
            if (doctorUser.Authenticate("drsmith", "docpass"))
            {
                doctorUser.AccessControl();
            }

            if (patientUser.Authenticate("john", "patientpass"))
            {
                patientUser.AccessControl();
            }
        }
    }
}
