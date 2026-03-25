using Movies.DTOs.Requests;
using Movies.Models;

namespace Movies.Services
{
    public interface IMovieService
    {
        Movie CreateMovie (CreateMovieRequest req);
        List<Movie> GetAllMovies();
        Movie GetMovieById(int id);
        Movie UpdateMovie(int id, UpdateMovieRequest req);
        bool DeleteMovie(int id);
    }
}
