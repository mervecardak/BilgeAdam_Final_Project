namespace BilgeAdam_Final_Project.Areas.Admin.Models
{
    public class GetMovieDetailVM
    {
        //detay sayfası için olusturdum oluşma zamanına falan gerek yok movieVm de olurdu ama gereksiz kodlar olmasın diye yeniden yaptık
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public string DirectorName { get; set; }
        public string Image { get; set; }
        public List<string> Categories { get; set; }
    }
}
