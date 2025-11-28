namespace MultiTenancy.Dtos
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public IFormFile ImageFiles { get; set; }
    }
}
