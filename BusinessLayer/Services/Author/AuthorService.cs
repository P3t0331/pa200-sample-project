﻿using BusinessLayer.DTOs.Author;
using BusinessLayer.Mapper;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services.Author
{
    public class AuthorService : BaseService , IAuthorService
    {
        private readonly BookHubDBContext _dbContext;
        public AuthorService(BookHubDBContext dBContext) : base(dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<bool> CreateAuthor(AuthorCreateUpdateDTO authorDTO)
        {
            var author = EntityMapper.MapToAuthor(authorDTO);
            if (author == null) {
                return false;
            }
            _dbContext.Authors.Add(author);
            await SaveAsync(true);
            return true;
        }

        public async Task<bool> DeleteAuthor(int authorId)
        {
            var author = await _dbContext.Authors.FindAsync(authorId);
            if (author == null)
            {
                return false;
            }
            var isAuthorOfAnyBook = await _dbContext.AuthorBooks.AnyAsync(ab => ab.AuthorId == authorId);

            if (isAuthorOfAnyBook)
            {
                // Prevent deletion as the author still has books
                return false;
            }

            _dbContext.Authors.Remove(author);
            await SaveAsync(true);
            return true;
        }

        public async Task<List<AuthorWithoutBooksDTO>> GetAll()
        {
            var authors = await _dbContext.Authors.ToListAsync();
            return authors.Select(a => DTOMapper.MapToAuthorDTOList(a)).ToList();
        }

        public async Task<AuthorDTO?> GetByAuthorId(int authorId)
        {
            var author = await _dbContext.Authors
                .Include(b => b.AuthorBooks)
                .ThenInclude(ab => ab.Book)
                .ThenInclude(book => book.GenreBooks)
                .ThenInclude(gb => gb.Genre)
                .Include(b => b.AuthorBooks)
                .ThenInclude(ab => ab.Book)
                .ThenInclude(b => b.Publisher)
                .FirstOrDefaultAsync(a => a.Id == authorId);
            if (author == null)
            {
                return null;
            }
            return DTOMapper.MapToAuthorDTO(author);
        }

        public async Task<AuthorDTO?> GetByAuthorName(string authorName)
        {
            var author = await _dbContext.Authors
                .Include(b => b.AuthorBooks)
                .ThenInclude(ab => ab.Book)
                .ThenInclude(book => book.GenreBooks)
                .ThenInclude(gb => gb.Genre)
                .Include(b => b.AuthorBooks)
                .ThenInclude(ab => ab.Book)
                .ThenInclude(b => b.Publisher)
                .FirstOrDefaultAsync(a => a.Name == authorName);
            if (author == null)
            {
                return null;
            }
            return DTOMapper.MapToAuthorDTO(author);
        }

        public async Task<bool> UpdateAuthor(int id, AuthorCreateUpdateDTO authorDTO)
        {
            var author = await _dbContext.Authors.FindAsync(id);
            if (author == null)
            {
                return false;
            }

            author.Name = authorDTO.Name;
            _dbContext.Authors.Update(author);
            await SaveAsync(true);
            return true;
        }
    }
}
