using AeroLux.Domain.Common;
using AeroLux.Domain.Enums;

namespace AeroLux.Domain.Entities;

/// <summary>
/// Aircraft aggregate root representing private jets
/// </summary>
public class Aircraft : Entity
{
    public string RegistrationNumber { get; private set; }
    public string Model { get; private set; }
    public string Manufacturer { get; private set; }
    public int PassengerCapacity { get; private set; }
    public AircraftStatus Status { get; private set; }
    public int TotalFlightHours { get; private set; }
    public DateTime? LastMaintenanceDate { get; private set; }

    private Aircraft() { } // EF Core

    public Aircraft(string registrationNumber, string model, string manufacturer, int passengerCapacity)
    {
        RegistrationNumber = registrationNumber ?? throw new ArgumentNullException(nameof(registrationNumber));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Manufacturer = manufacturer ?? throw new ArgumentNullException(nameof(manufacturer));
        
        if (passengerCapacity <= 0)
            throw new ArgumentException("Passenger capacity must be greater than zero", nameof(passengerCapacity));
        
        PassengerCapacity = passengerCapacity;
        Status = AircraftStatus.Available;
        TotalFlightHours = 0;
    }

    public void UpdateStatus(AircraftStatus newStatus)
    {
        Status = newStatus;
        MarkAsUpdated();
    }

    public void RecordMaintenance(DateTime maintenanceDate)
    {
        LastMaintenanceDate = maintenanceDate;
        Status = AircraftStatus.Maintenance;
        MarkAsUpdated();
    }

    public void AddFlightHours(int hours)
    {
        if (hours < 0)
            throw new ArgumentException("Flight hours cannot be negative", nameof(hours));
        
        TotalFlightHours += hours;
        MarkAsUpdated();
    }
}
