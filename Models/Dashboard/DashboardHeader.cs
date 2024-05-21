namespace CMS_Brian.Models.Dashboard
{
    public class DashboardModel
    {
        public string UserId { get; set; }
        public string Role { get; set; }
        public int TotalAdministrators { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalJudges { get; set; }
        public int TotalClients { get; set; }
        public int TotalClasses { get; set; }
    }
}