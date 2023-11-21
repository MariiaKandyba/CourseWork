using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DALTest.Entities
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsArchived { get; set; }
        public DateTime RegisterDate { get; set; }
        [JsonIgnore]
        public virtual ICollection<Group> Groups { get; set; }
    }
}