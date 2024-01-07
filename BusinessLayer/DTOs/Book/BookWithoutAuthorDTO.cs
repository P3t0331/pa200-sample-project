﻿using BusinessLayer.DTOs.Author;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs.Book
{
    public class BookWithoutAuthorDTO
    {
        public string Title { get; set; }
        public List<GenreDTO> Genres { get; set; }
        public PublisherDTO Publisher { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}
