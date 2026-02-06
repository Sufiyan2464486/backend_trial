namespace backend_trial.Models.Domain
{
    public class Report
    {
        public Guid ReportId { get; set; } = Guid.NewGuid();
        public string Scope { get; set; } = null!;
        public string Metrics { get; set; } = null!;
        public DateTime GeneratedDate { get; set; }
    }
}
