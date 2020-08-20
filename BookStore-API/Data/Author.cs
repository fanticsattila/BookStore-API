using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore_API.Data
{
    [Table("Authors")]
    public partial class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }

        private IList<Book> books;

        public virtual IList<Book> GetBooks()
        {
            return books;
        }

        public virtual void SetBooks(IList<Book> value)
        {
            books = value;
        }
    }
}