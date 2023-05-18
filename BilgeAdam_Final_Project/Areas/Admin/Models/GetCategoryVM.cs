using ApplicationCore.Entities.Abstract;

namespace BilgeAdam_Final_Project.Areas.Admin.Models
{
    public class GetCategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Status Status { get; set; }
    }
}
