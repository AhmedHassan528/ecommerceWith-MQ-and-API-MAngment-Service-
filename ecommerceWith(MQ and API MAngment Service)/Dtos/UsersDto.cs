namespace MultiTenancy.Dtos
{
    public class UsersDto
    {
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string email { get; set; }
        public List<string> UserRoles { get; set; }
        public string PhoneNumber { get; set; }

    }
}
