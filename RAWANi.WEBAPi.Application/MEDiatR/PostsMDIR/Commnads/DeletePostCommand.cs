using MediatR;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Commnads
{
    /// <summary>
    /// Command to delete a post.
    /// </summary>
    public class DeletePostCommand : IRequest<OperationResult<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the post to be deleted.
        /// </summary>
        public Guid PostID { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user profile requesting the deletion.
        /// Ensures that only the post owner or an authorized user can delete the post.
        /// </summary>
        public Guid UserProfileID { get; set; }
    }
}
