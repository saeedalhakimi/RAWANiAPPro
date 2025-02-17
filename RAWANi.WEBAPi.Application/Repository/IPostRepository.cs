using RAWANi.WEBAPi.Domain.Entities.Posts;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Repository
{
    public interface IPostRepository
    {
        Task<bool> PostsHealthCheckAsync(CancellationToken cancellationToken);
        Task<OperationResult<bool>> CreatePostAsync(Post post, CancellationToken cancellationToken);
        Task<OperationResult<bool>> IsUserProfileExistsAsync(Guid userProfileID, CancellationToken cancellationToken);
        Task<OperationResult<Post>> GetPostByPostIDAsync(Guid postID, CancellationToken cancellationToken);
        Task<OperationResult<bool>> UpdatePostImageAsync(Post post, CancellationToken cancellationToken);
        Task<OperationResult<bool>> DeletePostAsync(Guid postId, Guid userProfileId, CancellationToken cancellationToken);
        Task<OperationResult<int>> GetPostCountAsync(Guid userProfileId, CancellationToken cancellationToken);
        Task<OperationResult<IEnumerable<Post>>> GetPostsWithPaginationAsync(
            Guid userProfileId,
            int pageNumber,
            int pageSize,
            string sortColumn,
            string sortDirection,
            CancellationToken cancellationToken);
        Task<OperationResult<bool>> UpdatePostContentsAsync(Post post, CancellationToken cancellationToken);
        Task<OperationResult<bool>> IsPostExistsAsync(Guid postId, CancellationToken cancellationToken);
        Task<OperationResult<bool>> CreatePostCommentAsync(PostComment postComment, CancellationToken cancellationToken);
        Task<OperationResult<IEnumerable<PostComment>>> GetCommentsForPostAsync(
            Guid postId,
            int pageNumber,
            int pageSize,
            string sortColumn,
            string sortDirection, 
            CancellationToken cancellationToken);
        Task<OperationResult<int>> GetPostCommentsCountAsync(Guid postId, CancellationToken cancellationToken);
        Task<OperationResult<PostComment>> GetCommentByIDAsync(Guid commentID, CancellationToken cancellationToken);
        Task<OperationResult<bool>> DeleteCommentAsync(Guid commentID, Guid userProfileID, CancellationToken cancellationToken);
        Task<OperationResult<bool>> UpdateCommentContentsAsync(PostComment postComment, CancellationToken cancellationToken);

    }
}
