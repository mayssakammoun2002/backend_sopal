public class MachineDto
{
    public string Id { get; set; } = string.Empty;  // ✅ string, correspond à CodeMachine
    public string Nom { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string Statut { get; set; } = string.Empty;
    public string? Localisation { get; set; }
}