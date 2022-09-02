using Downcast.Bookmarks.Client.Model;

using Refit;

namespace Downcast.Bookmarks.Client;

public interface IBookmarksClient
{
    /// <summary>
    /// Returns user bookmarks as a stream of objects
    /// </summary>
    /// <returns></returns>
    [Get("/api/v1/bookmarks")]
    Task<ApiResponse<IEnumerable<Bookmark>>> GetBookmarks(
        [Query] BookmarksFilter filter,
        [Authorize] string token);


    /// <summary>
    /// Returns a bookmark by article id
    /// </summary>
    /// <returns></returns>
    [Get("/api/v1/bookmarks/article/{articleId}")]
    Task<ApiResponse<Bookmark>> GetBookmark(string articleId, [Authorize] string token);

    /// <summary>
    /// Deletes bookmark by article id
    /// </summary>
    /// <returns></returns>
    [Delete("/api/v1/bookmarks/article/{articleId}")]
    Task<HttpResponseMessage> DeleteBookmark(string articleId, [Authorize] string token);

    /// <summary>
    /// Adds a new bookmark for a given article Id
    /// </summary>
    /// <returns></returns>
    [Post("/api/v1/bookmarks")]
    Task<HttpResponseMessage> AddBookmark([Body] BookmarkInput bookmark, [Authorize] string token);


    /// <summary>
    /// Deletes all user bookmarks
    /// </summary>
    /// <returns></returns>
    [Delete("/api/v1/bookmarks/user/{userId}")]
    Task<HttpResponseMessage> DeleteUserBookmarks(string userId, [Authorize] string token);
}