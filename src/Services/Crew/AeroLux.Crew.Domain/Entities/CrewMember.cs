using AeroLux.Shared.Kernel.DDD;

namespace AeroLux.Crew.Domain.Entities;

/// <summary>
/// Crew member role enumeration
/// </summary>
public sealed class CrewRole : Enumeration<CrewRole>
{
    public static readonly CrewRole Captain = new(1, nameof(Captain));
    public static readonly CrewRole FirstOfficer = new(2, nameof(FirstOfficer));
    public static readonly CrewRole FlightAttendant = new(3, nameof(FlightAttendant));

    private CrewRole(int value, string name) : base(value, name) { }
}

/// <summary>
/// Crew member availability status
/// </summary>
public sealed class CrewStatus : Enumeration<CrewStatus>
{
    public static readonly CrewStatus Available = new(1, nameof(Available));
    public static readonly CrewStatus OnDuty = new(2, nameof(OnDuty));
    public static readonly CrewStatus Resting = new(3, nameof(Resting));
    public static readonly CrewStatus OnLeave = new(4, nameof(OnLeave));
    public static readonly CrewStatus Training = new(5, nameof(Training));

    private CrewStatus(int value, string name) : base(value, name) { }
}

/// <summary>
/// Crew member aggregate root
/// </summary>
public sealed class CrewMember : AggregateRoot<Guid>
{
    private readonly List<string> _certifications = [];
    private readonly List<string> _aircraftQualifications = [];

    public Guid UserId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public CrewRole Role { get; private set; } = null!;
    public CrewStatus Status { get; private set; } = null!;
    public string LicenseNumber { get; private set; } = string.Empty;
    public DateTime LicenseExpiryDate { get; private set; }
    public DateTime? LastFlightDate { get; private set; }
    public DateTime? RestStartTime { get; private set; }
    public int TotalFlightHours { get; private set; }

    public IReadOnlyList<string> Certifications => _certifications.AsReadOnly();
    public IReadOnlyList<string> AircraftQualifications => _aircraftQualifications.AsReadOnly();
    public string FullName => $"{FirstName} {LastName}";
    public bool IsLicenseValid => LicenseExpiryDate > DateTime.UtcNow;

    private CrewMember() { }

    public static CrewMember Create(
        Guid userId,
        string firstName,
        string lastName,
        CrewRole role,
        string licenseNumber,
        DateTime licenseExpiryDate)
    {
        return new CrewMember
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            Status = CrewStatus.Available,
            LicenseNumber = licenseNumber,
            LicenseExpiryDate = licenseExpiryDate,
            TotalFlightHours = 0
        };
    }

    public void AddCertification(string certification)
    {
        if (!_certifications.Contains(certification))
            _certifications.Add(certification);
    }

    public void AddAircraftQualification(string aircraftModel)
    {
        if (!_aircraftQualifications.Contains(aircraftModel))
            _aircraftQualifications.Add(aircraftModel);
    }

    public bool IsQualifiedForAircraft(string aircraftModel)
    {
        return _aircraftQualifications.Contains(aircraftModel);
    }

    public void AssignToDuty()
    {
        if (Status != CrewStatus.Available)
            throw new InvalidOperationException("Crew member is not available.");

        if (!IsLicenseValid)
            throw new InvalidOperationException("Crew member's license has expired.");

        if (!IsRestedEnough())
            throw new InvalidOperationException("Crew member has not completed required rest period.");

        Status = CrewStatus.OnDuty;
    }

    public void StartRest()
    {
        Status = CrewStatus.Resting;
        RestStartTime = DateTime.UtcNow;
    }

    public void EndRest()
    {
        if (Status != CrewStatus.Resting)
            throw new InvalidOperationException("Crew member is not resting.");

        Status = CrewStatus.Available;
    }

    public void CompleteFlight(int flightHours)
    {
        TotalFlightHours += flightHours;
        LastFlightDate = DateTime.UtcNow;
        StartRest();
    }

    public bool IsRestedEnough()
    {
        if (RestStartTime is null || LastFlightDate is null)
            return true;

        var restDuration = DateTime.UtcNow - RestStartTime.Value;
        return restDuration.TotalHours >= 10; // FAA minimum rest requirement
    }
}
