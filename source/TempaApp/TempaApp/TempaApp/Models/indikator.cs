using SQLite;

namespace TempaApp.Models
{
    public class Indicator
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; }
        public bool IsCurrentIndicator { get; set; }
        public string SoftwareVersion { get; set; }
    }
}
