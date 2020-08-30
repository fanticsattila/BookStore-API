﻿namespace BookStore_UI.Static
{
    public static class Endpoints
    {
        public static string BaseUrl = "https://localhost:5001";
        public static string AuthorsEndpoint = $"{BaseUrl}/api/authors/";
        public static string BooksEndpoint = $"{BaseUrl}/api/books/";
        public static string RegisterEndpoint = $"{BaseUrl}/api/users/register";
    }
}
