using ApplicationCore.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IMovieCategoryService
    {
        Task AddMovieCategory(MovieCategory movieCategory);
        Task DeleteMovieCategory(int categoryId, int movieId);
        Task<List<MovieCategory>> GetAllByMovieId(int id);
        Task<bool> AnyByMovieId(int movieId, int categoryId);
        Task<List<Category>> GetCategoriesByMovieId(int movieId);
        List<string> GetStringCategoriesByMovieId(int movieId);
    }
}
