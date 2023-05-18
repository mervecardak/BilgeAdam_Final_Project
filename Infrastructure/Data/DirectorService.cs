using ApplicationCore.Entities.Concrete;
using ApplicationCore.Entities.DTO_s.DirectorDTO;
using ApplicationCore.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class DirectorService : EfRepository<Director>, IDirectorService
    {
        private readonly AppDbContext _context;

        public DirectorService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<GetDirectorDTO>> GetDirectors()
        {
            var directors =await _context.Directors.ToListAsync(); 
            var diretorDtos=new List<GetDirectorDTO>();
            foreach (var director in directors)
            {
                var model = new GetDirectorDTO
                {
                    Id = director.Id,
                    FullName = director.FirstName + " " + director.LastName,
                };
                diretorDtos.Add(model);
            }
            return diretorDtos;
        }
    }
}
