namespace JobPortal.Shared.Structs;

public struct SalaryRange
{
    public decimal Min { get; set; }
    public decimal Max { get; set; }

    public SalaryRange(decimal min, decimal max)
    {
        if (min <= 0) throw new ArgumentException("Minimum salary must be greater than 0.");
        if (max < min) throw new ArgumentException("Maximum salary must be >= minimum salary.");
        Min = min;
        Max = max;
    }

    public bool IsInRange(decimal value) => value >= Min && value <= Max;

    public override string ToString() => $"{Min:F0} - {Max:F0} LPA";
}
