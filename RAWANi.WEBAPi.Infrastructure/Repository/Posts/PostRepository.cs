using Azure.Core;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Contracts.AuthDtos.Responses;
using RAWANi.WEBAPi.Application.Models;
using RAWANi.WEBAPi.Application.Repository;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Entities.Posts;
using RAWANi.WEBAPi.Domain.Models;
using RAWANi.WEBAPi.Infrastructure.Data.DataWrapperFactory;
using RAWANi.WEBAPi.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RAWANi.WEBAPi.Infrastructure.Repository.Posts
{
    public class PostRepository : IPostRepository
    {
        private readonly IAppLogger<PostRepository> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabaseConnectionFactory _connectionFactory;
        private readonly string _connectionString;

        public PostRepository(
            IConfiguration configuration,
            IAppLogger<PostRepository> logger, 
            ILoggMessagingService messagingService, 
            IErrorHandler errorHandler, 
            IDatabaseConnectionFactory connectionFactory, 
            string connectionString = null)
        {
            _connectionString = connectionString ?? configuration.GetConnectionString("DefaultConnection")!;
            _logger = logger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
            _connectionFactory = connectionFactory;
        }

        public async Task<OperationResult<bool>> CreatePostAsync(
            Post post, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{_messagingService.GetLoggMessage(
                nameof(LoggMessage.ADOHandlingRequest))}");

            _logger.LogInformation("Starting post creation process for PostID: {PostID}, UserProfileID: {UserProfileID}",post.PostID.Value, post.UserProfileID.Value);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_CreatePost";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@PostID", post.PostID.Value);
                command.AddParameter("@UserProfileID", post.UserProfileID.Value);
                command.AddParameter("@PostTitle", post.PostTitle.Value);
                command.AddParameter("@PostContent", post.PostContent.Value);
                command.AddParameter("@PostImage", post.ImageLink ?? (object)DBNull.Value);
                command.AddParameter("@CreatedAt", post.CreatedAt);
                command.AddParameter("@UpdatedAt", post.UpdatedAt);
                
                _logger.LogDebug("Post parameters added to the command: {PostTitle}, {PostContent}, {ImageLink}, {CreatedAt}, {UpdatedAt}",
                    post.PostTitle.Value, post.PostContent.Value, post.ImageLink, post.CreatedAt, post.UpdatedAt);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation($"{_messagingService.GetSuccessMessage(
                    nameof(SuccessMessage.DataConnectionSuccess))}");

                _logger.LogInformation("Executing stored procedure '{StoredProcedure}' for post creation.", command.CommandText);

                int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
                if (rowsAffected > 0) 
                {
                    _logger.LogInformation($"Post {_messagingService.GetSuccessMessage(
                        nameof(SuccessMessage.DBCreationSuccess))} -rows affected: {rowsAffected}");

                    return OperationResult<bool>.Success(true);
                }
                else
                {
                    _logger.LogWarning("Post creation failed. No rows affected. PostID: {PostID}", post.PostID.Value);
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.RESOURCECREATIONFAILED,
                        "Creation Failed",
                        $"Falied To Create Post -rows affected: {rowsAffected}"
                        ));
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Post creation operation was canceled. PostID: {PostID}, Exception: {Exception}",
                    post.PostID.Value, ex.Message);
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while creating post '{post.PostID.Value}", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }

        public async Task<OperationResult<bool>> DeletePostAsync(
            Guid postId, Guid userProfileId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{_messagingService.GetLoggMessage(
                nameof(LoggMessage.ADOHandlingRequest))}");

            _logger.LogInformation("Starting post deletion process for PostID: {PostID}, UserProfileID: {UserProfileID}", postId, userProfileId);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);
                using var command = connection.CreateCommand();
                command.CommandText = "SP_DeletePost";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@PostID", postId);
                command.AddParameter("@UserProfileID", userProfileId);

                _logger.LogDebug("Post deletion parameters added to the command: {PostID}, {UserProfileID}", postId, userProfileId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation($"{_messagingService.GetSuccessMessage(nameof(SuccessMessage.DataConnectionSuccess))}");

                _logger.LogInformation("Executing stored procedure '{StoredProcedure}' for post deletion.", command.CommandText);

                int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
                if (rowsAffected > 0)
                {
                    _logger.LogInformation($"Post deleted successfully - rows affected: {rowsAffected}");
                    return OperationResult<bool>.Success(true);
                }
                else
                {
                    _logger.LogWarning("Post deletion failed. No rows affected. PostID: {PostID}", postId);
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.RESOURCENOTFOUND,
                        "Deletion Failed",
                        $"Failed to delete post. No matching record found. PostID: {postId}"
                    ));
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Post deletion operation was canceled. PostID: {PostID}, Exception: {Exception}", postId, ex.Message);
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while deleting post '{postId}'", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }

        public async Task<OperationResult<Post>> GetPostByPostIDAsync(
            Guid postID, CancellationToken cancellationToken)
        {

            _logger.LogInformation($"{_messagingService.GetLoggMessage(
                nameof(LoggMessage.ADOHandlingRequest))}");

            _logger.LogInformation($"Attempting to retrieve post id: {postID} ");
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_GetPostById";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@PostID", postID);

                _logger.LogDebug("Post id parameter added to the command: {PostID}", postID);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation($"{_messagingService.GetSuccessMessage(
                    nameof(SuccessMessage.DataConnectionSuccess))}");

                _logger.LogInformation("Executing stored procedure '{StoredProcedure}' for post creation.", command.CommandText);

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    var post = Post.FetchPostFromDatabase(
                        reader.GetGuid(reader.GetOrdinal("PostID")),
                        reader.GetGuid(reader.GetOrdinal("UserProfileID")),
                        reader.GetString(reader.GetOrdinal("PostTitle")),
                        reader.GetString(reader.GetOrdinal("PostContent")),
                        reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                        reader.IsDBNull(reader.GetOrdinal("PostImage")) ? null : reader.GetString(reader.GetOrdinal("PostImage"))
                    );

                    if (!post.IsSuccess) return OperationResult<Post>.Failure(post.Errors);

                    _logger.LogInformation(_messagingService.GetSuccessMessage(
                        nameof(SuccessMessage.RetrieveSuccess)));

                    return OperationResult<Post>.Success(post.Data!);
                }
                else
                {
                    _logger.LogWarning($"Post {postID} Not Found");
                    return OperationResult<Post>.Failure(new Error(
                        ErrorCode.NotFound,
                        "Resource Not Found",
                        $"Post with ID {postID} was not found in the database."
                    ));
                }

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Post retrieve operation was canceled. PostID: {PostID}, Exception: {Exception}",
                    postID, ex.Message);
                return _errorHandler.HandleCancelationToken<Post>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while retrieveing post '{postID}", ex);
                return _errorHandler.HandleException<Post>(ex);
            }
        }

        public async Task<OperationResult<int>> GetpostCountAsync(Guid userProfileId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_CountPostsByUser";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@UserProfileID", userProfileId);

                _logger.LogDebug("Post id parameter added to the command: {UserProfileID}", userProfileId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation($"{_messagingService.GetSuccessMessage(
                    nameof(SuccessMessage.DataConnectionSuccess))}");

                _logger.LogInformation("Executing stored procedure '{StoredProcedure}' for post creation.", command.CommandText);

                var result = await command.ExecuteScalarAsync(cancellationToken);

                if (result != null && int.TryParse(result.ToString(), out int postCount))
                {
                    _logger.LogInformation("Successfully retrieved post count: {PostCount} for UserProfileID: {UserProfileID}", postCount, userProfileId);
                    return OperationResult<int>.Success(postCount);
                }
                else
                {
                    _logger.LogWarning("Failed to retrieve post count. Returning 0 for UserProfileID: {UserProfileID}", userProfileId);
                    return OperationResult<int>.Failure(new Error(
                        ErrorCode.NotFound,
                        "Post Count Retrieval Failed",
                        "No posts found for the given UserProfileID."
                    ));
                }

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Post count retrieval operation was canceled. UserProfileID: {UserProfileID}, Exception: {Exception}",
                    userProfileId, ex.Message);
                return _errorHandler.HandleCancelationToken<int>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while retrieving post count for UserProfileID '{userProfileId}'", ex);
                return _errorHandler.HandleException<int>(ex);
            }
        }

        public async Task<OperationResult<IEnumerable<Post>>> GetPostsWithPaginationAsync(
              Guid userProfileId, int pageNumber, int pageSize, string sortColumn,
              string sortDirection, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_GetPostsByUserWithPagination";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@UserProfileID", userProfileId);
                command.AddParameter("@PageNumber", pageNumber);
                command.AddParameter("@PageSize", pageSize);
                command.AddParameter("@SortColumn", sortColumn ?? "CreatedAt");
                command.AddParameter("@SortDirection", sortDirection?.ToUpper() == "DESC" ? "DESC" : "ASC");

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Connected to database successfully.");

                _logger.LogInformation("Executing stored procedure '{StoredProcedure}' to retrieve posts.", command.CommandText);

                var posts = new List<Post>();

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    var post = Post.FetchPostFromDatabase( // Fixed method name
                        reader.GetGuid(reader.GetOrdinal("PostID")),
                        reader.GetGuid(reader.GetOrdinal("UserProfileID")),
                        reader.GetString(reader.GetOrdinal("PostTitle")),
                        reader.GetString(reader.GetOrdinal("PostContent")),
                        reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                        reader.IsDBNull(reader.GetOrdinal("PostImage")) ? null : reader.GetString(reader.GetOrdinal("PostImage"))
                    );

                    if (!post.IsSuccess)
                    {
                        _logger.LogWarning("Failed to fetch post: {Errors}", post.Errors);
                        return OperationResult<IEnumerable<Post>>.Failure(post.Errors);
                    }

                    posts.Add(post.Data);
                }

                _logger.LogInformation("Successfully retrieved {PostCount} posts for UserProfileID: {UserProfileID}", posts.Count, userProfileId);
                return OperationResult<IEnumerable<Post>>.Success(posts);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Post retrieval operation was canceled. Exception: {Exception}", ex.Message);
                return _errorHandler.HandleCancelationToken<IEnumerable<Post>>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while retrieving posts: {Exception}", ex);
                return _errorHandler.HandleException<IEnumerable<Post>>(ex);
            }
        }

        public async Task<OperationResult<bool>> IsUserProfileExistsAsync(
            Guid userProfileId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{_messagingService.GetLoggMessage(
                nameof(LoggMessage.ADOHandlingRequest))}");

            _logger.LogInformation("Start checking if user profile {UserProfile} Exists...", userProfileId);
            
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = @"SELECT COUNT(*) FROM UserProfiles WHERE UserProfileID = @UserProfileID;";
                command.CommandType = CommandType.Text;
                command.AddParameter("@UserProfileID", userProfileId);

                _logger.LogDebug("Parameters added to the command: {UserProfileID}", userProfileId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation($"{_messagingService.GetSuccessMessage(
                    nameof(SuccessMessage.DataConnectionSuccess))}");

                _logger.LogInformation("Executing query '{query}' to get the result. ", command.CommandText);

                var count = (int)await command.ExecuteScalarAsync(cancellationToken);
                _logger.LogInformation("User profile {UserProfileID} exists: {Exists}", userProfileId, count > 0);

                // Return the result
                return OperationResult<bool>.Success(count > 0);

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("The operation to check if user profile {UserProfileID} exists was canceled.", userProfileId);
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while checking user profile '{userProfileId}", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }

        public async Task<bool> PostsHealthCheckAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT TOP 1 PostID FROM Posts";
                command.CommandType = CommandType.Text;
                await connection.OpenAsync(cancellationToken);
                var result = await command.ExecuteScalarAsync(cancellationToken);
                return result != null;
            }
            catch 
            {
                return false;
            }
        }

        public async Task<OperationResult<bool>> UpdatePostImageAsync(
            Post post, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_UpdatePostImageLink";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@PostID", post.PostID.Value);
                command.AddParameter("@PostImage", post.ImageLink ?? (object)DBNull.Value);
                command.AddParameter("@UpdatedAt", post.UpdatedAt);
                command.AddOutputParameter("@RowsAffected", SqlDbType.Int);

                await connection.OpenAsync(cancellationToken);
                await command.ExecuteNonQueryAsync(cancellationToken);

                int rowsAffected = Convert.ToInt32(command.GetParameterValue("@RowsAffected"));
                if (rowsAffected > 0)
                {
                    _logger.LogInformation($"Post update successful - Rows affected: {rowsAffected}");
                    return OperationResult<bool>.Success(true);
                }
                else
                {
                    _logger.LogWarning("Post Update failed. No rows affected. PostID: {PostID}", post.PostID.Value);
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.RESOURCECREATIONFAILED,
                        "Update Failed",
                        $"Failed to update post - Rows affected: {rowsAffected}"
                    ));
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Post update operation was canceled. PostID: {PostID}, Exception: {Exception}",
                    post.PostID.Value, ex.Message);
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while updating post '{post.PostID.Value}", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }

        public async Task<OperationResult<bool>> UpdatePostContentsAsync(
            Post post, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_UpdatePostContents";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@PostID", post.PostID.Value);
                command.AddParameter("@PostTitle", post.PostTitle.Value);
                command.AddParameter("@PostContent", post.PostContent.Value);
                command.AddParameter("@UpdatedAt", post.UpdatedAt);
                command.AddOutputParameter("@RowsAffected", SqlDbType.Int);

                await connection.OpenAsync(cancellationToken);
                await command.ExecuteNonQueryAsync(cancellationToken);

                int rowsAffected = Convert.ToInt32(command.GetParameterValue("@RowsAffected"));
                if (rowsAffected > 0)
                {
                    _logger.LogInformation($"Post update successful - Rows affected: {rowsAffected}");
                    return OperationResult<bool>.Success(true);
                }
                else
                {
                    _logger.LogWarning("Post Update failed. No rows affected. PostID: {PostID}", post.PostID.Value);
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.RESOURCECREATIONFAILED,
                        "Update Failed",
                        $"Failed to update post - Rows affected: {rowsAffected}"
                    ));
                }

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Post update operation was canceled. PostID: {PostID}, Exception: {Exception}",
                    post.PostID.Value, ex.Message);
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while updating post '{post.PostID.Value}", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }

        public async Task<OperationResult<bool>> IsPostExistsAsync(
            Guid postId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = @"SELECT COUNT(*) FROM Posts WHERE PostID = @PostID;";
                command.CommandType = CommandType.Text;
                command.AddParameter("@PostID", postId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation($"{_messagingService.GetSuccessMessage(
                    nameof(SuccessMessage.DataConnectionSuccess))}");

                var count = (int)await command.ExecuteScalarAsync(cancellationToken);

                // Return the result
                return OperationResult<bool>.Success(count > 0);
            }
            catch (OperationCanceledException ex)
            {
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                return _errorHandler.HandleException<bool>(ex);
            }
        }

        public async Task<OperationResult<bool>> CreatePostCommentAsync(
            PostComment postComment, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_CreatePostComment";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@CommentID", postComment.CommentID.Value);
                command.AddParameter("@PostID", postComment.PostID.Value);
                command.AddParameter("@UserProfileID", postComment.UserProfileID.Value);
                command.AddParameter("@CommentContent", postComment.CommentContent.Value);
                command.AddParameter("@CreatedAt", postComment.CreatedAt);
                command.AddParameter("@UpdatedAt", postComment.UpdatedAt);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation($"{_messagingService.GetSuccessMessage(
                    nameof(SuccessMessage.DataConnectionSuccess))}");

                _logger.LogInformation("Executing stored procedure '{StoredProcedure}' for post creation.", command.CommandText);

                int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
                if (rowsAffected > 0)
                {
                    _logger.LogInformation($"PostComment {_messagingService.GetSuccessMessage(
                        nameof(SuccessMessage.DBCreationSuccess))} -rows affected: {rowsAffected}");

                    return OperationResult<bool>.Success(true);
                }
                else
                {
                    _logger.LogWarning("PostComment creation failed. No rows affected. PostID: {PostID}", postComment.CommentID.Value);
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.RESOURCECREATIONFAILED,
                        "Creation Failed",
                        $"Falied To Create PostComment -rows affected: {rowsAffected}"
                        ));
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("PostComment creation operation was canceled. PostID: {PostID}, Exception: {Exception}",
                    postComment.PostID.Value, ex.Message);
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while creating postComment '{postComment.CommentID.Value}", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }

        public async Task<OperationResult<IEnumerable<PostComment>>> GetCommentsForPostAsync(
            Guid postId, int pageNumber, int pageSize, string sortColumn, 
            string sortDirection, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_GetCommentsForAPost";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@PostID", postId);
                command.AddParameter("@PageNumber", pageNumber);
                command.AddParameter("@PageSize", pageSize);
                command.AddParameter("@SortColumn", sortColumn ?? "CreatedAt");
                command.AddParameter("@SortDirection", sortDirection?.ToUpper() == "DESC" ? "DESC" : "ASC");

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Connected to database successfully.");

                _logger.LogInformation("Executing stored procedure '{StoredProcedure}' to retrieve posts.", command.CommandText);
                var comments = new List<PostComment>();

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    var comment = PostComment.FetchFromDatabase(
                        reader.GetGuid(reader.GetOrdinal("CommentID")),
                        reader.GetGuid(reader.GetOrdinal("PostID")),
                        reader.GetGuid(reader.GetOrdinal("UserProfileID")),
                        reader.GetString(reader.GetOrdinal("CommentContent")),
                        reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                        );

                    if (!comment.IsSuccess)
                    {
                        _logger.LogWarning("Failed to fetch post comments: {Errors}", comment.Errors);
                        return OperationResult<IEnumerable<PostComment>>.Failure(comment.Errors);
                    }

                    comments.Add(comment.Data);
                }

                _logger.LogInformation("Successfully retrieved {CommentCount} comments for post: {PostID}", comments.Count, postId);
                return OperationResult<IEnumerable<PostComment>>.Success(comments);
            }
            catch (OperationCanceledException ex)
            {
                return _errorHandler.HandleCancelationToken<IEnumerable<PostComment>>(ex);
            }
            catch (Exception ex)
            {
                return _errorHandler.HandleException<IEnumerable<PostComment>>(ex);
            }
        }

        public async Task<OperationResult<int>> GetPostCommentsCountAsync(Guid postId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_CountPostCommentsByPost";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@PostID", postId);


                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation($"{_messagingService.GetSuccessMessage(
                    nameof(SuccessMessage.DataConnectionSuccess))}");

                _logger.LogInformation("Executing stored procedure '{StoredProcedure}' for comment count.", command.CommandText);
                var result = await command.ExecuteScalarAsync(cancellationToken);

                if (result != null && int.TryParse(result.ToString(), out int postCount))
                {
                    _logger.LogInformation("Successfully retrieved comments count: {CommentCount} for PostID: {PostID}", postCount, postId);
                    return OperationResult<int>.Success(postCount);
                }
                else
                {
                    _logger.LogWarning("Failed to retrieve comment count. Returning 0 for PostID: {PostID}", postId);
                    return OperationResult<int>.Failure(new Error(
                        ErrorCode.NotFound,
                        "Comment Count Retrieval Failed",
                        "No comments found for the given post."
                    ));
                }

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("comment count retrieval operation was canceled. PostID: {PostID}, Exception: {Exception}",
                    postId, ex.Message);
                return _errorHandler.HandleCancelationToken<int>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while retrieving comment count for PostID '{postId}'", ex);
                return _errorHandler.HandleException<int>(ex);
            }
        }

        public async Task<OperationResult<PostComment>> GetCommentByIDAsync(Guid commentID, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_GetCommentByID";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@CommentID", commentID);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation($"{_messagingService.GetSuccessMessage(
                    nameof(SuccessMessage.DataConnectionSuccess))}");

                _logger.LogInformation("Executing stored procedure '{StoredProcedure}' for post creation.", command.CommandText);

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    var comment = PostComment.FetchFromDatabase(
                                            reader.GetGuid(reader.GetOrdinal("CommentID")),
                                            reader.GetGuid(reader.GetOrdinal("PostID")),
                                            reader.GetGuid(reader.GetOrdinal("UserProfileID")),
                                            reader.GetString(reader.GetOrdinal("CommentContent")),
                                            reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                            reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                                            );
                    if (!comment.IsSuccess) return OperationResult<PostComment>.Failure(comment.Errors);
                    
                    _logger.LogInformation(_messagingService.GetSuccessMessage(
                        nameof(SuccessMessage.RetrieveSuccess)));

                    return OperationResult<PostComment>.Success(comment.Data!);
                }
                else
                {
                    _logger.LogWarning($"Comment {commentID} Not Found");
                    return OperationResult<PostComment>.Failure(new Error(
                        ErrorCode.NotFound,
                        "Resource Not Found",
                        $"Comment with ID {commentID} was not found in the database."
                    ));
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Comment retrieve operation was canceled. CommentID: {CommentID}, Exception: {Exception}",
                    commentID, ex.Message);
                return _errorHandler.HandleCancelationToken<PostComment>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while retrieveing comment '{commentID}", ex);
                return _errorHandler.HandleException<PostComment>(ex);
            }
        }

        public async Task<OperationResult<bool>> DeleteCommentAsync(
            Guid commentID, Guid userProfileId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);
                using var command = connection.CreateCommand();
                command.CommandText = "SP_DeleteComment";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@CommentID", commentID);
                command.AddParameter("@UserProfileID", userProfileId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation($"{_messagingService.GetSuccessMessage(nameof(SuccessMessage.DataConnectionSuccess))}");

                _logger.LogInformation("Executing stored procedure '{StoredProcedure}' for comment deletion.", command.CommandText);

                int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
                if (rowsAffected > 0)
                {
                    _logger.LogInformation($"Comment deleted successfully - rows affected: {rowsAffected}");
                    return OperationResult<bool>.Success(true);
                }
                else
                {
                    _logger.LogWarning("Comment deletion failed. No rows affected. CommentID: {CommentID}", commentID);
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.RESOURCENOTFOUND,
                        "Deletion Failed",
                        $"Failed to delete comment. No matching record found. Comment Id: {command}"
                    ));
                }

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Comment deletion operation was canceled. CommentID: {CommentID}, Exception: {Exception}", commentID, ex.Message);
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while deleting comment '{commentID}'", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }

        public async Task<OperationResult<bool>> UpdateCommentContentsAsync(
            PostComment postComment, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await using var connection = await _connectionFactory.CreateConnectionAsync(
                    _connectionString, cancellationToken);

                var command = connection.CreateCommand();
                command.CommandText = "SP_UpdateCommentContents";
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter("@CommentID", postComment.CommentID.Value);
                command.AddParameter("@CommentContent", postComment.CommentContent.Value);
                command.AddParameter("@UpdatedAt", postComment.UpdatedAt);
                command.AddOutputParameter("@RowsAffected", SqlDbType.Int);

                await connection.OpenAsync(cancellationToken);
                await command.ExecuteNonQueryAsync(cancellationToken);

                int rowsAffected = Convert.ToInt32(command.GetParameterValue("@RowsAffected"));

                if (rowsAffected > 0)
                {
                    _logger.LogInformation($"Comment update successful - Rows affected: {rowsAffected}");
                    return OperationResult<bool>.Success(true);
                }
                else
                {
                    _logger.LogWarning("Comment Update failed. No rows affected. CommentID: {CommentID}", postComment.CommentID.Value);
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.RESOURCECREATIONFAILED,
                        "Update Failed",
                        $"Failed to update comment - Rows affected: {rowsAffected}"
                    ));
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Comment update operation was canceled. CommentID: {CommentID}, Exception: {Exception}", postComment.CommentID.Value, ex.Message);
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while updating comment '{postComment.CommentID.Value}", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }
    }
}
